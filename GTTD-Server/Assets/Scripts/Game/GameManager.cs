using System;
using UnityEngine;

namespace Vulf.GTTD.Game
{
	public class GameManager : MonoBehaviour
	{
		#region Properties
		public static GameManager Instance
		{
			get => instance;
			set
			{
				if (instance == null)
					instance = value;
				else if (instance != value)
				{
					Debug.Log($"An instance of type {nameof(GameManager)} was created but one already exists! Destroying duplicate");
					Destroy(value);
				}
			}
		}

		public Lobby Lobby { get; private set; }

		public bool GameRunning { get; private set; }

		public uint CurrentTick { get; private set; }
		#endregion

		#region Events
		public event Action GameStarted;
		#endregion

		#region Fields
		static GameManager instance;
		#endregion

		#region Private Methods
		void Awake()
		{
			Instance = this;
		}

		void Start()
		{
			// Initialize the lobby
			Lobby = new Lobby();
		}

		void FixedUpdate()
		{
			CurrentTick++;

			if (CurrentTick == 3 * 60)
				StartGame();
		}

		void StartGame()
		{
			GameRunning = true;
			GameStarted?.Invoke();

			// Create all player objects
			Lobby.CreatePlayerObjects();
		}
		#endregion
	}
}