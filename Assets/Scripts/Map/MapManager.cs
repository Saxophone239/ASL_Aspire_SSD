using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private StateManager stateManager;
    // Start is called before the first frame update
    void Start()
    {
        stateManager = FindObjectOfType<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartArcade()
    {
        stateManager.ChangeState(MenuState.Arcade);
    }

    public void EnterFlashcards(int packetNum)
    {
        GlobalManager.Instance.CurrentPacket = packetNum;
        SceneManager.LoadScene("FlashcardScene");
    }
}
