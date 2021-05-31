using Sandbox;
using System;

namespace SpeedDial.Weapons {
	[Library("sd_tac", Title = "TAC-7")]
	partial class Tac7 : BaseSpeedDialWeapon {

		public override float PrimaryRate => 2.0f;
		public override float SecondaryRate => 1.0f;
		public override int ClipSize => 30;
		public int burst = 3;
		private int curBurst = 0;
		private bool isFiring;
		public TimeSince burstTimer;
		public override int HoldType => 4;
		public override Vector4 ScreenShakeParameters => new(0.5f, 4.0f, 1.0f, 0.5f);
		public override float BulletSpread => 0.025f;
		public override float BulletForce => 1.5f;
		public override float BulletDamage => 100;
		public override float BulletSize => 3;
		public override string ShootSound => "rust_smg.shoot";

		// Override for burst fire
		public override void Simulate(Client owner) {

			base.Simulate(owner);

			if(isFiring) {
				if(burstTimer > 0.07f && curBurst < burst) {
					curBurst++;
					if(!TakeAmmo(1)) {
						burstTimer = 0;
						curBurst = 0;
						isFiring = false;
						return;
					}
					(Owner as AnimEntity).SetAnimBool("b_attack", true);

					ShootEffects();
					PlaySound(ShootSound);
					ShootBullet(BulletSpread * (float)((curBurst * 1.5) + 1), BulletForce, BulletDamage, BulletSize);
					burstTimer = 0;
				}

				if(curBurst >= burst) {
					burstTimer = 0;
					curBurst = 0;
					isFiring = false;
				}
			}
		}

		public override void AttackPrimary() {
			TimeSincePrimaryAttack = 0;

			if(!isFiring) {
				isFiring = true;
			}
		}
	}
}
