using UnityEngine;

namespace Vulf.GTTD.Game.Input
{
	public class InputSource
	{
		public Vector2 MovementAxis { get; set; }
		public Vector2 AimDirection { get; set; }
		public bool FirePressed { get; set; }
		public bool ReloadPressed { get; set; }
	}
}