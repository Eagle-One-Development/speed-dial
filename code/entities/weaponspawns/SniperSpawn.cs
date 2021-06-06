using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.WeaponSpawns {
	[Library("sd_weaponspawn_sniper")]
	public class SniperWeaponSpawn : BaseWeaponSpawn {
		public override string WeaponClass => "sd_sniper";
	}
}