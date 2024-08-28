using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MazeGlobals;

public class MazeMenuScreenManager : MonoBehaviour
{
	[SerializeField] private MazeSpawner mazeSpawner;
	[SerializeField] private MRCameraController cameraController;
    [SerializeField] private ToggleGroup difficultyToggleGroup;

	public event Action OnGameActivated;

    public string MRGameplaySceneName = "MazeRunnerGameplay";

    // Start is called before the first frame update
    private void Start()
    {
        // loadingPanel.SetActive(false);
        Time.timeScale = 1.0f;
		cameraController.BeginIntroAnimation();
    }
    
    // public void UpdateGlobalCoins(bool gameFinished = true)
	// {
    //     if (gameFinished)
	// 	{
    //         Debug.Log("Updating globalmanger coins");
    //         // GlobalManager.student.coins += GlobalManager.coinsRecentlyAdded;
    //     }
    // }

    public async void OnStartButtonClick()
    {
        string difficulty = difficultyToggleGroup.ActiveToggles().FirstOrDefault().ToString();
        if (CaseInsensitiveContains(difficulty, "easy"))
        {
            MazeGlobals.difficulty = MazeRunnerDifficulty.Easy;
        } else if (CaseInsensitiveContains(difficulty, "medium"))
        {
            MazeGlobals.difficulty = MazeRunnerDifficulty.Medium;
        } else if (CaseInsensitiveContains(difficulty, "hard"))
        {
            MazeGlobals.difficulty = MazeRunnerDifficulty.Hard;
        } else
        {
            throw new System.Exception("Unknown difficulty selection, ensure name of toggle has difficulty written in it.");
        }

		await mazeSpawner.CreateMaze(MazeGlobals.difficulty);

		OnGameActivated?.Invoke();
    }

    private bool CaseInsensitiveContains(string source, string toCompare)
    {
        return source.IndexOf(toCompare, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

	public void OnPausePanelEnter()
	{
		Time.timeScale = 0.0f;
	}

	public void OnPausePanelExit()
	{
		Time.timeScale = 1.0f;
	}

	public void OnBackToMenuButtonPress()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void BackToArcadeButton()
	{
        //UpdateGlobalCoins(true);
		StartCoroutine(LoadMainSceneAsync());
    }

	private IEnumerator LoadMainSceneAsync()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);

		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}
