using System;
using System.Linq;

using Sandbox;

using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.Rounds {
	public partial class PostRound: TimedRound {
		public override TimeSpan RoundDuration => TimeSpan.FromSeconds(11);
		public override string RoundText => "Round over.";

		protected override void OnStart() {
			base.OnStart();

			foreach(var client in Client.All.Where(x => x.Pawn is ClassicPlayer)) {
				var pawn = client.Pawn as ClassicPlayer;

				pawn.Frozen = true;
			}
		}

		protected override void OnFinish() {
			base.OnFinish();
			Game.Current.ActiveGamemode?.SetRound(new PreRound());
		}
	}
}