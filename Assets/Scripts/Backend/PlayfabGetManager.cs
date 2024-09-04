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
   

//    public bool GetStudentData(){

//    }

   
    // public bool GetCoinsData(){
    //     PlayFabClientAPI.GetUserData(new GetUserDataRequest(),OnCoinDataReceived,OnError);
    //     return true;

    // }

    
    // void OnStudentDataReceived(GetUserDataResult result){
    //     if(result.Data !=null && result.Data.ContainsKey("StudentData")){
    //         Debug.Log("Received student data!");
    //         GlobalManager.student.coins = int.Parse(result.Data["Coins"].Value);

    //     }

    // }

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
    }
}

void OnError(PlayFabError error) {
    Debug.Log(error);
}
}
