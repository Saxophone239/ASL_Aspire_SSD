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
		TurnOnSpinner(false);
		IsLocked = _isLocked;
	}

	public override void OnButtonClick()
	{
		// set backend to lesson packet and go to flashcards
		Debug.Log($"set backend to lesson packet up to packet {packetIDDisplayed} and go to flashcards");
		TurnOnSpinner(true);

		GlobalManager.Instance.CurrentPacket = packetIDDisplayed;
		mapManager.EnterFlashcards();
	}

	private void OnValidate()
	{
		SetTooltipText($"Packet {packetIDDisplayed} Flashcards", false);
	}
}
