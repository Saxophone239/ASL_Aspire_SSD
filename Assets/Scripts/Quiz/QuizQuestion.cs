using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    public int questionID;
    public string questionText;
    // See QuestionType.cs for explanation
    public QuestionType questionType;
    public string videoURL;
    public Sprite icon;
    // 0..3 correspond to A..D
    public string[] answers;
    public int correctAnswer;
}
