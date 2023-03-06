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
		public static InputHandler Instance { get; private set; }
		private InputMap InputMap { get; set; }
		#endregion

		#region Private Methods
		void Awake()
		{
			// Initialize the singleton
			if (Instance != null && Instance != this)
			{
				Debug.Log($"An instance of type {typeof(InputHandler)} was created but one already exists. Destroying duplicate!");
				Destroy(this);
				return;
			}
			else
			{
				Instance = this;
			}

			// Create the input map and player input
			InputMap = new InputMap();
		}

		void FixedUpdate()
		{
			SendInputPacket();
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

		void SendInputPacket()
		{
			Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.input);

			Vector2 movementAxis = InputMap.Gameplay.MovementAxis.ReadValue<Vector2>();

			message.AddVector2(movementAxis);

			NetworkManager.Instance.Client.Send(message);
		}
		#endregion
	}
}