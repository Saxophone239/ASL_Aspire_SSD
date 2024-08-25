using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class QuestionManager : MonoBehaviour
{
    private int questionIdx;
    private bool lastAnswerCorrect;
    [SerializeField] private List<QuizQuestion> quizQuestions;
    [SerializeField] private AnswerButton[] answerButtons;

    [SerializeField] RectTransform startSlide;
    [SerializeField] RectTransform questionSlide;
    [SerializeField] RectTransform endSlide;

    // Companion elements
    [SerializeField] private Image speechBubble;

    // Question Slide elements
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI questionNumberText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private RawImage videoScreen;
    [SerializeField] private Image icon;
    [SerializeField] private Image progressMask;

    // End Slide elements
    [SerializeField] private TextMeshProUGUI numLearnedText;

    [SerializeField] private VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        questionIdx = -1;
    }

    public void StartQuiz()
    {
        startSlide.gameObject.SetActive(false);
        questionSlide.gameObject.SetActive(true);
        lastAnswerCorrect = true;
        NextQuestion();
        foreach(QuizQuestion question in quizQuestions)
        {
            Debug.Log(JsonUtility.ToJson(question, true));
        }
    }

    public void NextQuestion()
    {

        // If last question was correct, move on with it in place
        // If it was incorrect shuffle it to the end and try again later
        if (lastAnswerCorrect)
        {
            questionIdx++;
        } else
        {
            QuizQuestion toShuffle = quizQuestions[questionIdx];
            quizQuestions.RemoveAt(questionIdx);
            quizQuestions.Add(toShuffle);
        }

        // If we've gotten them all correct, move to the end
        if (questionIdx >= quizQuestions.Count)
        {
            FinishQuiz();
            return;
        }

        continueButton.gameObject.SetActive(false);
        QuizQuestion currentQuestion = quizQuestions[questionIdx];

        // Do the following every time
        questionText.text = currentQuestion.questionText;
        questionNumberText.text = "Question " + (questionIdx + 1) + "/" + quizQuestions.Count;
        videoPlayer.url = currentQuestion.videoURL;
        icon.sprite = currentQuestion.icon;
        // Set the answer text
        for (int i = 0; i < 4; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i];
            answerButtons[i].ResetColor();
            answerButtons[i].GetComponent<Button>().enabled = true;
        }
        // Update progress bar
        progressMask.fillAmount = (float)questionIdx / quizQuestions.Count;
        // Update companion
        speechBubble.gameObject.SetActive(false);

        // Handle specific question type (hide or show supplementary)
        switch (currentQuestion.questionType)
        {
            case QuestionType.SignWordToWord:
                ActivateSupplementary(true, false);
                break;
            case QuestionType.DefToWord:
                ActivateSupplementary(false, false);
                break;
            case QuestionType.WordToDef:
                ActivateSupplementary(false, false);
                break;
            case QuestionType.SignDefToDef:
                ActivateSupplementary(true, false);
                break;
            case QuestionType.IconToWord:
                ActivateSupplementary(false, true);
                break;
            default:
                Debug.LogError("Unknown question type");
                break;
        }
    }

    public void FinishQuiz()
    {
        questionSlide.gameObject.SetActive(false);
        numLearnedText.text = "You just learned " + quizQuestions.Count + " words!";
        endSlide.gameObject.SetActive(true);
    }

    private void ActivateSupplementary(bool hasVideo, bool hasIcon)
    {
        videoScreen.gameObject.SetActive(hasVideo);
        icon.gameObject.SetActive(hasIcon);
    }

    public void CheckAnswer(int selectedAnswer)
    {
        QuizQuestion currentQuestion = quizQuestions[questionIdx];
        if (selectedAnswer != currentQuestion.correctAnswer)
        {
            // Highlight incorrect choice
            answerButtons[selectedAnswer].HighlightIncorrect();
            speechBubble.gameObject.SetActive(true);
            lastAnswerCorrect = false;
        } else
        {
            lastAnswerCorrect = true;
        }
        answerButtons[currentQuestion.correctAnswer].HighlightCorrect();
        // TODO: Disable all the buttons, move on to next question
        foreach (AnswerButton answerButton in answerButtons)
        {
            answerButton.GetComponent<Button>().enabled = false;
        }
        continueButton.gameObject.SetActive(true);
    }
}
