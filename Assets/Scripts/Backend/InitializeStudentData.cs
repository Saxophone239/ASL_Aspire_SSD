using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeStudentData : MonoBehaviour
{
    // public DataModels dataModels;
    // public PlayfabPostManager postManager;
    
	private void Start()
	{
		// Check if this is the player's first time logging in
		StartCoroutine(GetFirstTimeEntranceCoroutine());
	}

	public IEnumerator GetFirstTimeEntranceCoroutine()
	{
		// Check if it's player's first time
		bool isCompleted = false;
		PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
			result =>
			{
				StartCoroutine(OnLessonDataReceived(result, () => isCompleted = true));
			},
			error =>
			{
				Debug.LogError($"Playfab Get error {error.GenerateErrorReport()}");
				isCompleted = true;
			}
		);

		yield return new WaitUntil(() => isCompleted);

		// Change to next scene
		StartCoroutine(LoadYourSceneAsync("MapLayoutScene"));
	}

	private IEnumerator OnLessonDataReceived(GetUserDataResult result, Action onComplete)
	{
		if (result.Data != null && result.Data.ContainsKey($"FirstTimeEntrance"))
		{
			// This is not the student's first time logging in, no need to post new data
			Debug.Log($"Student has entered before!");
			GlobalManager.Instance.firstTimeEntrance = false;
		}
		else
		{
			// This is the student's first time logging in, let's post new data
			Debug.Log($"Student has not entered before, this is their first time (or data is null)");
			GlobalManager.Instance.firstTimeEntrance = true;
			yield return StartCoroutine(InitializeAllStudentData());
			yield return StartCoroutine(PlayfabPostManager.Instance.PostFirstTimeEntranceCoroutine());
		}

		onComplete?.Invoke();
	}

    public IEnumerator InitializeAllStudentData()
	{
        // Initialize Reviews
        // int[] review0Packets = {1,2,3};
        // int[] review1Packets = {4,5,6};
        // int[] review2Packets = {7,8,9};
        // int[] review3Packets = {10,11};
		int[] review0Packets = {0,1,2};
        int[] review1Packets = {3,4,5};
        int[] review2Packets = {6,7,8};
        int[] review3Packets = {9,10};

        List<int[]> reviewSetupList = new List<int[]>();
        reviewSetupList.Add(review0Packets);
        reviewSetupList.Add(review1Packets);
        reviewSetupList.Add(review2Packets);
        reviewSetupList.Add(review3Packets);

        int reviewIndex = 0; 
        foreach (int[] reviewPacketsList in reviewSetupList)
		{
            ReviewData reviewData = DataModels.Instance.InitializeReviewFromVocabulary(reviewPacketsList);
            reviewData.reviewID = reviewIndex;
            reviewIndex += 1; 
            yield return StartCoroutine(PlayfabPostManager.Instance.PostReviewCoroutine(reviewData));
        }


        // Initialize Lessons
        for (int i = 0; i < 11; i++)
		{
            LessonData lessonData = DataModels.Instance.InitializeLessonFromVocabulary(i);
            lessonData.packetID = i;
            yield return StartCoroutine(PlayfabPostManager.Instance.PostLessonCoroutine(lessonData));
        }
    }

	private IEnumerator LoadYourSceneAsync(string sceneName)
	{
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
	}
}
