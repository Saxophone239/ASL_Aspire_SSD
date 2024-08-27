using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMechanics : MonoBehaviour
{
    [SerializeField] private StreetSignsUIManager uiManager;

    public int Score = 0;
    public int Lives = 5;
    public bool IsGameOver;
    public bool IsMainMenu = true;

    private int scoreCorrection;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        uiManager.UpdateLives(Lives);
        uiManager.UpdateScore(Score);

        VideoManager.GenerateVocabListFromSelectedVocabSet();
    }

    // Update is called once per frame
    void Update()
    {
        uiManager.UpdateScore(Score);

        if (IsGameOver)
        {
            PlayerController player = GameObject.FindObjectOfType<PlayerController>();
            StartCoroutine(player.StartDeathCoroutine());
        }
    }

    public void LoseLife()
    {
        Lives--;
        if (Lives <= 0)
        {
            IsGameOver = true;
        }
        uiManager.UpdateLives(Lives);
    }

    public void AddLife()
    {
        Lives++;
        uiManager.UpdateLives(Lives);
    }

    // When the player hits "Play Game," because the score tracks the player's z axis and the player
    // already has a value > 0, we record this value such that when the player starts the game,
    // the score starts at 0 and not that weird z axis value.
    public void SetScoreCorrection(int correction)
    {
        scoreCorrection = correction;
    }

    public void UpdateScore(int xAxisScore)
    {
        // Increase score (based on z position)
        Score = xAxisScore - scoreCorrection;
    }

    public void ShowGameOverScreen()
    {
        Time.timeScale = 0.0f;
        uiManager.ShowGameOverScreen();
    }
}
