using UnityEngine;
using TMPro;
using Vulf.GTTD.Game;
using Vulf.GTTD.Game.Player;

namespace Vulf.GTTD.UI
{
	public class HUDBehavior : MonoBehaviour
	{
		[SerializeField] TMP_Text hpDisplay;
		[SerializeField] TMP_Text ammoCounter;

		void FixedUpdate()
		{
			UpdateDisplay();
		}

		public void UpdateDisplay()
		{
			if (GameManager.Instance.Lobby.PlayerList.Count == 0)
				return;

			PlayerInfo info = GameManager.Instance.Lobby.LocalPlayer;

			hpDisplay.text = $"HP: {info.HP}/150";
			ammoCounter.text = $"Ammo: {info.CurrentAmmo}/20";
		}
	}
}