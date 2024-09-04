using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
    public class StudentData
    {
        public Dictionary<int,LessonData> lessonDataDictionary; //Will populate with PACKETID:LessonData

		public Dictionary<int,ReviewData> reviewDataDictionary; 

        public LoginSession[] loginSessionList;

    }

    [System.Serializable]
    public class LessonData
    {
		public int packetID; 
        public Dictionary<int,float> flashcardData;   //Will populate with wordID:TIMESPENT in flashcard area
		public Dictionary<int,Dictionary<string,int>> gameVocabCountDict; //Will populate with wordID:Dict<TypeOfRepresentationOfWord:Counts>
		
		public bool isUnlocked = false; //Manipulated manually by researchers
		public bool gameSessionComplete = false;
		public bool flashcardsComplete = false;
		public bool lessonComplete = false; 


		public LessonData()
        {
            flashcardData = new Dictionary<int, float>();
            gameVocabCountDict = new Dictionary<int, Dictionary<string, int>>();
        }
		
    }

	[System.Serializable]
	public class ReviewData
    {
		public Dictionary<int,Dictionary<string,int>> gameVocabCountDict; //Will populate with wordID:Dict<TypeOfRepresentationOfWord:Counts>		
		
		public Dictionary<int,QuizQuestionObject> quizDataDictionary; //Will populate with QuizQuestionObjectID: QuizQuestionObject data
		public bool isUnlocked = false; //Manipulated manually by researchers
		public bool gameSessionComplete;
		public bool flashcardsComplete;
		public bool lessonComplete; 
		
    }

	[System.Serializable]

	public class QuizQuestionObject{
		public bool successfulAnswer; 
		public int numAttempts;
		public int vocabID;

		public bool supplementaryPresent;

		public bool supplementaryAccessed; //indicates if extra material was viewed by the student for a word

	}


	[System.Serializable]

	public class LoginSession{
		public string date; 
		public string sessionID; 
		public int[] packetsInteractedWith; //ID of the lessons/reviews interacted with

		public GameSession[] gameSessionList;
	}


	[System.Serializable]

	public class GameSession{ //A class representing a single instance of a game being played
		public bool exitPressed; //if exit button pressed
		public bool tutorialPressed;
		public float timeSpent; 
		public int arcadeGameID; //Each game should have ID. Maze = 0, SignIt = 1, StreetSigns = 2
		public int sessionScore; 
		public int ticketsEarned;

	}






public class DataModels : MonoBehaviour
{
	
}
