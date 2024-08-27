using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;

public class LoginManager : MonoBehaviour
{
    [SerializeField] TMP_InputField idInput;
    [SerializeField] TextMeshProUGUI failText;
    [SerializeField] Button loginButton;

    private StateManager stateManager;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Make this failsafe
        stateManager = FindObjectOfType<StateManager>();
    }

    public void Login()
    {
		if (!idInput.text.Equals(string.Empty))
		{
			StudentLoginActivate(idInput.text);
		}
		else
		{
			LoginFail();
		}
    }

    private void LoginFail()
    {
        failText.gameObject.SetActive(true);
    }

	public void StudentLoginActivate(string customID)
	{
        var request = new LoginWithCustomIDRequest {
        	CustomId = customID
        };
        PlayFabClientAPI.LoginWithCustomID(request, StudentOnLoginSuccess, OnError);
        
        Debug.Log("Login sent");
    }


    void StudentOnLoginSuccess(LoginResult result) {
        Debug.Log("Login success!");
		stateManager.ChangeState(MenuState.Map);
    }

    void OnError(PlayFabError error) {
        Debug.Log(error.ErrorMessage);
    }
}
