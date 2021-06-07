using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Player;
using SpeedDial.UI;
namespace SpeedDial {
	public class GameRound : BaseRound {
		public override string RoundName => "Game Round";
		public override int RoundDuration => 300;

		//private bool _roundStarted;


		public override void OnPlayerSpawn(SpeedDialPlayer player) {
			if(Players.Contains(player)) return;

			AddPlayer(player);

			//if(_roundStarted) {
			//	// ouch
			//}

			base.OnPlayerSpawn(player);
		}

		protected override void OnStart() {
			Log.Info("GAME ROUND START");
			var players = Client.All;
			foreach(var p in players.ToArray()) {
				(p.Pawn as SpeedDialPlayer).ResetWeapon();
				(p.Pawn as SpeedDialPlayer).Unfreeze();
			}
			SpeedDialHud.Scoreboard?.UpdateScoreboard();
		}

		protected override void OnFinish() {
			Log.Info("Finished Game Round");

		}

		protected override void OnTimeUp() {
			Log.Info("Game Round Up!");

			SpeedDialGame.Instance.ChangeRound(new PostRound());

			base.OnTimeUp();
		}
	}
}
