using Riptide;
using Riptide.Utils;
using System;
using UnityEngine;
using Vulf.GTTD.Game;
using Vulf.GTTD.Game.Player;
using Vulf.GTTD.Input;

namespace Vulf.GTTD.Networking
{
	public enum ServerToClientId : ushort
	{
		PlayerJoined = 0,
		RoundStarted = 1,
		PlayerPosition = 2,
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

		public Client Client { get; private set; }
		#endregion

		#region Settings
		[SerializeField] string ip;
		[SerializeField] ushort port;
		#endregion

		#region Fields
		static NetworkManager instance;
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

			// Create the client
			Client = new Client();

			// Subscribe to client events
			Client.Connected += OnConnected;
			Client.Disconnected += OnDisconnected;
			Client.ConnectionFailed += OnConnectionFailed;
			Client.ClientDisconnected += OnClientDisconnected;

			// Connect the client to the specified ip and port
			Client.Connect($"{ip}:{port}");
		}

		void FixedUpdate()
		{
			Client.Update();
			SendInputPacket();
		}

		void OnApplicationQuit()
		{
			Client.Disconnect();
		}

		void OnConnected(object sender, EventArgs e)
		{
			// Send client info to the server
			Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.ClientInfo);
			message.AddString("Username");
			Client.Send(message);
		}

		void OnDisconnected(object sender, DisconnectedEventArgs e)
		{
			
		}

		void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
		{
			
		}

		void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
		{
			GameManager.Instance.Lobby.RemovePlayer(e.Id);
		}

		void SendInputPacket()
		{
			Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.Input);
			message.AddVector2(InputHandler.Instance.MovementAxis);
			Client.Send(message);
		}
		#endregion

		#region Message Handlers
		[MessageHandler((ushort)ServerToClientId.PlayerJoined)]
		static void HandlePlayerJoined(Message message)
		{
			ushort id = message.GetUShort();

			PlayerInfo playerInfo = new PlayerInfo(id)
			{
				Username = message.GetString(),
				IsLocal = (id == Instance.Client.Id),
			};

			GameManager.Instance.Lobby.AddPlayer(playerInfo);
		}

		[MessageHandler((ushort)ServerToClientId.RoundStarted)]
		static void HandleRoundStart(Message message)
		{
			GameManager.Instance.StartRound();
		}

		[MessageHandler((ushort)ServerToClientId.PlayerPosition)]
		static void UpdatePlayerPosition(Message message)
		{
			ushort id = message.GetUShort();

			GameManager.Instance.Lobby.PlayerList[id].PlayerObject.transform.position = message.GetVector2();
		}
		#endregion
	}
}