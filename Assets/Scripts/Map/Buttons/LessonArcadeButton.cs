using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessonArcadeButton : MapButton
{
	[SerializeField] private MapManager mapManager;

	private void Start()
	{
		mapManager = FindObjectOfType<MapManager>();
		SetTooltipText($"Packet {packetIDDisplayed} Arcade", true);
		IsLocked = _isLocked;
	}

	public override void OnButtonClick()
	{
		// set backend to lesson packet and go to arcade
		Debug.Log($"set backend to lesson packet up to packet {packetIDDisplayed} and go to arcade");

		GlobalManager.Instance.CurrentPacket = packetIDDisplayed - 1;
		GlobalManager.Instance.ReviewPreviousPackets = false;

		mapManager.StartArcade();
	}
}
