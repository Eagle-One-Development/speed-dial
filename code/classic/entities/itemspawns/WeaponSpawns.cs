using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Entities;

[Library("sd_weaponspawn_random", Title = "Random Weapon Spawn")]
[EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
[EntityTool("Random Weapon", "Speed-Dial Weaponspawns", "Spawns random weapons.")]
public partial class ClassicRandomWeaponSpawn : ClassicWeaponSpawn {
	public override string WeaponClass => WeaponBlueprint.GetRandomSpawnable().WeaponClass;
}

[Library("sd_weaponspawn_pistol")]
[EditorModel("models/weapons/pistol/prop_pistol.vmdl")]
[EntityTool("Pistol", "Speed-Dial Weaponspawns", "Spawns Pistols.")]
public class PistolWeaponSpawn : ClassicWeaponSpawn {
	public override string WeaponClass => "sd_pistol";
}

[Library("sd_weaponspawn_rifle")]
[EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
[EntityTool("Rifle", "Speed-Dial Weaponspawns", "Spawns Rifles.")]
public class RifleWeaponSpawn : ClassicWeaponSpawn {
	public override string WeaponClass => "sd_rifle";
}

[Library("sd_weaponspawn_smg")]
[EditorModel("models/weapons/smg/prop_smg.vmdl")]
[EntityTool("SMG", "Speed-Dial Weaponspawns", "Spawns SMGs.")]
public class SmgWeaponSpawn : ClassicWeaponSpawn {
	public override string WeaponClass => "sd_smg";
}

[Library("sd_weaponspawn_shotgun")]
[EditorModel("models/weapons/shotgun/prop_shotgun.vmdl")]
[EntityTool("Shotgun", "Speed-Dial Weaponspawns", "Spawns Shotguns.")]
public class ShotgunWeaponSpawn : ClassicWeaponSpawn {
	public override string WeaponClass => "sd_shotgun";
}

[Library("sd_weaponspawn_sniper")]
[EditorModel("models/weapons/rifle/prop_rifle.vmdl")]
[EntityTool("Sniper", "Speed-Dial Weaponspawns", "Spawns Snipers.")]
public class SniperWeaponSpawn : ClassicWeaponSpawn {
	public override string WeaponClass => "sd_sniper";
}

[Library("sd_weaponspawn_roomclearer")]
[EditorModel("models/weapons/shotgun/prop_roomclearer.vmdl")]
[EntityTool("Room-Clearer", "Speed-Dial Weaponspawns", "Spawns Room-Clearer Shotguns.")]
public class RoomClearerWeaponSpawn : ClassicWeaponSpawn {
	public override string WeaponClass => "sd_roomclearer";
}

[Library("sd_weaponspawn_bat")]
[EditorModel("models/weapons/melee/melee.vmdl")]
[EntityTool("Baseball Bat", "Speed-Dial Weaponspawns", "Spawns Baseball Bats.")]
public class BaseballBatWeaponSpawn : ClassicWeaponSpawn {
	public override string WeaponClass => "sd_bat";
}
