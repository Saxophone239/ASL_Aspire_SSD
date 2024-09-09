using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeStudentData : MonoBehaviour
{
    public DataModels dataModels;
    public PlayfabPostManager postManager;
    // Start is called before the first frame update
    public void InitializeAllStudentData(){

        //Initialize Reviews
        int[] review0Packets = {1,2,3};
        int[] review1Packets = {4,5,6};
        int[] review2Packets = {7,8,9};
        int[] review3Packets = {10,11};

        List<int[]> reviewSetupList = new List<int[]>();
        reviewSetupList.Add(review0Packets);
        reviewSetupList.Add(review1Packets);
        reviewSetupList.Add(review2Packets);
        reviewSetupList.Add(review3Packets);

        int reviewIndex = 1; 
        foreach (int[] reviewPacketsList in reviewSetupList){
            ReviewData reviewData = dataModels.InitializeReviewFromVocabulary(reviewPacketsList);
            reviewData.reviewID = reviewIndex;
            reviewIndex += 1; 
            postManager.PostReview(reviewData);
        }


        //Initialize Lessons
        for(int i = 1; i < 12; i++ ){
            LessonData lessonData = dataModels.InitializeLessonFromVocabulary(i);
            lessonData.packetID = i;
            postManager.PostLesson(lessonData);
        }
        


    }
}
