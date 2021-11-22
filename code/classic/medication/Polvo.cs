﻿using Sandbox;

namespace SpeedDial.Classic.Meds {
	[Library("meds_polvo")]
	[Hammer.EditorModel("models/drugs/polvo/polvo.vmdl")]
	[Hammer.EntityTool("Polvo", "Speed Dial Drugs", "Polvo spawn point")]
	public class Polvo : BaseMedication {
		public override string WorldModel => "models/drugs/polvo/polvo.vmdl";
		public override float RotationSpeed => 75f;
		public override string DrugName => "POLVO";
		public override string DrugFlavor => "you are speed"; // legs so fast // gotta move // dashing!
		public override float DrugDuration => 4f;
		public override DrugType Drug => DrugType.Polvo;
		public override string icon => "materials/ui/polvo.png";
		public override string PickupSound => "sd_polvo_take";
		public override string ParticleName => "particles/drug_fx/sd_polvo/sd_polvo.vpcf";
	}
}
