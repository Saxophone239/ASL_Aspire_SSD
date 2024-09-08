using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashcardButton : MapButton
{
	[SerializeField] private MapManager mapManager;

	private void Start()
	{
		mapManager = FindObjectOfType<MapManager>();
		SetTooltipText($"Packet {packetIDDisplayed} Flashcards", true);
		IsLocked = _isLocked;
	}

	public override void OnButtonClick()
	{
		// set backend to lesson packet and go to flashcards
		Debug.Log($"set backend to lesson packet up to packet {packetIDDisplayed} and go to flashcards");

		GlobalManager.Instance.CurrentPacket = packetIDDisplayed - 1;
		mapManager.EnterFlashcards();
	}
}
