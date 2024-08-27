using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class SSQuestionManager : MonoBehaviour
{
	[SerializeField] private StreetSignsUIManager uiManager;
    public List<string> VocabWords;
    public string CorrectWord;

	public enum SSQuestionType
	{
		ASLSignToEnglishWord,
		EnglishDefinitionToEnglishWord,
		IconToEnglishWord,
	}

	private int questionTypeCount;

    // Start is called before the first frame update
    void Start()
    {
		questionTypeCount = System.Enum.GetNames(typeof(SSQuestionType)).Length;

        VocabWords = new List<string>(VideoManager.VocabWordToPathDict.Keys);
        SelectNewWord();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void LoadRandomQuestion()
	{
		// // Make panel randomly select question type
		// SSQuestionType selectedQuestionType = (SSQuestionType) UnityEngine.Random.Range(0, questionTypeCount);

		// // Populate panel according to question type
		// switch (selectedQuestionType)
		// {
		// 	case SSQuestionType.ASLSignToEnglishWord:
		// 		UpdateQuestionVideoPanel("What is this sign?", "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4");
		// 		break;
		// 	case SSQuestionType.EnglishDefinitionToEnglishWord:
		// 		UpdateQuestionOnlyPanel("The powerhouse of the cell is...");
		// 		break;
		// 	case SSQuestionType.IconToEnglishWord:
		// 		UpdateQuestionIconPanel("This image shows...", defaultIconToShow);
		// 		break;
		// }

		// // Populate buttons according to question
		// listOfAllPotentialAnswers = new List<string>{"correctAnswer", "mitochondria", "vacuole", "DNA", "ribosome"};
		// correctAnswer = "correctAnswer";
		// RenderButtonText();
	}

    public void SelectNewWord()
    {
        int randomIndex = Random.Range(0, VocabWords.Count);
        CorrectWord = VocabWords[randomIndex];
    }

    public string GetWordURL()
    {
        return VideoManager.VocabWordToPathDict[CorrectWord];
    }
}
