using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 10;

    private int currentLap = 0;
    private float lapTime;
    private float startTime;

    public TMP_Text lapText;
    public GameObject finishText;

    private void Start()
    {
        UpdateLapText();
    }

    private void Update()
    {
        lapTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has the "player" tag
        if (other.CompareTag("Player"))
        {
            // Assuming your start/finish line has a trigger collider
            UpdateLap();
            Debug.Log("Lap");
        }
    }

    private void UpdateLap()
    {
        currentLap++;

        if (currentLap <= totalLaps)
        {
            UpdateLapText();
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

    private void ShowFinishText()
    {
        Debug.Log("Finished");
        finishText.SetActive(true);
    }

}
