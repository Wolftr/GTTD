using UnityEngine;

namespace Vulf.GTTD.Game.Player
{
	public class PlayerInfo
	{
		public ushort Id { get; private set; }
		public string Username { get; set; }
		public bool IsLocal { get; set; }

		// States
		public int HP { get; set; }
		public int CurrentAmmo { get; set; }

		public GameObject PlayerObject { get; set; }

		public PlayerInfo(ushort id)
		{
			Id = id;
		}
	}
}
