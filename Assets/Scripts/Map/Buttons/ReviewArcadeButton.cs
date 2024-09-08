using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewArcadeButton : MapButton
{
	[SerializeField] private MapManager mapManager;

	private void Start()
	{
		mapManager = FindObjectOfType<MapManager>();
		SetTooltipText($"Review {reviewNumber} Arcade", true);
		IsLocked = _isLocked;
	}

	public override void OnButtonClick()
	{
		// set backend to review packet and go to arcade
		Debug.Log($"set backend to review packet up to packet {packetIDDisplayed} and go to arcade");

		GlobalManager.Instance.CurrentPacket = packetIDDisplayed - 1;
		GlobalManager.Instance.ReviewPreviousPackets = true;

		mapManager.StartArcade();
	}
}
