using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Vulf.GTTD.Game;
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
		public Vector2 AimDirection
		{
			get
			{
				Vector2 aimAxis = InputMap.Gameplay.AimAxis.ReadValue<Vector2>();

				// Ensure the aim axis is never zero
				if (aimAxis == Vector2.zero)
					aimAxis = oldAimAxis;
				else
					oldAimAxis = aimAxis;


				Vector2 aimDirection;
				if (OnController)
					aimDirection = aimAxis;
				else
				{
					Vector2 pos1 = Camera.main.ScreenToWorldPoint(InputMap.Gameplay.MousePosition.ReadValue<Vector2>());
					Vector2 pos2 = GameManager.Instance.Lobby.LocalPlayer.PlayerObject.transform.position;
					aimDirection = pos1 - pos2;
				}

				return aimDirection.normalized;
			}
		}
		public bool FirePressed => InputMap.Gameplay.Fire.IsPressed();
		public bool ReloadPressed => InputMap.Gameplay.Reload.IsPressed();

		public bool OnController { get; private set; }
		#endregion

		#region Fields
		static InputHandler instance;

		Vector2 oldAimAxis = Vector2.right;
		#endregion

		#region Private Methods
		void Awake()
		{
			Instance = this;

			// Create the input map and player input
			InputMap = new InputMap();

			// Subscribe OnActionPerformed to all actions
			InputMap.Gameplay.MovementAxis.performed += ctx => OnActionPerformed(ctx);
			InputMap.Gameplay.AimAxis.performed += ctx => OnActionPerformed(ctx);
			InputMap.Gameplay.Fire.performed += ctx => OnActionPerformed(ctx);
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

		void OnActionPerformed(InputAction.CallbackContext ctx)
		{
			InputDevice device = ctx.control.device;
			OnController = device is Gamepad;
		}
		#endregion
	}
}