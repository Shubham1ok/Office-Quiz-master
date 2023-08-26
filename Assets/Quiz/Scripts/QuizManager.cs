using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class QuizManager : MonoBehaviour
{
    private struct UserResponse
    {
        public string question;
        public string selectedAnswer;
        public string correctAnswer;
        public bool isCorrect;
    }

#pragma warning disable 649
    //ref to the QuizGameUI script
    [SerializeField] private QuizGameUI quizGameUI;
    //ref to the scriptableobject file
    [SerializeField] private List<QuizDataScriptable> quizDataList;
    [SerializeField] private float timeInSeconds;
    [SerializeField] private Result result;
    [SerializeField] private GameObject scrollHolder;
    [SerializeField] private Text overallPercentageText;
    [SerializeField] private Text scoreText;
#pragma warning restore 649




    private int currentQuestionIndex = 0;
    private string currentCategory = "";
    private int correctAnswerCount = 0;
    //questions data
    private List<Question> questions;
    //current question data
    private Question selectedQuetion = new Question();
    private int gameScore;
    //private int lifesRemaining; //life
    private float currentQuestionTime = 0f;  // Timer for the current question
    private List<UserResponse> userResponses = new List<UserResponse>();
    private QuizDataScriptable dataScriptable;

    private GameStatus gameStatus = GameStatus.NEXT;

    public GameStatus GameStatus { get { return gameStatus; } }

    public List<QuizDataScriptable> QuizData { get => quizDataList; }

    public void StartGame(int categoryIndex, string category)
    {
        currentCategory = category;
        correctAnswerCount = 0;
        gameScore = 0;
        //lifesRemaining = 3;
        currentQuestionTime = timeInSeconds;
        //set the questions data
        questions = new List<Question>();
        dataScriptable = quizDataList[categoryIndex];
        questions.AddRange(dataScriptable.questions);
        currentQuestionIndex = 0;  // Start with the first question
        selectedQuetion = questions[currentQuestionIndex];
        //select the question
        SelectQuestion();
        gameStatus = GameStatus.PLAYING;
    }

    /// <summary>
    /// Method used to randomly select the question form questions data
    /// </summary>
    private void SelectQuestion()
    {
        if (currentQuestionIndex < questions.Count)
        {
            selectedQuetion = questions[currentQuestionIndex];
            quizGameUI.SetQuestion(selectedQuetion);

            currentQuestionIndex++;  // Move to the next question
        }
        else
        {
            
            GameEnd();  // All questions have been answered
        }
    }

    private void Update()
    {
        if (gameStatus == GameStatus.PLAYING)
        {
            timeInSeconds -= Time.deltaTime;
            SetTime(timeInSeconds);
        }
    }

    void SetTime(float value)
    {
        TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);                       //set the time value
        quizGameUI.TimerText.text = time.ToString("mm':'ss");   //convert time to Time format

        if (timeInSeconds <= 0)
        {
            /*//Game Over
            GameEnd();*/
            
            SelectQuestion();
            //lifesRemaining--;
            timeInSeconds = currentQuestionTime;
            //quizGameUI.ReduceLife(lifesRemaining);
           /* if (lifesRemaining == 0)
            {
                GameEnd();
            }*/

        }
    }

    /// <summary>
    /// Method called to check the answer is correct or not
    /// </summary>
    /// <param name="selectedOption">answer string</param>
    /// <returns></returns>
    public bool Answer(string selectedOption)
    {
        //set default to false
        bool correct = false;
        //if selected answer is similar to the correctAns
        if (selectedQuetion.correctAns == selectedOption)
        {
            //Yes, Ans is correct

            correctAnswerCount++;
            correct = true;
            gameScore += 1;
            quizGameUI.ScoreText.text = "Score:" + gameScore;
            timeInSeconds = currentQuestionTime;
            // Store user response
            UserResponse response = new UserResponse
            {
                question = selectedQuetion.questionInfo,
                selectedAnswer = selectedOption,
                correctAnswer = selectedQuetion.correctAns,
                isCorrect = correct
            };
            userResponses.Add(response);
        }
        else
        {
            //No, Ans is wrong
            //Reduce Life
            //lifesRemaining--;
            //quizGameUI.ReduceLife(lifesRemaining);
           
            /*if (lifesRemaining == 0)
            {
                GameEnd();
            }*/
            timeInSeconds = currentQuestionTime;
            // Store user response
            UserResponse response = new UserResponse
            {
                question = selectedQuetion.questionInfo,
                selectedAnswer = selectedOption,
                correctAnswer = selectedQuetion.correctAns,
                isCorrect = correct
            };
            userResponses.Add(response);
        }

        if (gameStatus == GameStatus.PLAYING)
        {
            if (currentQuestionIndex < questions.Count)
            {
                Invoke("SelectQuestion", 0.4f);
            }
            else
            {
                GameEnd();
            }
        }
        //return the value of correct bool
        return correct;
    }

    private void GameEnd()
    {
        gameStatus = GameStatus.NEXT;
        quizGameUI.TopHolder.SetActive(false);
        quizGameUI.QuestionInfo.SetActive(false);
        quizGameUI.OptionsHolder.SetActive(false);
        quizGameUI.GameOverPanel.SetActive(true);

        int totalQuestionsAnswered = userResponses.Count;
        int totalQuestions = questions.Count;
        float percentageCorrect = (float)correctAnswerCount / totalQuestionsAnswered * 100;

        // Display or print the summary of questions and responses
        for (int i = 0; i < userResponses.Count; i++)
        {
           
            UserResponse response = userResponses[i];
            //string result = response.isCorrect ? "Correct" : "Incorrect";
            Result result1= Instantiate(result, scrollHolder.transform);
            result1.ResultQuestionAndAnswer(response.question, response.correctAnswer, response.selectedAnswer);
            
        }
        overallPercentageText.text = $"Overall Percentage: {percentageCorrect}%";
        scoreText.text = $"Final Score: {correctAnswerCount}";
        
       

        userResponses.Clear();  // Clear the responses for a new game
    }


}

//Datastructure for storeing the quetions data
[System.Serializable]
public class Question
{

    public string questionInfo;         //question text
    public QuestionType questionType;   //type
    public Sprite questionImage;        //image for Image Type
    public AudioClip audioClip;         //audio for audio type
    public UnityEngine.Video.VideoClip videoClip;   //video for video type
    public List<string> options;        //options to select
    public string correctAns;           //correct option
}

[System.Serializable]
public enum QuestionType
{
    TEXT,
    IMAGE,
    AUDIO,
    VIDEO
}

[SerializeField]
public enum GameStatus
{
    PLAYING,
    NEXT
}