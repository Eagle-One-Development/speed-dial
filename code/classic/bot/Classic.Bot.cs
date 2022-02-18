﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Bot {
	public partial class ClassicBot : Sandbox.Bot {
		protected ClassicBotBehaviour behaviour;

		public override void BuildInput(InputBuilder builder) {
			builder.Clear();

			if (behaviour != null) {
				builder.InputDirection = behaviour.InputDirection;
				builder.ViewAngles = behaviour.ViewAngles;
				builder.SetButton(InputButton.Attack1, behaviour.Attack1);
				builder.SetButton(InputButton.Attack2, behaviour.Attack2);
			}
		}

		public virtual void ApplyBehaviour<T>() where T : ClassicBotBehaviour, new() {
			Log.Debug($"Bot behaviour applied | {typeof(T)}");
			behaviour = new T {
				Bot = this
			};
		}

		public override void Tick() {
			if (behaviour is null) {
				return;
			}

			behaviour.Tick();
		}
	}
}