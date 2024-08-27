using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MazeButtonHandler : MonoBehaviour
{   
    [Header("Managers")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private MazeGameMechanics gameMechanics;
	[SerializeField] private GameObject confettiParticleSystem;
	[SerializeField] private MazeQuestionLoader ql;
	[SerializeField] private TextMeshProUGUI answerText;
	[SerializeField] private Player player;

	[Header("Settings")]
	public float AnimationTime = 0.1f;
    

    private Button _button;

    private string currentText;

    /// <summary>
    /// Sets the text of the button
    /// </summary>
    /// <param name="txt">Text to set</param>
    public void SetText(string txt)
	{
		answerText.text = txt;
        currentText = txt;
    }

    /// <summary>
    /// Handles a MC button click and handles logic according to whether that was the correct answer
    /// </summary>
    public void HandleClick()
	{
        if (ql.IsAnswerCorrect(currentText))
		{
            StartCoroutine(CorrectAnswer());
        }
		else
		{
            StartCoroutine(WrongAnswer());
        }

        Invoke("ResetToWhite", AnimationTime);
    }

    public void ResetToWhite()
	{
        SetColor(Color.white);
    }

    /// <summary>
    /// Sets the color of the button
    /// </summary>
    /// <param name="c">Color to set to</param>
    public void SetColor(Color c)
	{
        var bg = GetComponent<Image>();
        bg.color = c;
    }

    private IEnumerator CorrectAnswer()
    {
        SetColor(Color.green);
        yield return new WaitForSeconds(0.5f);

        ql.HandleEndOfPanelLogic(true);

        gameMechanics.AddScore(1);

        GameObject burst = Instantiate(confettiParticleSystem, player.transform.position, Quaternion.identity);
        burst.GetComponent<ParticleSystem>().Play();
    }

    private IEnumerator WrongAnswer()
    {
        SetColor(Color.red);
        yield return new WaitForSeconds(0.5f);

        ql.HandleEndOfPanelLogic(false);
    }
}
