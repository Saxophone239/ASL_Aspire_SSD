using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class FlashcardManager : MonoBehaviour
{
    // Describes what "slide" we're on given n vocab terms
    // 0 == intro slide
    // 1..2n == flashcard slides
    // 2n + 1 == ending slide
    private int currentSlide;
    private int currentWord;

    [SerializeField] private FlashcardData[] flashcardArr;

    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI definitionText;
    [SerializeField] private Image flashcardIcon;
    [SerializeField] private VideoPlayer wordVideoPlayer;
    [SerializeField] private VideoPlayer definitionVideoPlayer;

    [SerializeField] private RectTransform screenParent;
    [SerializeField] private RectTransform frontScreen;
    [SerializeField] private RectTransform backScreen;

    [SerializeField] private RectTransform progressBar;
    [SerializeField] private Image progressMask;
    [SerializeField] private RectTransform starsFolder;
    [SerializeField] private GameObject starPrefab;
    private float starOffset = 30f;
    private ProgressStar[] progressStars;

    [SerializeField] private RectTransform flashcardMask;
    [SerializeField] private RectTransform winScreen;
    [SerializeField] private Button nextArrow;

    // Start is called before the first frame update
    void Start()
    {
        currentSlide = 0;
        currentWord = -1;
        progressStars = new ProgressStar[flashcardArr.Length];
        InstantiateStars();
    }

    private void InstantiateStars()
    {
        Debug.Log("Instantiating stars");
        for (int i = 1; i < flashcardArr.Length + 1; i++)
        {
            GameObject starObj = Instantiate(starPrefab);
            starObj.transform.SetParent(starsFolder, false);
            RectTransform starTransform = starObj.GetComponent<RectTransform>();
            // Anchor it to the middle left of its parent
            starTransform.anchorMin = new Vector2(0, 0.5f);
            starTransform.anchorMax = new Vector2(0, 0.5f);
            // Then set it to the proper spacing
            starTransform.anchoredPosition = new Vector2(((float)i / flashcardArr.Length) * progressBar.sizeDelta.x - starOffset, 0);
            progressStars[i - 1] = starObj.GetComponent<ProgressStar>();
        }
    }

    public void NextSlide()
    {
        if (currentSlide == 0)
        {
            // Handle leaving intro slide
            NextWord();
            flashcardMask.gameObject.SetActive(true);
        } else if (currentSlide >= 2*flashcardArr.Length)
        {
            // Handle moving to ending slide
            MoveToEnd();
        } else if (currentSlide % 2 == 1)
        {
            // Handle flipping flashcard
            FlipCard();
        } else
        {
            // Handle changing to next flashcard
            NextWord();
        }
        currentSlide++;
    }

    private void FlipCard()
    {
        headerText.text += " means...";
        definitionText.gameObject.SetActive(true);
        backScreen.gameObject.SetActive(true);
        screenParent.anchoredPosition = new Vector2(screenParent.anchoredPosition.x, -backScreen.anchoredPosition.y);
        frontScreen.gameObject.SetActive(false);
        progressMask.fillAmount = (float)currentSlide / (float)(2 * flashcardArr.Length);
        Debug.Log("Progress = " + progressMask.fillAmount);
    }

    private void NextWord()
    {
        currentWord++;
        FlashcardData currentFlashcard = flashcardArr[currentWord];

        progressMask.fillAmount = (float)currentWord / (float)flashcardArr.Length;
        if (currentWord > 0) progressStars[currentWord - 1].SetAchieved();

        definitionText.gameObject.SetActive(false);
        headerText.text = currentFlashcard.word;
        definitionText.text = currentFlashcard.definitionText;
        flashcardIcon.sprite = currentFlashcard.icon;
        wordVideoPlayer.url = currentFlashcard.wordVideoURL;
        definitionVideoPlayer.url = currentFlashcard.definitionVideoURL;

        frontScreen.gameObject.SetActive(true);
        screenParent.anchoredPosition = new Vector2(screenParent.anchoredPosition.x, 0f);
        backScreen.gameObject.SetActive(false);
    }

    private void MoveToEnd()
    {
        definitionText.gameObject.SetActive(false);
        flashcardMask.gameObject.SetActive(false);
        nextArrow.gameObject.SetActive(false);
        headerText.text = "You did it!";
        progressMask.fillAmount = 1;
        progressStars[progressStars.Length - 1].SetAchieved();
        winScreen.gameObject.SetActive(true);
    }

    public void ExitToMap()
    {
        // TODO: Scene logic
    }
}
