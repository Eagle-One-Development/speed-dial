//CREDIT: Modified from Espionage.Engine by Jake Wooshito
namespace SpeedDial;

public partial class BasePlayer : AnimEntity {

	public virtual float RespawnTime => 1;
	public virtual string ModelPath { get; set; } = "models/citizen/citizen.vmdl";
	[Net] public float MaxHealth { get; set; } = 100;
	[Net] public TimeSince TimeSinceDied { get; set; }
	[Predicted] Entity LastActiveChild { get; set; }

	public override void Simulate(Client cl) {
		if(LifeState == LifeState.Dead) {
			if(TimeSinceDied > RespawnTime && IsServer && CanRespawn()) {
				Respawn();
			}
		}

		SimulateActiveChild(cl, ActiveChild);
		GetActiveController()?.Simulate(cl, this, GetActiveAnimator());
	}

	public virtual bool CanRespawn() {
		return true;
	}

	[ClientRpc]
	public static void SoundFromEntity(string name, Entity entity) {
		Sound.FromEntity(name, entity);
	}

	[ClientRpc]
	public static void SoundFromScreen(string name) {
		Sound.FromScreen(name);
	}

	[ClientRpc]
	public static void SoundFromScreen(string name, float x, float y) {
		Sound.FromScreen(name, x, y);
	}

	[ClientRpc]
	public static void SoundFromWorld(string name, Vector3 position) {
		Sound.FromWorld(name, position);
	}

	public virtual void SimulateActiveChild(Client client, Entity child) {
		if(LastActiveChild != child) {
			OnActiveChildChanged(LastActiveChild, child);
			LastActiveChild = child;
		}

		if(!LastActiveChild.IsValid())
			return;

		if(LastActiveChild.IsAuthority) {
			LastActiveChild.Simulate(client);
		}
	}

	public virtual void OnActiveChildChanged(Entity previous, Entity next) {
		previous?.ActiveEnd(this, previous.Owner != this);
		next?.ActiveStart(this);
	}

	public override void FrameSimulate(Client cl) {
		base.FrameSimulate(cl);

		GetActiveController()?.FrameSimulate(cl, this, GetActiveAnimator());
	}

	public virtual void CreateHull() {
		CollisionGroup = CollisionGroup.Player;
		AddCollisionLayer(CollisionLayer.Player);
		SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));

		MoveType = MoveType.MOVETYPE_WALK;
		EnableHitboxes = true;
	}

	public override void BuildInput(InputBuilder input) {
		if(input.StopProcessing)
			return;

		ActiveChild?.BuildInput(input);

		GetActiveController()?.BuildInput(input);

		if(input.StopProcessing)
			return;

		GetActiveAnimator()?.BuildInput(input);
	}

	//
	// Pawn States
	//

	public virtual void InitialRespawn() {
		Respawn();
		// call round stuff after respawn to potentially override stuff in it
		Game.Current.ActiveGamemode?.ActiveRound?.OnPawnJoined(this);
	}

	public virtual void Respawn() {
		Host.AssertServer();

		SetModel(ModelPath);

		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;

		CreateHull();
		ResetInterpolation();

		Game.Current.PawnRespawned(this);
		Game.Current.MoveToSpawnpoint(this);
		Game.Current.ActiveGamemode?.ActiveRound?.OnPawnRespawned(this);
	}

	public override void OnKilled() {
		LifeState = LifeState.Dead;
		Game.Current.PawnKilled(this, LastRecievedDamage);
	}

	public DamageInfo LastRecievedDamage { get; set; }

	public override void TakeDamage(DamageInfo info) {
		// If this pawn is allowed to take damage. Then take damage
		if(Game.Current.PawnDamaged(this, ref info)) {
			LastRecievedDamage = info;
			base.TakeDamage(info);
			this.ProceduralHitReaction(info);
		}
	}

	/// <summary>
	///  called before the pawn gets cleaned up upon a client disconnect
	/// </summary>
	public virtual void OnClientDisconnected() { }

	//
	// Controller
	//

	[Net, Predicted]
	public PawnController Controller { get; set; }

	[Net]
	public PawnController DevController { get; set; }

	public virtual PawnController GetActiveController() {
		return DevController ?? Controller;
	}

	//
	// Animator
	//

	[Net, Predicted]
	protected PawnAnimator Animator { get; set; }

	public virtual PawnAnimator GetActiveAnimator() => Animator;
}
