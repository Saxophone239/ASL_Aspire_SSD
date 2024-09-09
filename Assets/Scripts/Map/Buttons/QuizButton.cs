using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizButton : MapButton
{
	[SerializeField] private MapManager mapManager;

	private void Start()
	{
		mapManager = FindObjectOfType<MapManager>();
		SetTooltipText($"Review {reviewNumber} Quiz", true);
		IsLocked = _isLocked;
	}

	public override void OnButtonClick()
	{
		// TODO: set backend to review packet and go to quiz
		Debug.Log($"TODO: set backend to review packet up to packet {packetIDDisplayed} and go to quiz");
	}
}
