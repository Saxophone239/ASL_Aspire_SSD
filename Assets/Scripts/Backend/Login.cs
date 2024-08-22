using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using TMPro; 
public class Login : MonoBehaviour
{


    void Start(){
        DevLoginActivate();
    }
    public void DevLoginActivate() {
        var request = new LoginWithCustomIDRequest {
        CustomId = "Student1"
        };
        PlayFabClientAPI.LoginWithCustomID(request, DevOnLoginSuccess, OnError);
        
        Debug.Log("Login sent");

    }



     void DevOnLoginSuccess(LoginResult result) {
        Debug.Log("Login success!");
    }

    void OnError(PlayFabError error) {
         Debug.Log(error.ErrorMessage);
    }

}
