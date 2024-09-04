using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json; 

public class PlayfabPostManager : MonoBehaviour
{

	public void PostLesson(LessonData lessonData)
	{
		var request = new UpdateUserDataRequest{
            Data = new Dictionary<string,string>{
                {$"Lesson {lessonData.packetID}",JsonConvert.SerializeObject(lessonData)}
            }
        
        };
        PlayFabClientAPI.UpdateUserData(request,OnLessonDataSend,OnError);

	}


     void OnLessonDataSend(UpdateUserDataResult result){
        Debug.Log("Successful lesson user data sent!");
    }


	public void PostReview(ReviewData reviewData){
		var request = new UpdateUserDataRequest{
        Data = new Dictionary<string,string>{
                {$"Review {reviewData.reviewID}",JsonConvert.SerializeObject(reviewData)}
            }
        
        };
        PlayFabClientAPI.UpdateUserData(request,OnReviewDataSend,OnError);
	}

	
     void OnReviewDataSend(UpdateUserDataResult result){
        Debug.Log("Successful review user data sent!");
    }

	
	void OnError(PlayFabError error){
        Debug.Log(error);
    }
}
