using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemain = 360f;
    public Text timerText;
    bool timerIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        // Start the timer on start
        timerIsRunning = true;    
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            if(timeRemain > 0)
            {
                timeRemain -= Time.deltaTime;
                DisplayTime(timeRemain);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemain = 0;
                timerIsRunning = false;
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
