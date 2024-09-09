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
    private VocabularyPacket currentPacket;

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

    // Animation controls
    [SerializeField] private AnimationCurve flipAnimCurve;
    private float flipTime = 0.8f;
    [SerializeField] private DialogueAnimator definitionAnimator;

    // Dev variables
    [SerializeField] private Sprite devIcon;

    // Start is called before the first frame update
    void Start()
    {
        headerText.text = $"Welcome to Packet {GlobalManager.Instance.CurrentPacket + 1}!";
        LoadWords();
        currentSlide = 0;
        currentWord = -1;
        progressStars = new ProgressStar[currentPacket.Entries.Count];
        flashcardIcon.preserveAspect = true;
        InstantiateStars();
        PrepWordVideo(currentPacket.Entries[0].ASL_Sign_and_Spelled);
    }

    private void LoadWords()
    {
        currentPacket = VocabularyLoader.Instance.VocabularyData.Packets[GlobalManager.Instance.CurrentPacket];
    }

    private void InstantiateStars()
    {
        Debug.Log("Instantiating stars");
        for (int i = 1; i < currentPacket.Entries.Count + 1; i++)
        {
            GameObject starObj = Instantiate(starPrefab);
            starObj.transform.SetParent(starsFolder, false);
            RectTransform starTransform = starObj.GetComponent<RectTransform>();
            // Anchor it to the middle left of its parent
            starTransform.anchorMin = new Vector2(0, 0.5f);
            starTransform.anchorMax = new Vector2(0, 0.5f);
            // Then set it to the proper spacing
            starTransform.anchoredPosition = new Vector2(((float)i / currentPacket.Entries.Count) * progressBar.sizeDelta.x - starOffset, 0);
            progressStars[i - 1] = starObj.GetComponent<ProgressStar>();
        }
    }

    public void NextSlide()
    {
        // Don't want button to work while we're animating. Skip the text first
        if (definitionAnimator.InProgress)
        {
            Debug.Log("Skip text!");
            return;
        }

        if (currentSlide == 0)
        {
            // Handle leaving intro slide
            NextWord();
            flashcardMask.gameObject.SetActive(true);
        } else if (currentSlide >= 2 * currentPacket.Entries.Count)
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
        progressMask.fillAmount = (float)currentSlide / (float)(2 * currentPacket.Entries.Count);
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

        if (currentWord < currentPacket.Entries.Count - 1)
        {
            PrepWordVideo(currentPacket.Entries[currentWord + 1].ASL_Sign_and_Spelled);
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
        VocabularyEntry currentFlashcard = currentPacket.Entries[currentWord];

        progressMask.fillAmount = (float)currentWord / (float)currentPacket.Entries.Count;
        if (currentWord > 0) progressStars[currentWord - 1].SetAchieved();

        definitionText.gameObject.SetActive(false);
        headerText.text = currentFlashcard.English_Word;
        definitionText.text = currentFlashcard.English_Definition;
        flashcardIcon.sprite = GlobalManager.Instance.GetIcon(currentFlashcard.Vocabulary_ID);
        definitionVideoPlayer.url = currentFlashcard.ASL_Definition;

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
