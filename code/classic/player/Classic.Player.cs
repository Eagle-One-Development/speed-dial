using SpeedDial.Classic.Drugs;
using SpeedDial.Classic.UI;
using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Player;

public partial class ClassicPlayer : BasePlayer
{
	[Net] public bool Frozen { get; set; }
	public Character Character { get { return Character.All.ElementAtOrDefault( CharacterIndex ); } }
	[Net] public int CharacterIndex { get; set; }
	[Net] public bool ActiveDrug { get; set; }
	[Net] public DrugType DrugType { get; set; }
	[Net] public TimeSince TimeSinceDrugTaken { get; set; }
	public Particles DrugParticles;
	public virtual float DrugDuration => 8f;
	public override float RespawnTime => 1.5f;
	[Net] public TimeSince TimeSinceMurdered { get; set; }

	public override void InitialRespawn()
	{
		CharacterIndex = Rand.Int( 0, Character.All.Count - 1 );

		Client.SetValue( "score", 0 );
		Client.SetValue( "maxcombo", 0 );
		Client.SetValue( "combo", 0 );

		base.InitialRespawn();
	}

	public override void Respawn()
	{
		Host.AssertServer();

		Model = Character.CharacterModel;

		LifeState = LifeState.Alive;
		Health = 100;
		DeathCause = CauseOfDeath.Generic;
		Velocity = Vector3.Zero;

		CreateHull();
		ResetInterpolation();

		Controller = new ClassicController();
		Animator = new ClassicAnimator();
		Camera = new ClassicCamera();

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableLagCompensation = true;

		Game.Current.PawnRespawned( this );
		Game.Current.MoveToSpawnpoint( this );
		Game.Current.ActiveGamemode?.ActiveRound?.OnPawnRespawned( this );

		// reset drug
		ActiveDrug = false;
		DrugParticles?.Destroy( true );

		Frozen = false;
		GiveWeapon( Character.WeaponClass );

		// just in case this was left open for some reason
		WinScreen.SetState( To.Single( Client ), false );

		// we died, so there's no way anybody still has a highlight on us
		UpdateGlow( To.Everyone, this, false, Color.Black );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( ActiveChild is not null )
		{
			TimeSinceWeaponCarried = 0;
		}

		if ( Input.Pressed( InputButton.SecondaryAttack ) )
		{
			HandleAttack2();
			if ( ActiveChild != null && !Frozen )
			{
				ThrowWeapon();
			}
		}

		if ( ActiveChild == null && Input.Down( InputButton.PrimaryAttack ) && TimeSinceMeleeStarted >= 0.6f && !Frozen )
		{
			StartMelee();
		}

		if ( ActiveMelee )
		{
			SimulateMelee();
		}

		// handle weapon pickup after throwing so you can swap
		// TODO: hold input for pickup when activechild is null?
		// last bit is to prevent the player from immediately grabbing it out of the air again after throwing if spamming rightclick
		if ( ActiveChild is null && Pickup && PickupWeapon is not null && PickupWeapon.IsValid() && Input.Pressed( InputButton.SecondaryAttack ) && (TimeSincePickup > (PickupWeapon.PreviousOwner == this ? 0.3f : 0)) )
		{
			var weapon = PickupWeapon;
			Pickup = false;
			PickupWeapon = null;

			TimeSincePickup = 0;

			ActiveChild = weapon;
			weapon.OnCarryStart( this );
		}

		SetAnimParameter( "b_polvo", ActiveDrug && DrugType is DrugType.Polvo );

		if ( ActiveDrug )
		{
			SimulateDrug();
		}

		// DEBUG: spawn a random gun
		if ( Debug.Enabled && Input.Pressed( InputButton.Zoom ) )
		{
			using ( Prediction.Off() )
			{
				if ( IsServer )
				{
					var ent = WeaponBlueprint.Create( WeaponBlueprint.GetRandomSpawnable() );
					ent.Position = EyePosition;
				}
			}
		}

		// reset combo
		if ( TimeSinceMurdered > 5f )
		{
			Client.SetValue( "combo", 0 );
		}
	}

	public void RefreshCharacter()
	{
		Model = Character.CharacterModel;
		GiveWeapon( Character.WeaponClass );
	}

	[ClientRpc]
	public static void UpdateGlow( ModelEntity ent, bool state, Color col )
	{
		if ( !ent.IsValid() ) return;
		ent.SetGlowState( state, col );
	}

	public virtual void SimulateDrug()
	{
		if ( TimeSinceDrugTaken >= DrugDuration )
		{
			ActiveDrug = false;
			DrugParticles?.Destroy();
			return;
		}
	}

	public virtual void HandleAttack2() { }

