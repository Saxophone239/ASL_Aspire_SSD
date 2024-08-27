using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        if (idInput.text == "user1")
        {
            LoginSuccessful();
        } else
        {
            LoginFail();
        }
    }

    private void LoginSuccessful()
    {
        stateManager.ChangeState(MenuState.Map);
    }

    private void LoginFail()
    {
        failText.gameObject.SetActive(true);
    }
}
