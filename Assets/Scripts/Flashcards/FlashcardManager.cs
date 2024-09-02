using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FlashcardManager : MonoBehaviour
{
    // Describes what "slide" we're on given n vocab terms
    // 0 == intro slide
    // 1..2n == flashcard slides
    // 2n + 1 == ending slide
    private int currentSlide;
    private int currentWord;

    [SerializeField] private List<FlashcardData> flashcardList;

    // Basic flashcard variables
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI definitionText;
    [SerializeField] private Image flashcardIcon;

    // Video variables
    [SerializeField] private RectTransform screenParent;
    [SerializeField] private RectTransform frontScreen;
    [SerializeField] private RectTransform backScreen;
    [SerializeField] private VideoPlayer wordVideoPlayer;
    [SerializeField] private VideoPlayer definitionVideoPlayer;

    // Progress variables
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private Image progressMask;
    [SerializeField] private RectTransform starsFolder;
    [SerializeField] private GameObject starPrefab;
    private float starOffset = 30f;
    private ProgressStar[] progressStars;
    [SerializeField] private ParticleSystem confettiParticles;

    [SerializeField] private RectTransform flashcardMask;
    [SerializeField] private GameObject inputDisabler;
    [SerializeField] private RectTransform winScreen;
    [SerializeField] private Button nextArrow;

    [SerializeField] private AnimationCurve flipAnimCurve;
    private float flipTime = 0.8f;
    [SerializeField] private DialogueAnimator definitionAnimator;

    // Dev variables
    [SerializeField] private Sprite devIcon;

    // Start is called before the first frame update
    void Start()
    {
        flashcardList = new List<FlashcardData>();
        headerText.text = $"Welcome to Packet {GlobalManager.Instance.CurrentPacket + 1}!";
        LoadWords();
        currentSlide = 0;
        currentWord = -1;
        progressStars = new ProgressStar[flashcardList.Count];
        InstantiateStars();
        PrepWordVideo(flashcardList[0].wordVideoURL);
    }

    private void LoadWords()
    {
        VocabularyPacket currentPacket = VocabularyLoader.Instance.VocabularyData.Packets[GlobalManager.Instance.CurrentPacket];
        foreach (VocabularyEntry entry in currentPacket.Entries)
        {
            FlashcardData newCard = new FlashcardData();
            newCard.id = entry.Vocabulary_ID;
            newCard.word = entry.English_Word;
            newCard.definitionText = entry.English_Definition;
            newCard.wordVideoURL = entry.ASL_Sign_and_Spelled;
            newCard.definitionVideoURL = entry.ASL_Definition;
            newCard.icon = devIcon;
            flashcardList.Add(newCard);
        }
    }

    private void InstantiateStars()
    {
        Debug.Log("Instantiating stars");
        for (int i = 1; i < flashcardList.Count + 1; i++)
        {
            GameObject starObj = Instantiate(starPrefab);
            starObj.transform.SetParent(starsFolder, false);
            RectTransform starTransform = starObj.GetComponent<RectTransform>();
            // Anchor it to the middle left of its parent
            starTransform.anchorMin = new Vector2(0, 0.5f);
            starTransform.anchorMax = new Vector2(0, 0.5f);
            // Then set it to the proper spacing
            starTransform.anchoredPosition = new Vector2(((float)i / flashcardList.Count) * progressBar.sizeDelta.x - starOffset, 0);
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
        } else if (currentSlide >= 2*flashcardList.Count)
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
        StartCoroutine(AnimateFlip());
        progressMask.fillAmount = (float)currentSlide / (float)(2 * flashcardList.Count);
    }

    private IEnumerator AnimateFlip()
    {
        inputDisabler.SetActive(true);

        backScreen.gameObject.SetActive(true);
        definitionVideoPlayer.Pause();
        definitionVideoPlayer.frame = 0;
        Vector2 startPos = screenParent.anchoredPosition;
        Vector2 endPos = new Vector2(screenParent.anchoredPosition.x, -backScreen.anchoredPosition.y);
        
        float t = 0;
        while (t < 1)
        {
            screenParent.anchoredPosition = Vector2.Lerp(startPos, endPos, flipAnimCurve.Evaluate(t));
            t += Time.deltaTime / flipTime;
            yield return null;
        }
        // Set final position since we overshoot t == 1
        screenParent.anchoredPosition = endPos;

        definitionVideoPlayer.Play();
        frontScreen.gameObject.SetActive(false);
        definitionText.gameObject.SetActive(true);
        definitionAnimator.PlayAnimation();
        inputDisabler.SetActive(false);

        if (currentWord < flashcardList.Count - 1)
        {
            PrepWordVideo(flashcardList[currentWord + 1].wordVideoURL);
        }
        yield break;
    }

    private void PrepWordVideo(string url)
    {
        wordVideoPlayer.url = url;
        wordVideoPlayer.Pause();
        wordVideoPlayer.frame = 0;
    }

    private void NextWord()
    {
        currentWord++;
        FlashcardData currentFlashcard = flashcardList[currentWord];

        progressMask.fillAmount = (float)currentWord / (float)flashcardList.Count;
        if (currentWord > 0) progressStars[currentWord - 1].SetAchieved();

        definitionText.gameObject.SetActive(false);
        headerText.text = currentFlashcard.word;
        definitionText.text = currentFlashcard.definitionText;
        flashcardIcon.sprite = currentFlashcard.icon;
        definitionVideoPlayer.url = currentFlashcard.definitionVideoURL;

        frontScreen.gameObject.SetActive(true);
        wordVideoPlayer.Play();
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
        confettiParticles.Play();
    }

    public void ExitToMap()
    {
        // TODO: Scene logic
        SceneManager.LoadScene("MainPageScene");
    }
}
