using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json; 

public class PlayfabPostManager : MonoBehaviour
{



	void OnError(PlayFabError error){
        Debug.Log(error);
    }

     void OnLessonDataSend(UpdateUserDataResult result){
        Debug.Log("Successful lesson user data sent!");
    }


	public void PostLesson(LessonData lessonData)
	{
		var request = new UpdateUserDataRequest{
            Data = new Dictionary<string,string>{
                {$"Lesson {lessonData.packetID}",JsonConvert.SerializeObject(lessonData)}
            }
        
        };
        PlayFabClientAPI.UpdateUserData(request,OnLessonDataSend,OnError);

	}



	// TODO: build lesson skeleton

	public void PostQuiz(Dictionary<int, int> dictionary)
	{

	}

	public void PostReviewGames(Dictionary<int, List<bool>> dictionary)
	{

	}
}
