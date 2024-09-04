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
    public bool GetLessonData(int packetID) {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result => OnLessonDataReceived(result, packetID),
            OnError);
        return true;
    }

    void OnLessonDataReceived(GetUserDataResult result, int packetID) {
        if (result.Data != null && result.Data.ContainsKey($"Lesson {packetID}")) {
            Debug.Log($"Received student lesson data for lesson {packetID}!");
            LessonData lessonData = JsonUtility.FromJson<LessonData>(result.Data[$"Lesson {packetID}"].Value); 
            Debug.Log(lessonData.packetID);
            GlobalManager.Instance.currentLessonData = lessonData;
        }
    }



    
    public bool GetReviewData(int reviewID) {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result => OnReviewDataReceived(result, reviewID),
            OnError);
        return true;
    }

    void OnReviewDataReceived(GetUserDataResult result, int reviewID) {
        if (result.Data != null && result.Data.ContainsKey($"Review {reviewID}")) {
            Debug.Log($"Received student review data for review {reviewID}!");
            ReviewData reviewData = JsonUtility.FromJson<ReviewData>(result.Data[$"Review {reviewID}"].Value); 
            Debug.Log(reviewData.reviewID);
            GlobalManager.Instance.currentReviewData = reviewData;
        }
    }

    void OnError(PlayFabError error) {
        Debug.Log(error);
    }
}
