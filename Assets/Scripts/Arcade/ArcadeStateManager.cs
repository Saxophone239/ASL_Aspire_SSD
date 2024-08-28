using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeStateManager : MonoBehaviour
{
    public void PlaySICI()
	{
		SceneManager.LoadScene("SignItGameplay");
	}

	public void PlayMR()
	{
		SceneManager.LoadScene("MazeRunnerGameplay");
	}

	public void PlaySS()
	{
		SceneManager.LoadScene("StreetSigns");
	}
}
