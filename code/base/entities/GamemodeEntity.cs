﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Sandbox;

namespace SpeedDial {
	[Library]
	[Hammer.Skip]
	public partial class GamemodeEntity<T> : Entity where T : Entity {

		[Property("excludedgamemodes", Title = "Excluded Gamemodes"), FGDType("flags")]
		public Gamemodes ExcludedGamemodes { get; set; }

		public bool Enabled { get; private set; } = true;

		[Input]
		public async void Enable() {
			if(Enabled) return;

			OnEnabled();
			Enabled = true;
			await OnEntityEnabled.Fire(this);
		}

		protected Output OnEntityEnabled { get; set; }
		public virtual void OnEnabled() { }

		[Input]
		public async void Disable() {
			if(!Enabled) return;

			OnDisabled();
			Enabled = false;
			await OnEntityDisabled.Fire(this);
		}
		
		protected Output OnEntityDisabled { get; set; }
		public virtual void OnDisabled() { }

		[Flags] // THIS HAS TO LINE UP WITH THE ENUM IN GAMEMODE.CS
		public enum Gamemodes {
			Classic = 1,
			Koth = 2,
			Dodgeball = 4
		}
	}
}