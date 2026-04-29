using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    public float timeRemaining = 10;
    [HideInInspector] public float totalTime;
    public bool timerIsRunning = false;
    [SerializeField] TextMeshProUGUI timeText;

    // Start is called before the first frame update
    public void InitTimer()
    {
        timeRemaining = totalTime;
        DisplayTime(timeRemaining-1);
        timerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = -1;
                timerIsRunning = false;
                if (Loader.Instance.currLevel == 1)
                    TutorialManager.Instance.OnGameOverPanel(false);
                else
                    GameManager.Instance.OnGameOverPanel(false);
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public string GetTime()
    {
        timerIsRunning = false;
        timeRemaining += 1;
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
