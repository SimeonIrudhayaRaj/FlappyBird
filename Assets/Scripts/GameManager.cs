using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager instance;
    public GameObject startPage, gameOverPage, countDownPage;
    public Text scoreText;

        public AudioSource bgSound; 

    public enum PageState {
        None,
        Start,
        GameOver,
        CountDown
    }
    int score = 0;
    bool gameOver = true;

    public bool isGameOver() {
        return gameOver;
    }
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }

    void SetPageState(PageState state) {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(false);
            break;

            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(false);
            break;

            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countDownPage.SetActive(false);
            break;

            case PageState.CountDown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(true);
            break;   
        }
    }

    public void ConfirmGameOver() {
        OnGameOverConfirmed();
        scoreText.text = "0";
        SetPageState(PageState.Start);
        SceneManager.LoadScene("color");
    }

    public void StartGame() {
        SetPageState(PageState.CountDown);
        bgSound.Play();
    }

    void OnEnable() {
        CountDown.OnCountDownFinished += OnCountDownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
        BallController.OnPlayerDied += OnPlayerDied;
        BallController.OnPlayerScored += OnPlayerScored;
    }
    void OnDisable() {
        CountDown.OnCountDownFinished -= OnCountDownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
        BallController.OnPlayerDied -= OnPlayerDied;
        BallController.OnPlayerScored -= OnPlayerScored;
    }
    void OnCountDownFinished() {
        gameOver = false;
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
    }

    void OnPlayerDied() {
        gameOver = true;
        bgSound.Stop();

        int savedScore = PlayerPrefs.GetInt("ColorHiScore");
        if (score > savedScore) {
            PlayerPrefs.SetInt("ColorHiScore", score);
        }

        SetPageState(PageState.GameOver);
    }
    void OnPlayerScored() {
        score++;
        scoreText.text = score.ToString();
    }
}
