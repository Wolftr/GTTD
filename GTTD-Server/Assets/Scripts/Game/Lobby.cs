using System.Collections.Generic;
using UnityEngine;
using Vulf.GTTD.Game.Player;

namespace Vulf.GTTD.Game
{
	public class Lobby
	{
		public Dictionary<ushort, PlayerInfo> PlayerList { get; private set; }

		public Lobby()
		{
			PlayerList = new Dictionary<ushort, PlayerInfo>();
		}
	}
}