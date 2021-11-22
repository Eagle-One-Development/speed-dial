using System.Threading.Tasks;

using Sandbox;

using SpeedDial.Classic.GameSound;

namespace SpeedDial.Classic.Player {
	public partial class SpeedDialPlayer {
		public SoundTrack SoundTrack { get; set; }
		public bool SoundtrackPlaying { get; set; }

		[ClientRpc]
		public void PlayRoundendClimax() {
			if(!GetMusicBool()) return;
			SoundTrack.FromScreen("climax");
			_ = StopSoundtrackAsync();
		}

		private async Task StopSoundtrackAsync(int delay = 5) {
			await GameTask.DelaySeconds(delay);
			_ = SoundTrack.Stop(5, 500);
			SoundtrackPlaying = false;
		}


		[ClientRpc]
		public void PlaySoundtrack() {
			if(!GetMusicBool()) return;
			_ = PlaySoundtrackAsync(ClassicGamemode.Instance.CurrentSoundtrack, 2.5f);
		}

		private async Task PlaySoundtrackAsync(string track, float delay) {
			await GameTask.DelaySeconds(delay);
			if(!SoundtrackPlaying) {
				SoundTrack = SoundTrack.FromScreen(track);
				SoundtrackPlaying = true;
			}
		}

		[ClientRpc]
		public void StopSoundtrack(bool instant = false) {
			if(!GetMusicBool()) return;
			if(instant) {
				SoundTrack?.Stop();
				SoundtrackPlaying = false;
			} else {
				SoundTrack?.Stop(1);
				SoundtrackPlaying = false;
			}
		}

		[ClientRpc]
		public void FadeSoundtrack(float volumeTo) {
			if(!GetMusicBool()) return;
			SoundTrack?.FadeVolumeTo(volumeTo);
		}

		[ClientRpc]
		public void PlayUISound(string sound) {
			Sound.FromScreen(sound);
		}
	}
}
