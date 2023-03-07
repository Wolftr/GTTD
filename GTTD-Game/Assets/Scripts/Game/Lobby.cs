using System.Collections.Generic;
using UnityEngine;
using Vulf.GTTD.Game.Player;

namespace Vulf.GTTD.Game
{
	public class Lobby
	{
		#region Properties
		public Dictionary<ushort, PlayerInfo> PlayerList { get; private set; }
		#endregion

		#region Fields
		GameObject playerPrefab;
		GameObject localPlayerPrefab;
		#endregion

		#region Constructor
		public Lobby()
		{
			// Initialize the player list
			PlayerList = new Dictionary<ushort, PlayerInfo>();

			// Load the player prefab from resources
			playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
			localPlayerPrefab = Resources.Load<GameObject>("Prefabs/Player/Local Player");
		}
		#endregion

		#region Public Methods
		public bool AddPlayer(PlayerInfo info)
		{
			return PlayerList.TryAdd(info.Id, info);
		}

		public void RemovePlayer(ushort id)
		{
			// Destroy the player object if it exists
			if (PlayerList[id].PlayerObject != null)
				Object.Destroy(PlayerList[id].PlayerObject);

			// Remove the player from the list
			PlayerList.Remove(id);
		}

		public void CreatePlayerObject(ushort id)
		{
			if (PlayerList[id].IsLocal)
				PlayerList[id].PlayerObject = Object.Instantiate(localPlayerPrefab);
			else
				PlayerList[id].PlayerObject = Object.Instantiate(playerPrefab);

			PlayerList[id].PlayerObject.name = $"{PlayerList[id].Username} ({id})";
		}

		public void CreatePlayerObjects()
		{
			foreach (KeyValuePair<ushort, PlayerInfo> player in PlayerList)
				CreatePlayerObject(player.Key);
		}
		#endregion
	}
}