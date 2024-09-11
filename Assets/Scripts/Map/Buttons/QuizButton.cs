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
		TurnOnSpinner(false);
		IsLocked = _isLocked;
	}

	public override void OnButtonClick()
	{
		// set backend to review packet and go to quiz
		Debug.Log($"set backend to review {reviewNumber} and go to quiz");
		TurnOnSpinner(true);

		GlobalManager.Instance.CurrentReview = reviewNumber;
		GlobalManager.Instance.CurrentPacket = packetIDDisplayed;
		mapManager.EnterQuiz();
	}

	private void OnValidate()
	{
		SetTooltipText($"Review {reviewNumber} Quiz", false);
	}
}
