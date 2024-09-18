using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeStateManager : MonoBehaviour
{
	[SerializeField] private GameObject MRspinner;
	[SerializeField] private GameObject SICIspinner;
	[SerializeField] private GameObject SSspinner;
	[SerializeField] private TextMeshProUGUI ticketText;

	private void Start()
	{
		SICIspinner.SetActive(false);
		MRspinner.SetActive(false);
		SSspinner.SetActive(false);
	}

    public void PlaySICI()
	{
		SICIspinner.SetActive(true);
		// SceneManager.LoadScene("SignItGameplay");
		StartCoroutine(LoadMyGameSceneAsync("SignItGameplay", 1));
	}

	public void PlayMR()
	{
		MRspinner.SetActive(true);
		// SceneManager.LoadScene("MazeRunnerGameplay");
		StartCoroutine(LoadMyGameSceneAsync("MazeRunnerGameplay", 0));
	}

	public void PlaySS()
	{
		SSspinner.SetActive(true);
		// SceneManager.LoadScene("StreetSigns");
		StartCoroutine(LoadMyGameSceneAsync("StreetSigns", 2));
	}

	public void UpdateTicketText(int numberToChangeBy)
	{
		GlobalManager.Instance.TotalCoinsPlayerHas += numberToChangeBy;
		ticketText.text = GlobalManager.Instance.TotalCoinsPlayerHas.ToString();
	}

	private IEnumerator LoadMyGameSceneAsync(string sceneName, int gameID)
	{
		GameSession session = new GameSession();
		session.arcadeGameID = gameID;
		GlobalManager.Instance.currentLoginSession.gameSessionList.Add(session);
		GlobalManager.Instance.GameStartTime = Time.realtimeSinceStartup;

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}
