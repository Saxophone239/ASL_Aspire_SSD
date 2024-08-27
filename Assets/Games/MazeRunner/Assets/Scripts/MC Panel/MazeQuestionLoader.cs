using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class MazeQuestionLoader : MonoBehaviour
{
    // UI Components
    [Header("UI components")]
	public GameObject QuestionOnlyPanel;
	public TextMeshProUGUI QuestionOnlyText;

	public GameObject QuestionIconPanel;
	public TextMeshProUGUI QuestionIconText;
	public Image QuestionIconImage;

	public GameObject QuestionVideoPanel;
	public TextMeshProUGUI QuestionVideoText;
	public VideoPlayer QuestionVideoVideoplayer;
    // public VideoPlayerController VideoPlayerController;

    public Button ButtonAnswer1;
    public Button ButtonAnswer2;
    public Button ButtonAnswer3;
    public Button ButtonAnswer4;

	[Header("References")]
	[SerializeField] private Animator canvasAnimator;
	[SerializeField] private Player player;
	[SerializeField] private MazeGameMechanics gameMechanics;
	[SerializeField] private Sprite defaultIconToShow;

	private GameObject currentActiveQuestionPanel;

    // JSON file reading
    //private List<Question> _questions = new List<Question>();
    private Dictionary<string, string> vocabToLinkDict = new Dictionary<string, string>();
    private bool isWebGL;

    // Private vars
    private string correctAnswer; // the current word that's being asked
    private List<string> listOfAllPotentialAnswers;
    public TimerBar timer;
	private int questionTypeCount;

	public enum MazeQuestionType
	{
		ASLSignToEnglishWord,
		EnglishDefinitionToEnglishWord,
		EnglishWordToEnglishDefinition,
		ASLDefinitionToEnglishDefinition,
		IconToEnglishWord,
	}

    //New UX

    // Start is called before the first frame update
    void Start()
    {
        //s Check device type (WebGL vs iOS)
        // if (GlobalManager.currentDeviceType.Equals("Desktop"))
        // {
        //     // // Use JSON file videos
        //     // if (!GlobalManager.currentJson)
        //     // {
        //     //     throw new System.Exception("JSON file doesn't exist!");
        //     // }
            
        //     VideoManager.GenerateVocabListFromSelectedVocabSet(); 
        //     vocabToLinkDict = VideoManager.VocabWordToPathDict;
        //     wordList = vocabToLinkDict.Keys.ToList();

        //     isWebGL = true;
        // }
        // else
        // {
        //     // Use StreamingAssets videos
        //     wordList = VideoPlayerController.VocabWordToPathDict.Keys.ToList();

        //     isWebGL = false;
        // }
        
        // Debug.Log($"wordList Size = {wordList.Count}");
        // LoadRandomQuestion();
        // VideoPlayerController.videoPlayer.prepareCompleted += playVideoAndStartTimer;

		questionTypeCount = System.Enum.GetNames(typeof(MazeQuestionType)).Length;
    }

	public void LoadRandomQuestion()
	{
		// Make panel randomly select question type
		MazeQuestionType selectedQuestionType = (MazeQuestionType) UnityEngine.Random.Range(0, questionTypeCount);

		// Populate panel according to question type
		switch (selectedQuestionType)
		{
			case MazeQuestionType.ASLSignToEnglishWord:
				UpdateQuestionVideoPanel("What is this sign?", "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4");
				break;
			case MazeQuestionType.EnglishDefinitionToEnglishWord:
				UpdateQuestionOnlyPanel("The powerhouse of the cell is...");
				break;
			case MazeQuestionType.EnglishWordToEnglishDefinition:
				UpdateQuestionOnlyPanel("The mitochondria is...");
				break;
			case MazeQuestionType.ASLDefinitionToEnglishDefinition:
				UpdateQuestionVideoPanel("This definition pairs with...", "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4");
				break;
			case MazeQuestionType.IconToEnglishWord:
				UpdateQuestionIconPanel("This image shows...", defaultIconToShow);
				break;
		}

		// Populate buttons according to question
		listOfAllPotentialAnswers = new List<string>{"correctAnswer", "mitochondria", "vacuole", "DNA", "ribosome"};
		correctAnswer = "correctAnswer";
		RenderButtonText();
	}

	public void UpdateQuestionOnlyPanel(string text)
	{
		currentActiveQuestionPanel = QuestionOnlyPanel;
		currentActiveQuestionPanel.SetActive(true);

		QuestionOnlyText.text = text;

		timer.RestartTimer();
	}

	public void UpdateQuestionIconPanel(string questionText, Sprite icon)
	{
		currentActiveQuestionPanel = QuestionIconPanel;
		currentActiveQuestionPanel.SetActive(true);

		QuestionIconText.text = questionText;
		QuestionIconImage.sprite = icon;

		timer.RestartTimer();
	}

	public void UpdateQuestionVideoPanel(string questionText, string videoURL)
	{
		currentActiveQuestionPanel = QuestionVideoPanel;
		currentActiveQuestionPanel.SetActive(true);

		QuestionVideoText.text = questionText;
		QuestionVideoVideoplayer.url = videoURL;
		QuestionVideoVideoplayer.Prepare();
		QuestionVideoVideoplayer.prepareCompleted += playVideoAndStartTimer;
	}

	// When prepareCompleted event is received, start the timer
    public void playVideoAndStartTimer(object sender)
	{
        QuestionVideoVideoplayer.Play();

        timer.RestartTimer();
    }

    // /// <summary>
    // /// Loads a question and 4 answers into the panel's contents
    // /// </summary>
    // public void LoadRandomQuestion()
    // {
    //     int randomIndex = GetRandomQuestionIndex();
    //     Debug.Log($"correct word: {wordList[randomIndex]}");
    //     LoadWord(wordList[randomIndex]);
    // }

    // /// <summary>
    // /// Get a random question index
    // /// </summary>
    // /// <returns>Returns an int corresponding to an array index</returns>
    // public int GetRandomQuestionIndex()
    // {
    //     return Random.Range(0, wordList.Count);
    // }

    // /// <summary>
    // /// Loads the question/word into the videoplayer and buttons
    // /// </summary>
    // /// <param name="word">Word the panel is asking about</param>
    // public void LoadWord(string word)
    // {
    //     _currentWord = word;
    //     RenderButtonText(word);
    //     RenderQuestionText(word);
    //     RenderVideo(word, isWebGL);
    // }

    // /// <summary>
    // /// Renders question text
    // /// </summary>
    // /// <param name="question">Question we're asking</param>
    // public void RenderQuestionText(string question)
    // {
    //     // VideoQuestionText.text = "What sign does the video show?";
    // }

    /// <summary>
    /// Creates 4 random answers, one of them being correct, and assigns them to random buttons
    /// </summary>
    /// <param name="question">Word we're asking about</param>
    public void RenderButtonText()
    {
        List<string> answersShuffled = GetRandomAnswers(listOfAllPotentialAnswers, correctAnswer);

        ButtonAnswer1.gameObject.GetComponent<MazeButtonHandler>().SetText(answersShuffled[0]);
        ButtonAnswer2.gameObject.GetComponent<MazeButtonHandler>().SetText(answersShuffled[1]);
        ButtonAnswer3.gameObject.GetComponent<MazeButtonHandler>().SetText(answersShuffled[2]);
        ButtonAnswer4.gameObject.GetComponent<MazeButtonHandler>().SetText(answersShuffled[3]);
    }

	/// <summary>
    /// Takes a list of vocab words and randomly selects 4 words, one of them being the correct answer
    /// </summary>
    /// <param name="vocabList">List of vocab words</param>
    /// <param name="correctAns">The correct vocab word</param>
    /// <returns>Returns a size 4 array of vocab words, one of them being correct</returns>
    public List<string> GetRandomAnswers(List<string> vocabList, string correctAns)
    {
        List<string> toReturn = new List<string>();

        // Make list of 4 random words, including current one
        List<string> inputList = new List<string>();
        for (int i = 0; i < vocabList.Count; i++)
        {
            inputList.Add(vocabList[i]);
        }
        toReturn.Add(correctAns);
        inputList.Remove(correctAns);

        for (int i = 0; i < 3; i++)
        {
            int rndNum = Random.Range(0, inputList.Count);
            toReturn.Add(inputList[rndNum]);
            inputList.Remove(inputList[rndNum]);
        }

        // Randomize list
        for (int i = 0; i < toReturn.Count; i++)
        {
            string temp = toReturn[i];
            int randomIndex = Random.Range(i, toReturn.Count);
            toReturn[i] = toReturn[randomIndex];
            toReturn[randomIndex] = temp;
        }

        return toReturn;
    }

    /// <summary>
    /// Render the video associated with the question we're asking
    /// </summary>
    /// <param name="word">Word corresponding to title/sign of video</param>
    public void RenderVideo(string word, bool isWebGL)
    {
        // if (isWebGL) VideoPlayerController.videoPlayer.url = vocabToLinkDict[word];
        // else VideoPlayerController.PrepareVideo(word);
    }

    public async void VideoLoadDelay()
    {
        // whatever you need to do before delay goes here         

        await Task.Delay(2000);

        // whatever you need to do after delay.
    }

    /// <summary>
    /// Checks if the button selection matches the correct answer shown in the video
    /// </summary>
    /// <param name="answerText">The text of the button selected</param>
    /// <returns>Returns a bool corresponding to whether that selection was correct</returns>
    public bool IsAnswerCorrect(string answerText)
    {
        return correctAnswer.Equals(answerText);
    }

	/// <summary>
    /// Upon end of MC panel's duration, destroys the panel, plays coin collection animation, and resumes gameplay
    /// </summary>
    public void HandleEndOfPanelLogic(bool won)
    {
		timer.StopTimer();
		canvasAnimator.SetTrigger("TriggerQuestionPanel");
        player.CurrentCoinCollectible.GetComponent<Collider>().enabled = false;
        if (won)
		{
            player.CurrentCoinCollectible.GetComponent<Animator>().SetTrigger("WonCoin");
        }
		else
		{ 
            player.CurrentCoinCollectible.GetComponent<Animator>().SetTrigger("LostCoin");
        }
        player.UnpauseShieldTimer();
        player.IsCoinTouched = false;
        gameMechanics.IsGameplayActive = true;
    }

	private IEnumerator TurnOffActiveQuestionPanel()
	{
		yield return new WaitForSeconds(0.75f);
		currentActiveQuestionPanel.SetActive(false);
	}
}
