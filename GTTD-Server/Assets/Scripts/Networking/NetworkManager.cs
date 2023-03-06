using Riptide;
using Riptide.Utils;
using System.Collections.Generic;
using UnityEngine;
using Vulf.GTTD.Game;
using Vulf.GTTD.Game.Input;
using Vulf.GTTD.Game.Player;

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

		public Server Server { get; private set; }
		public Lobby Lobby { get; private set; }
		#endregion

		#region Settings
		[SerializeField] GameObject playerPrefab;
		#endregion

		#region Fields
		// Singleton
		private static NetworkManager instance;

		// Server info
		const ushort port = 7777;
		const ushort maxClientCount = 4;
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

			// Create the server
			Server = new Server();

			// Create the lobby
			Lobby = new Lobby();

			// Subscribe server events
			Server.ClientConnected += OnClientConnected;
			Server.ClientDisconnected += OnClientDisconnected;

			// Start the server
			Server.Start(port, maxClientCount);
		}

		void FixedUpdate()
		{
			Server.Update();

			// Update player positions
			foreach (KeyValuePair<ushort, PlayerInfo> player in Lobby.PlayerList)
			{
				Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.playerPosition);
				message.AddUShort(player.Key);
				message.AddVector2(player.Value.PlayerObject.transform.position);
				Server.SendToAll(message);
			}
		}

		void OnApplicationQuit()
		{
			Server.Stop();
		}

		void OnClientConnected(object sender, ServerConnectedEventArgs e)
		{
			
		}

		void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
		{
			// Destroy the client's game object
			Destroy(Lobby.PlayerList[e.Client.Id].PlayerObject);

			// Remove the client from the players list
			Lobby.PlayerList.Remove(e.Client.Id);
		}

		[MessageHandler((ushort)ClientToServerId.clientInfo)]
		static void RegisterPlayer(ushort fromClientId, Message message)
		{
			GameObject playerObject = Instantiate(Instance.playerPrefab, Vector2.zero, Quaternion.identity);

			// Create a new player
			PlayerInfo playerInfo = new PlayerInfo(fromClientId)
			{
				Username = message.GetString(),
				PlayerObject = playerObject,
				PlayerController = playerObject.GetComponent<PlayerController>(),
			};

			// Name the player object
			playerInfo.PlayerObject.name = $"{playerInfo.Username} ({fromClientId})";

			// Add the player to the player dictionary
			Instance.Lobby.PlayerList.Add(fromClientId, playerInfo);
		}

		[MessageHandler((ushort)ClientToServerId.input)]
		static void SetClientInput(ushort fromClientId, Message message)
		{
			InputSource input = Instance.Lobby.PlayerList[fromClientId].InputSource;
			input.MovementAxis = message.GetVector2().normalized;
		}
		#endregion
	}
}