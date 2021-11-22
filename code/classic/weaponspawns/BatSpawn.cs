using Sandbox;

namespace SpeedDial.Classic.WeaponSpawns {
	[Library("sd_weaponspawn_bat")]
	[Hammer.EditorModel("models/weapons/melee/melee.vmdl")]
	[Hammer.EntityTool("Bat", "Speed Dial Weapons", "Bat spawn point")]
	public class BatWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_bat";
	}
}
