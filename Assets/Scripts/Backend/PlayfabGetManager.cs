using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json; 
public class PlayfabGetManager : MonoBehaviour
{
	public static PlayfabGetManager Instance;
    // public PlayfabPostManager postManager;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

    public bool GetLessonData(int packetID)
	{
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result => OnLessonDataReceived(result, packetID),
            OnError);
        return true;
    }

	public IEnumerator GetLessonDataCoroutine(int packetID)
	{
		bool isCompleted = false;
		PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
			result =>
			{
				OnLessonDataReceived(result, packetID);
				isCompleted = true;
			},
			error =>
			{
				OnError(error);
				isCompleted = true;
			}
		);

		yield return new WaitUntil(() => isCompleted);
	}

    void OnLessonDataReceived(GetUserDataResult result, int packetID)
	{
        if (result.Data != null && result.Data.ContainsKey($"Lesson {packetID}"))
		{
            Debug.Log($"Received student lesson data for lesson {packetID}!");
            // LessonData lessonData = JsonUtility.FromJson<LessonData>(result.Data[$"Lesson {packetID}"].Value);
			LessonData lessonData = JsonConvert.DeserializeObject<LessonData>(result.Data[$"Lesson {packetID}"].Value);
            Debug.Log(lessonData.packetID);
            GlobalManager.Instance.currentLessonData = lessonData;
        }
    }

    public bool GetFirstTimeEntrance()
	{
		PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
			result => OnLessonDataReceived(result),
			OnError);
		return true;
    }

	public IEnumerator GetFirstTimeEntranceCoroutine()
	{
		bool isCompleted = false;
		PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
			result =>
			{
				OnLessonDataReceived(result);
				isCompleted = true;
			},
			error =>
			{
				OnError(error);
				isCompleted = true;
			}
		);

		yield return new WaitUntil(() => isCompleted);
	}

	void OnLessonDataReceived(GetUserDataResult result)
	{
		if (result.Data != null && result.Data.ContainsKey($"FirstTimeEntrance"))
		{
			Debug.Log($"Student has entered before!");
			GlobalManager.Instance.firstTimeEntrance = false;
		}
		else
		{
			Debug.Log($"Student has not entered before, this is their first time");
			GlobalManager.Instance.firstTimeEntrance = true;
			PlayfabPostManager.Instance.PostFirstTimeEntrance();
		}
	}

    public bool GetReviewData(int reviewID) {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result => OnReviewDataReceived(result, reviewID),
            OnError);
        return true;
    }

	public IEnumerator GetReviewDataCoroutine(int reviewID)
	{
		bool isCompleted = false;
		PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
			result =>
			{
				OnReviewDataReceived(result, reviewID);
				isCompleted = true;
			},
			error =>
			{
				OnError(error);
				isCompleted = true;
			}
		);

		yield return new WaitUntil(() => isCompleted);
	}

    void OnReviewDataReceived(GetUserDataResult result, int reviewID)
	{
        if (result.Data != null && result.Data.ContainsKey($"Review {reviewID}"))
		{
            Debug.Log($"Received student review data for review {reviewID}!");
            // ReviewData reviewData = JsonUtility.FromJson<ReviewData>(result.Data[$"Review {reviewID}"].Value); 
			ReviewData reviewData = JsonConvert.DeserializeObject<ReviewData>(result.Data[$"Review {reviewID}"].Value); 
            Debug.Log(reviewData.reviewID);
            GlobalManager.Instance.currentReviewData = reviewData;
        }
    }

    void OnError(PlayFabError error)
	{
		Debug.LogError($"Playfab Get error {error.GenerateErrorReport()}");
    }
}
