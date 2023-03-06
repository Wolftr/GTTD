using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vulf.GTTD.Networking
{
	public enum ServerToClientId : ushort
	{
		spawnPlayer,
		playerPosition,
	}

	public enum ClientToServerId : ushort
	{
		clientInfo,
		input,
	}

	public class NetworkManager : MonoBehaviour
	{
		#region Properties
		// Singleton
		public static NetworkManager Instance 
		{
			get => instance;
			private set
			{
				if (instance != null)
				{
					Debug.Log($"An instance of type {typeof(NetworkManager)} was created but one already exists! Destroying duplicate...");
					Destroy(value);
				}
				else if (Instance != value)
				{
					instance = value;
				}
			}
		}

		// Client
		public Client Client { get; private set; }
		public Dictionary<ushort, Player> PlayerList { get; private set; }
		#endregion

		#region Settings
		[SerializeField] string ip;
		[SerializeField] ushort port;
		#endregion

		#region Fields
		// Singleton
		private static NetworkManager instance;
		#endregion

		#region Private Methods
		void Awake()
		{
			// Set the singleton
			Instance = this;
		}

		void Start()
		{
			// Initialize the Riptide Logger
			RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, true);

			// Create the client
			Client = new Client();
			PlayerList = new Dictionary<ushort, Player>();

			// Subscribe client events
			Client.Connected += OnConnected;
			Client.Disconnected += OnDisconnect;
			Client.ConnectionFailed += OnConnectionFailed;

			// Connect the client to the server
			Client.Connect($"{ip}:{port}");
		}

		void FixedUpdate()
		{
			// Update the client
			Client.Update();

			// Send the client's input packet
		}

		void OnApplicationQuit()
		{
			Client.Disconnect();
		}

		void OnConnected(object sender, EventArgs e)
		{
			// Send the player info to the server
			SendPlayerInfo();
		}

		void OnDisconnect(object sender, DisconnectedEventArgs e)
		{
			
		}

		void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
		{
			
		}

		// Methods
		void SendPlayerInfo()
		{
			// Create the player info message
			Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.clientInfo);

			// Add the player's info to the message
			message.AddString("Username");

			// Send the message
			Client.Send(message);
		}

		[MessageHandler((ushort)ServerToClientId.spawnPlayer)]
		static void SpawnPlayer(Message message)
		{
			ushort playerId = message.GetUShort();
			
			
		}
		#endregion
	}
}