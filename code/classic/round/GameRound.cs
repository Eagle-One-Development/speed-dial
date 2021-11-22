using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.UI;

namespace SpeedDial.Classic {
	public class GameRound : BaseRound {
		public override string RoundName => "Game Round";
		public override int RoundDuration => 300; // 300

		//private bool _roundStarted;


		public override void OnPlayerSpawn(SpeedDialPlayer player) {
			//if(Players.Contains(player)) return;

			//AddPlayer(player);

			player.StopSoundtrack(To.Single(player), true);
			player.PlaySoundtrack(To.Single(player));

			base.OnPlayerSpawn(player);
		}

		protected override void OnStart() {
			var players = Client.All;
			foreach(var p in players.ToArray()) {
				if(p.Pawn != null) {
					(p.Pawn as SpeedDialPlayer).ResetWeapon();
					(p.Pawn as SpeedDialPlayer).Freeze = false;
				}
			}
			_ = PlayClimaxMusic(RoundDuration - 10);
		}

		private async Task PlayClimaxMusic(int delay) {
			await GameTask.DelaySeconds(delay);
			foreach(var p in Client.All.ToArray()) {
				if(p.Pawn != null) {
					(p.Pawn as SpeedDialPlayer).PlayRoundendClimax(To.Single(p));
				}
			}
		}

		protected override void OnTimeUp() {
			ClassicGamemode.Instance.ChangeRound(new PostRound());

			base.OnTimeUp();
		}
	}
}