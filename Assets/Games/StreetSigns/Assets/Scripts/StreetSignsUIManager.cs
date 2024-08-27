using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class StreetSignsUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private PlayerLivesBar livesBar;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameMechanics gameMechanics; 

	[Header("Question UI Panel")]
	[SerializeField] private GameObject questionUIPanel;
	[SerializeField] private Image questionUIImage;
	[SerializeField] private VideoPlayer questionUIVideoPlayer;
	[SerializeField] private TextMeshProUGUI questionUIText;

    // Start is called before the first frame update
    void Start()
    {
        gameOverPanel.SetActive(false);
        livesText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLives(int lives)
    {
        //livesText.text = $"Lives: {lives}";
        livesBar.UpdateBar(lives);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void ShowGameOverScreen()
    {
        gameOverPanel.SetActive(true);

        GameMechanics gM = GameObject.FindObjectOfType<GameMechanics>();
        int score = gM.Score;
        if (score > 0 && score <= 1000) gameOverScoreText.text = $"You got a score of {score}, not bad";
        if (score > 1000 && score <= 2000) gameOverScoreText.text = $"Hey, you got a score of {score}, that's pretty good";
        if (score > 2000 && score <= 3500) gameOverScoreText.text = $"With a score of {score}, you could run a marathon!";
        if (score > 3500) gameOverScoreText.text = $"Ooo we've got an ASL expert with that score of {score}!";
    }

    public void OnRestartButtonClick()
    {
        UpdateGlobalCoins(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowGameplayDisplay()
    {
        livesText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
    }

    public void UpdateGlobalCoins(bool gameFinished = true){
        int newCoins = gameMechanics.Score/100; 
        // GlobalManager.coinsRecentlyAdded += newCoins;
        // if (gameFinished){
        //     GlobalManager.student.coins += GlobalManager.coinsRecentlyAdded;

        // }
    }
}
