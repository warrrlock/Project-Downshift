using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 10;
    public float lapTime;
    public float prevLapTime;
    public float fastestLap = float.MaxValue;

    private int currentLap = -1;

    private float startTime;

    public TMP_Text lapText;
    public TMP_Text laptimeText;
    public TMP_Text prevLapTimeText;
    public TMP_Text fastLapText;
    public GameObject finishText;

    private void Start()
    {
        StartNewLap();
        UpdateLapText();
    }

    private void Update()
    {
        lapTime += Time.deltaTime;

        //show current laptime
        string formattedTime = LapTimeFormat(lapTime);
        laptimeText.text = "Time: " + formattedTime;    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UpdateLap();
            Debug.Log("Lap");
        }
    }

    private void UpdateLap()
    {
        currentLap++;
        UpdateLapTimes();

        if (currentLap <= totalLaps)
        {
            UpdateLapText();
            StartNewLap();
        }
        else
        {
            ShowFinishText();
        }
    }

    private void UpdateLapText()
    {
        lapText.text = "Lap: " + currentLap + "/" + totalLaps;
    }

    private void UpdateLapTimes()
    {
        if (lapTime < fastestLap) //if u make a fastest lap
        {
            StartCoroutine(BlinkText());
            fastestLap = lapTime; //set
        }

        //update other current & previous laps
        prevLapTime = lapTime;
        string prevLapFormatted = LapTimeFormat(prevLapTime);
        prevLapTimeText.text = "P: " + prevLapFormatted;

        string fastLapFormatted = (fastestLap == float.MaxValue) ? "0:00:00" : LapTimeFormat(fastestLap);
        fastLapText.text = "F: " + fastLapFormatted;
    }

    private void ShowFinishText()
    {
        Debug.Log("Finished");
        finishText.SetActive(true);
    }

    private void StartNewLap()
    {
        lapTime = 0f;
        startTime = Time.time;
    }

    private string LapTimeFormat(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    private IEnumerator BlinkText()
    {
        WaitForSeconds blinkDuration = new WaitForSeconds(0.2f); // Adjust the blink duration as needed
        int blinkCount = 5; // Adjust the blink count as needed

        for (int i = 0; i < blinkCount; i++)
        {
            fastLapText.enabled = false;
            yield return blinkDuration;
            fastLapText.enabled = true;
            yield return blinkDuration;
        }
    }

}
