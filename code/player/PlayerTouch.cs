using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.Meds;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer {
		public override void StartTouch(Entity other) {
			if(timeSinceDropped < 1) return;

			if(IsClient) return;

			if(other is PickupTrigger pt) {
				if(other.Parent is BaseSpeedDialWeapon wep1) {
					StartTouch(other.Parent);

					if(wep1.PhysicsBody.IsValid()) {
						float magnitude = wep1.PhysicsBody.Velocity.Length;
						//Log.Info($"Velocity: {magnitude}");
						if(magnitude > 450f) {

							wep1.PhysicsBody.EnableAutoSleeping = false;
							Sound.FromEntity("smack", this);
							CauseOfDeath = COD.Thrown;
							KillMyself(wep1.PreviousOwner);
							wep1.Velocity *= -0.5f;
							//if (wep1.PhysicsBody.Velocity.Normal.Dot(other.EyeRot.Forward.Normal) < 0.4f || Inventory.Active is not BaseSpeedDialWeapon) {
							//	
							//} 
							//else {
							//	wep1.PhysicsBody.EnableAutoSleeping = false;
							//	Sound.FromEntity("smack", this);
							//	Sound.FromEntity("sd_deflect_ricochet", this);
							//	wep1.Velocity *= -0.5f;
							//	if(Inventory.DropActive() is BaseSpeedDialWeapon drop) {
							//		drop.PhysicsBody.ApplyForce(EyeRot.Forward + ((Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 0.8f * 0.25f));
							//	}
							//}
						}
					}
				}

				if(!MedTaken && other.Parent is BaseMedication drug) {
					StartTouch(other.Parent);
					drug.PickUp(this);
					MedTaken = true;
					if(drug.Drug != DrugType.Leaf) {

					} else {
						//Since Leaf lets you take an extra hit we don't need to do any kind of effect over time so we can just set the health to 200
						Health = 200f;

					}

					DrugParticles = Particles.Create(drug.ParticleName);
					DrugParticles.SetForward(0, Vector3.Up);
					DrugParticles.SetEntityBone(0, this, GetBoneIndex("head"), Transform.Zero, true);

					CurrentDrug = drug.Drug;
					ResetTimeSinceMedTaken = true;
					MedTaken = true;
					MedDuration = drug.DrugDuration;
					DBump(drug.DrugName, this, drug.DrugFlavor);

				}
				return;
			}
		}

		[ClientRpc]
		public void DBump(string s, SpeedDialPlayer p, string f) {
			if(p == Local.Pawn) {
				DrugBump(s, f);
			}

		}

		public override void Touch(Entity other) {

			if(timeSinceDropped < 1f) return;

			if(IsClient) return;

			if(other is PickupTrigger) {
				if(other.Parent is BaseSpeedDialWeapon) {
					Touch(other.Parent);
					pickup = true;
				}
				return;
			}
			pickUpEntity = other;
		}

		public override void EndTouch(Entity other) {
			base.EndTouch(other);
			if(other is PickupTrigger) {
				if(other.Parent is BaseSpeedDialWeapon) {
					Touch(other.Parent);
					pickUpEntity = null;
					pickup = false;
				}
				return;
			}
		}
	}
}
