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
		// set backend to review packet and go to quiz
		Debug.Log($"set backend to review {reviewNumber} and go to quiz");

		GlobalManager.Instance.CurrentReview = reviewNumber - 1;
		mapManager.EnterQuiz();
	}
}
