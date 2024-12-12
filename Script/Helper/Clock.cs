using System;
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    #region Fields
    private int hour = 0;
    private int minute = 0;
    private int second = 0;

    private TMP_Text textClock;
    private float elapsedTime = 0f;
    private bool stopClock = false;
    #endregion

    #region Initialization
    private void Awake()
    {
        // Get reference to the TMP_Text component for clock display
        textClock = GetComponent<TMP_Text>();
    }

    void Start()
    {
        // Reset clock when game starts
        stopClock = false;
        elapsedTime = 0f;
    }
    #endregion

    #region Clock Logic
    void Update()
    {
        if (!stopClock)
        {
            elapsedTime += Time.deltaTime;

            // Update time
            TimeSpan span = TimeSpan.FromSeconds(elapsedTime);
            hour = span.Hours;
            minute = span.Minutes;
            second = span.Seconds;

            // Update the clock display
            textClock.text = FormatTime(hour) + ":" + FormatTime(minute) + ":" + FormatTime(second);
        }
    }

    /// <summary>
    /// Formats the time to always display two digits.
    /// </summary>
    /// <param name="n">The number to format.</param>
    /// <returns>String representation with leading zero if necessary.</returns>
    private string FormatTime(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }
    #endregion

    #region Game Control
    public void OnGameOver()
    {
        // Stop the clock when the game is over
        stopClock = true;
    }

    public void OnStartGame()
    {
        // Start or reset the clock when the game starts
        stopClock = false;
        elapsedTime = 0f;
    }

    public void StopClock()
    {
        // Explicitly stop the clock
        stopClock = true;
    }

    public void ContinueClock()
    {
        // Resume the clock
        stopClock = false;
    }
    #endregion
}
