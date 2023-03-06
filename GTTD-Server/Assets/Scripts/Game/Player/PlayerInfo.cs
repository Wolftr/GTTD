using UnityEngine;
using Vulf.GTTD.Game.Input;

namespace Vulf.GTTD.Game.Player
{
	public class PlayerInfo
	{
		public ushort Id { get; private set; }
		public string Username { get; set; }

		public GameObject PlayerObject { get; set; }
		public PlayerController PlayerController { get; set; }
		public InputSource InputSource => PlayerController.Input;

		public PlayerInfo(ushort id)
		{
			Id = id;
		}
	}
}