	[Net, Predicted] public TimeSince TimeSinceMeleeStarted { get; set; }
	[Net, Predicted] public bool ActiveMelee { get; set; }

	public void StartMelee()
	{
		ActiveMelee = true;
		TimeSinceMeleeStarted = 0;
	}

	public void SimulateMelee()
	{
		if ( TimeSinceMeleeStarted > 0.2f )
		{
			ActiveMelee = false;
			var forward = EyeRotation.Forward;
			Vector3 pos = EyePosition + Vector3.Down * 20f;
			var tr = Trace.Ray( pos, pos + forward * 40f )
			.UseHitboxes()
			.WithAnyTags( "solid", "player" )
			.Ignore( this )
			.Size( 20f )
			.Run();

			PlaySound( "woosh" );

			SetAnimParameter( "b_attack", true );

			if ( !tr.Entity.IsValid() || !this.Alive() ) return;

			using ( Prediction.Off() )
			{
				if ( IsServer )
				{
					var damage = DamageInfo.FromBullet( tr.EndPosition, EyeRotation.Forward * 100, 200 )
						.UsingTraceResult( tr )
						.WithAttacker( this );

					damage.Position = Position;
					tr.Entity.TakeDamage( damage );
				}
			}

			PlaySound( "smack" );

			if ( tr.Entity is ClassicPlayer player )
			{
				player.DeathCause = CauseOfDeath.Punch;
			}
		}
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( ActiveDrug && DrugType == DrugType.Leaf )
		{
			// leaf makes us get less damage
			info.Damage /= 5f;
		}
		base.TakeDamage( info );
	}

	public override void StartTouch( Entity other )
	{
		// this is a pickuptrigger, we could pick it up
		if ( other is BasePickupTrigger trigger )
		{
			if ( trigger.Parent is Weapon weapon )
			{
				PickupWeapon = weapon;
				Pickup = true;
			}
			// this is a gun, it could kill us
		}
		else if ( other is Weapon wep )
		{
			if ( wep.PhysicsBody.IsValid() )
			{
				if ( wep.CanImpactKill && this != wep.PreviousOwner && wep.Velocity.Length > 450f )
				{
					Sound.FromEntity( "smack", this );
					ImpactKill( wep.PreviousOwner, wep );

					// bounce off the body
					if ( IsServer )
					{
						wep.Velocity *= -0.3f;
						wep.CanImpactKill = false;
					}
				}
			}
		}
	}

	public void ImpactKill( Entity attacker, Entity weapon )
	{
		DeathCause = CauseOfDeath.Impact;
		DamageInfo info = new();
		info.Damage = 200;
		info.Attacker = attacker;
		// don't set the weapon because otherwise it'd count as a bullet death cause...
		// this should probably be done better
		//info.Weapon = weapon;
		TakeDamage( info );
		PlaySound( "smack" );
	}

	public override void Touch( Entity other )
	{
		if ( other is BasePickupTrigger trigger )
		{
			if ( trigger.Parent is Weapon weapon )
			{
				if ( PickupWeapon is null || !Pickup )
				{
					PickupWeapon = weapon;
					Pickup = true;
				}
				// handle drugs
				// do this in touch since drugs can run out while we're standing on one
			}
			else if ( trigger.Parent is ClassicBaseDrug drug && !ActiveDrug )
			{
				HandleDrugTaken( drug );
			}
		}
	}

	public virtual void HandleDrugTaken( ClassicBaseDrug drug )
	{
		drug.Taken( this );
	}

	public override void EndTouch( Entity other )
	{
		if ( other is BasePickupTrigger trigger )
		{
			if ( trigger.Parent is Weapon weapon )
			{
				if ( weapon == PickupWeapon )
				{
					PickupWeapon = null;
					Pickup = false;
				}
			}
		}
	}

	[ConCmd.Server( "set_score" )]
	public static void SetScore( int score )
	{
		if ( ConsoleSystem.Caller.Pawn is ClassicPlayer player )
		{
			if ( !Debug.Enabled )
			{
				Log.Warning( "Debug mode needs to be enabled for this command." );
				return;
			}
			player.Client.SetValue( "score", score.Clamp( 0, int.MaxValue ) );
		}
	}

	[ConCmd.Server( "set_char" )]
	public static void SetCharacter( int index )
	{
		if ( ConsoleSystem.Caller.Pawn is ClassicPlayer player )
		{
			player.OnSetCharacter( index );
		}
	}

	public virtual void OnSetCharacter( int index )
	{
		Log.Debug( $"char index set to {index}" );
		index = index.Clamp( 0, Character.All.Count );
		CharacterIndex = index;

		if ( Gamemode.Instance.Preparing || Gamemode.Instance.Waiting )
			RefreshCharacter();
	}
}
