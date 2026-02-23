using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class StarManagerLevel10 : MonoBehaviour
{
    public static StarManagerLevel10 Instance;

    [Header("Timer")]
    public float levelTime = 180f;
    private float currentTime;
    public TMP_Text timerText;

    [Header("Panels")]
    public GameObject endPanel;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Objective")]
    public int totalTrashCount;
    private int collectedTrashCount;

    private int consecutiveWrong = 0;
    private bool gameEnded = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = levelTime;
    }

    void Update()
    {
        if (gameEnded) return;

        currentTime -= Time.deltaTime;
        timerText.text = Mathf.Ceil(currentTime).ToString();

        if (currentTime <= 0)
        {
            LoseGame();
        }
    }

    public void TrashCollected()
    {
        collectedTrashCount++;

        if (collectedTrashCount >= totalTrashCount)
        {
            GameCompleted();
        }
    }

    public void WrongAnswer()
    {
        consecutiveWrong++;
        currentTime -= 5f;

        if (consecutiveWrong >= 3)
        {
            LoseGame();
        }
    }

    public void CorrectAnswer()
    {
        consecutiveWrong = 0;
    }

    void GameCompleted()
    {
        gameEnded = true;
        endPanel.SetActive(true);
    }

    public void ProceedToWin()
    {
        endPanel.SetActive(false);
        winPanel.SetActive(true);
    }

    void LoseGame()
    {
        gameEnded = true;
        losePanel.SetActive(true);
    }

    public void AddTime(float amount)
    {
        currentTime += amount;
    }
}