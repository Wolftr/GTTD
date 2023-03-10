using Riptide;
using UnityEngine;
using Vulf.GTTD.Input;

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

		public bool GameStarted { get; private set; }
		#endregion

		#region Field
		static GameManager instance;
		#endregion

		#region Private Methods
		void Awake()
		{
			Instance = this;
		}

		void Start()
		{
			Lobby = new Lobby();
		}

		void FixedUpdate()
		{
			
		}
		#endregion

		#region Public Methods
		public void StartRound()
		{
			GameStarted = true;
			Lobby.CreatePlayerObjects();
		}
		#endregion
	}
}