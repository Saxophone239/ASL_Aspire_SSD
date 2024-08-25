using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    private int answerNum;
    private Image buttonImage;
    [SerializeField] private Color correctColor;
    [SerializeField] private Color incorrectColor;
    private Color defaultColor;


    public int AnswerNum
    {
        get { return answerNum; }
    }

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
        defaultColor = buttonImage.color;
    }

    public void HighlightCorrect()
    {
        buttonImage.color = correctColor;
    }

    public void HighlightIncorrect()
    {
        buttonImage.color = incorrectColor;
    }

    public void ResetColor()
    {
        // TODO: Handle null component because it's causing issues
        if (!buttonImage) return;
        buttonImage.color = defaultColor;
    }
}
