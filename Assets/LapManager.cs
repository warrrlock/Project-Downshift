using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 3;
    private int currentLap = 0;

    public TMP_Text lapText;
    public TMP_Text finishText;

    private void Start()
    {
        UpdateLapText();
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
        finishText.IsActive();
    }

}
