using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
	[SerializeField] private MapDrawer mapDrawer;
	[SerializeField] private GameObject arcadePanel;
	[SerializeField] private GameObject loadingMapPanel;
	[SerializeField] private GameObject displayCoinsPanel;

    // private StateManager stateManager;
    
    private void Start()
    {
        // stateManager = FindObjectOfType<StateManager>();
		arcadePanel.SetActive(false);
		loadingMapPanel.SetActive(true);
		if (GlobalManager.Instance.DisplayCoinsCollected)
		{
			displayCoinsPanel.SetActive(true);
		}
		else
		{
			displayCoinsPanel.SetActive(false);
		}
		StartCoroutine(InitializeMapData());
    }

	public IEnumerator InitializeMapData()
	{
		loadingMapPanel.SetActive(true);

		Debug.Log("starting loading of map data");
		// int numberOfIcons = 30;
		// bool[] isIconLocked = new bool[30];
		List<bool> isIconLocked = new List<bool>();

		// Get player data
		GetUserDataResult playerData = null;
		bool isPlayerDataLoaded = false;

		PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
			result =>
			{
				playerData = result;
				isPlayerDataLoaded = true;
			},
			error =>
			{
				Debug.LogError($"Error retrieving player data: {error.GenerateErrorReport()}");
				isPlayerDataLoaded = true;
			}
		);

		yield return new WaitUntil(() => isPlayerDataLoaded);

		// Check if player data is not null
		if (playerData.Data == null)
		{
			Debug.LogError("Player lesson and review data is empty!");
			yield break;
		}

		// Look through first 3 lessons
		for (int i = 0; i < 3; i++)
		{
			MatchDataToIcons(ref isIconLocked, playerData, false, i);
		}

		// Look through review 1
		MatchDataToIcons(ref isIconLocked, playerData, true, 0);

		// Look through next 3 lessons
		for (int i = 3; i < 6; i++)
		{
			MatchDataToIcons(ref isIconLocked, playerData, false, i);
		}
		

		// Look through review 2
		MatchDataToIcons(ref isIconLocked, playerData, true, 1);
		

		// Look through next 3 lessons
		for (int i = 6; i < 9; i++)
		{
			MatchDataToIcons(ref isIconLocked, playerData, false, i);
		}
		

		// Look through review 3
		MatchDataToIcons(ref isIconLocked, playerData, true, 2);
		

		// Look through next 2 lessons
		for (int i = 9; i < 11; i++)
		{
			MatchDataToIcons(ref isIconLocked, playerData, false, i);
		}
		

		// Look through review 4
		MatchDataToIcons(ref isIconLocked, playerData, true, 3);

		Debug.Log("finished loading of map data");
		GlobalManager.Instance.MapIconIsLockedStatus = isIconLocked;
		mapDrawer.DrawIconLockedStatus();

		loadingMapPanel.SetActive(false);
	}

	private void MatchDataToIcons(ref List<bool> isIconLockedList, in GetUserDataResult playerData, bool isReview, int id)
	{
		bool isLessonUnlocked = false;
		bool isFlashcardsComplete = false;

		if (!isReview)
		{
			// We are a lesson
			LessonData lessonData;
			if (playerData.Data.ContainsKey($"Lesson {id}"))
			{
				lessonData = JsonUtility.FromJson<LessonData>(playerData.Data[$"Lesson {id}"].Value);
			}
			else
			{
				Debug.LogWarning($"Lesson {id} does not exist, creating a fresh new one");
				lessonData = new LessonData();
			}
			isLessonUnlocked = lessonData.isUnlocked;
			isFlashcardsComplete = lessonData.flashcardsComplete;
		}
		else
		{
			// We are a review
			ReviewData reviewData;
			if (playerData.Data.ContainsKey($"Review {id}"))
			{
				reviewData = JsonUtility.FromJson<ReviewData>(playerData.Data[$"Review {id}"].Value);
			}
			else
			{
				Debug.LogWarning($"Review {id} does not exist, creating a fresh new one");
				reviewData = new ReviewData();
			}
			isLessonUnlocked = reviewData.isUnlocked;
			isFlashcardsComplete = reviewData.flashcardsComplete;
		}

		if (isLessonUnlocked)
		{
			isIconLockedList.Add(false);
			if (isFlashcardsComplete)
			{
				isIconLockedList.Add(false);
			}
			else
			{
				isIconLockedList.Add(true);
			}
		}
		else
		{
			isIconLockedList.Add(true);
			isIconLockedList.Add(true);
		}
	}

    public void StartArcade()
    {
        // stateManager.ChangeState(MenuState.Arcade);
		arcadePanel.SetActive(true);
    }

    // public void EnterFlashcards(int packetNum)
    // {
    //     GlobalManager.Instance.CurrentPacket = packetNum;
    //     SceneManager.LoadScene("FlashcardScene");
    // }

	public void EnterFlashcards()
    {
        SceneManager.LoadScene("FlashcardScene");
    }
}
