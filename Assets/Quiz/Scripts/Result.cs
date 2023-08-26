using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Result : MonoBehaviour
{
    [SerializeField] private Text questionText;
    [SerializeField] private Text correctanswerText;
    [SerializeField] private Text yourAnswerText;


    

    public void ResultQuestionAndAnswer(string question, string correctanswer, string yourAnswer)
    {
        questionText.text = question;
        correctanswerText.text = correctanswer;
        yourAnswerText.text = yourAnswer;

    }
}
