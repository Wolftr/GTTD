using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vulf.GTTD.Networking;

namespace Vulf.GTTD.Input
{
	public class InputHandler : MonoBehaviour
	{
		#region Properties
		public static InputHandler Instance
		{
			get => instance;
			set
			{
				if (instance == null)
					instance = value;
				else if (instance != value)
				{
					Debug.Log($"An instance of type {nameof(InputHandler)} was created but one already exists! Destroying duplicate");
					Destroy(value);
				}
			}
		}

		// Map
		private InputMap InputMap { get; set; }

		// Input
		public Vector2 MovementAxis => InputMap.Gameplay.MovementAxis.ReadValue<Vector2>();
		#endregion

		#region Fields
		static InputHandler instance;
		#endregion

		#region Private Methods
		void Awake()
		{
			Instance = this;

			// Create the input map and player input
			InputMap = new InputMap();
		}

		void OnEnable()
		{
			// Enable the input map
			InputMap.Enable();
		}

		void OnDisable()
		{
			// Disable the input map
			InputMap.Disable();
		}
		#endregion
	}
}