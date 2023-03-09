using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using Vulf.GTTD.Game;
using Vulf.GTTD.Game.Input;
using Vulf.GTTD.Game.Player;

namespace Vulf.GTTD.Networking
{
	public enum ServerToClientId : ushort
	{
		PlayerJoined = 0,
		GameStarted = 1,
		PlayerStateToIndividual = 2,
		PlayerStateToAll = 3,
	}

	public enum ClientToServerId : ushort
	{
		ClientInfo = 0,
		Input = 1,
	}

	public class NetworkManager : MonoBehaviour
	{
		#region Properties
		public static NetworkManager Instance
		{
			get => instance;
			set
			{
				if (instance == null)
					instance = value;
				else if (instance != value)
				{
					Debug.Log($"An instance of type {nameof(NetworkManager)} was created but one already exists! Destroying duplicate");
					Destroy(value);
				}
			}
		}

		public Server Server { get; private set; }

		public int ServerTick { get; private set; }
		#endregion

		#region Fields
		static NetworkManager instance;

		const ushort port = 7777;
		const ushort maxClientCount = 4;
		#endregion

		#region Private Methods
		void Awake()
		{
			Instance = this;
		}

		void Start()
		{
			// Initialize the Riptide Logger
			RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, true);

			// Create the server
			Server = new Server();

			// Subscribe to server events
			Server.ClientDisconnected += OnClientDisconnected;
			GameManager.Instance.GameStarted += OnGameStarted;

			// Start the server
			Server.Start(port, maxClientCount);
		}

		void FixedUpdate()
		{
			ServerTick++;

			// Update the server every tick
			Server.Update();

			// Send player positions
			SendPlayerPositions();
		}

		void OnApplicationQuit()
		{
			// Stop the server when the application shuts down
			Server.Stop();
		}

		void SendPlayerPositions()
		{
			if (!GameManager.Instance.GameRunning)
				return;

			foreach (PlayerInfo playerInfo in GameManager.Instance.Lobby.PlayerList.Values)
			{
				// To individual
				Message toIndividual = Message.Create(MessageSendMode.Unreliable, ServerToClientId.PlayerStateToIndividual);
				toIndividual.AddUShort((ushort)playerInfo.PlayerController.Health);
				toIndividual.AddUShort((ushort)playerInfo.PlayerController.CurrentAmmo);
				Instance.Server.Send(toIndividual, playerInfo.Id);

				// To all
				Message toAll = Message.Create(MessageSendMode.Unreliable, ServerToClientId.PlayerStateToAll);
				toAll.AddUShort(playerInfo.Id);
				toAll.AddVector2(playerInfo.PlayerObject.transform.position);
				Instance.Server.SendToAll(toAll);
			}
		}
		#endregion

		#region Event Subscribers
		void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
		{
			// Remove the client's player from the lobby
			GameManager.Instance.Lobby.RemovePlayer(e.Client.Id);
		}

		void OnGameStarted()
		{
			// Notify clients of round start
			Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.GameStarted);
			Server.SendToAll(message);
		}
		#endregion

		#region Messages
		[MessageHandler((ushort)ClientToServerId.ClientInfo)]
		static void RegisterClient(ushort fromClientId, Message message)
		{
			// Create the player info
			PlayerInfo info = new PlayerInfo(fromClientId)
			{
				Username = message.GetString(),
			};

			// Add the player to the lobby
			bool successful = GameManager.Instance.Lobby.AddPlayer(info);

			// If the player could not be added, disconnect them
			if (!successful)
				Instance.Server.DisconnectClient(fromClientId);

			// Send this client to all clients
			message = Message.Create(MessageSendMode.Reliable, ServerToClientId.PlayerJoined);

			message.AddUShort(info.Id);
			message.AddString(info.Username);

			Instance.Server.SendToAll(message);

			// Send all clients to this client
			foreach (PlayerInfo playerInfo in GameManager.Instance.Lobby.PlayerList.Values)
			{
				// Skip self
				if (playerInfo.Id == fromClientId)
					continue;

				message = Message.Create(MessageSendMode.Reliable, ServerToClientId.PlayerJoined);
				message.AddUShort(playerInfo.Id);
				message.AddString(playerInfo.Username);
				Instance.Server.Send(message, fromClientId);
			}
		}

		[MessageHandler((ushort)ClientToServerId.Input)]
		static void HandleClientInput(ushort fromClientId, Message message)
		{
			if (!GameManager.Instance.GameRunning)
				return;

			InputSource clientInput = GameManager.Instance.Lobby.PlayerList[fromClientId].InputSource;
			clientInput.MovementAxis = message.GetVector2();
			clientInput.AimDirection = message.GetVector2();
			clientInput.FirePressed = message.GetBool();
			clientInput.ReloadPressed = message.GetBool();
		}
		#endregion
	}
}