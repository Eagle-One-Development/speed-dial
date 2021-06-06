using Sandbox;
using SpeedDial.Player;

namespace SpeedDial.Weapons {
	[Library("sd_sniper", Title = "Sniper Rifle (TEST)")]
	partial class Sniper : BaseSpeedDialWeapon {
		public override float PrimaryRate => 1.0f;
		public override int ClipSize => 16;
		public override float BulletSpread => 0.01f;
		public override float BulletForce => 3.5f;
		public override float BulletDamage => 100;
		public override float BulletSize => 5;
		public override int HoldType => 4;
		public override string ShootSound => "sd_pistol_shoot";
		public override string WorldModel => "models/weapons/sk_prop_rifle_01.vmdl";
		public override string AttachementName => "rifle_attach";
		public override bool Penetrate => true;
	}
}