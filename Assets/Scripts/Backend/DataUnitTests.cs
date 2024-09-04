using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUnitTests : MonoBehaviour     //Testing models and routes without requiring the full app loaded
{

    public Login login;
    public  PlayfabPostManager postManager;
 
    void Start(){
        login.StudentLoginActivate("Student1");
    }
 

    public void LessonDataTest(){
        LessonData lesson = new LessonData();

        // Populating flashcardData with example word IDs and time spent
        lesson.flashcardData.Add(101, 3.5f); // wordID: 101, timeSpent: 3.5 seconds
        lesson.flashcardData.Add(102, 5.0f); // wordID: 102, timeSpent: 5.0 seconds

        // Populating gameVocabCountDict with example word IDs and their representations
        lesson.gameVocabCountDict.Add(101, new Dictionary<string, int>
        {
            {"SIGNEDWORD-ENGWORD", 2},  // TypeOfRepresentation: SIGNEDWORD-ENGWORD
            {"ICON-ENGWORD", 1}   // TypeOfRepresentation: ICON-ENGWORD, Counts: 1
        });
        lesson.gameVocabCountDict.Add(102, new Dictionary<string, int>
        {
            {"SIGNEDWORD-ENGWORD", 1},  
            {"ENGDEF-ENGWORD", 3},
            {"ICON-ENGWORD",1}   
        });

        lesson.isUnlocked = true;
        lesson.gameSessionComplete = false;
        lesson.flashcardsComplete = true;
        lesson.lessonComplete = false;

        Debug.Log("Flashcard Data:");
        foreach (var item in lesson.flashcardData)
        {
            Debug.Log($"WordID: {item.Key}, Time Spent: {item.Value} seconds");
        }

        Debug.Log("\nGame Vocab Count Data:");
        foreach (var item in lesson.gameVocabCountDict)
        {
            Debug.Log($"WordID: {item.Key}");
            foreach (var subItem in item.Value)
            {
                Debug.Log($"   Type: {subItem.Key}, Count: {subItem.Value}");
            }
        }

        Debug.Log($"\nIs Unlocked: {lesson.isUnlocked}");
        Debug.Log($"Game Session Complete: {lesson.gameSessionComplete}");
        Debug.Log($"Flashcards Complete: {lesson.flashcardsComplete}");
        Debug.Log($"Lesson Complete: {lesson.lessonComplete}");


        //Attempt Post
        postManager.PostLesson(lesson);         

    }


    public void QuizQuestionObjectTest()
    {
        QuizQuestionObject quizQuestion = new QuizQuestionObject();

        quizQuestion.successfulAnswer = true;
        quizQuestion.numAttempts = 2;
        quizQuestion.vocabID = 101;
        quizQuestion.supplementaryPresent = true;
        quizQuestion.supplementaryAccessed = false;

        Debug.Log("Quiz Question Object Data:");
        Debug.Log($"Successful Answer: {quizQuestion.successfulAnswer}");
        Debug.Log($"Number of Attempts: {quizQuestion.numAttempts}");
        Debug.Log($"Vocabulary ID: {quizQuestion.vocabID}");
        Debug.Log($"Supplementary Material Present: {quizQuestion.supplementaryPresent}");
        Debug.Log($"Supplementary Material Accessed: {quizQuestion.supplementaryAccessed}");

        // postManager.PostQuizQuestion(quizQuestion);
    }




}
