using System;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [Header("TextMeshPro Fields")]
    [SerializeField] private TMP_Text[] bestTimeTexts;
    [SerializeField] private TMP_Text[] playCountTexts;
    [SerializeField] private TMP_Text[] successTexts;

    /// <summary>
    /// Updates the scoreboard with best times, play counts, and success stats.
    /// </summary>
    /// <param name="bestTimes">Array of best times</param>
    /// <param name="playCounts">Array of play counts</param>
    /// <param name="successStats">Array of success stats</param>
    public void UpdateScoreBoard(string[] bestTimes, int[] playCounts, string[] successStats)
    {
        UpdateBestTimes(bestTimes);
        UpdatePlayCounts(playCounts);
        UpdateSuccessStats(successStats);
    }

    /// <summary>
    /// Updates the best time text fields.
    /// </summary>
    /// <param name="bestTimes">Array of best times</param>
    private void UpdateBestTimes(string[] bestTimes)
    {
        UpdateTextArray(bestTimeTexts, bestTimes, "-");
    }

    /// <summary>
    /// Updates the play count text fields.
    /// </summary>
    /// <param name="playCounts">Array of play counts</param>
    private void UpdatePlayCounts(int[] playCounts)
    {
        UpdateTextArray(playCountTexts, playCounts, "-");
    }

    /// <summary>
    /// Updates the success stats text fields.
    /// </summary>
    /// <param name="successStats">Array of success stats</param>
    private void UpdateSuccessStats(string[] successStats)
    {
        UpdateTextArray(successTexts, successStats, "-");
    }

    /// <summary>
    /// Updates an array of TMP_Text fields with corresponding values from the data array.
    /// </summary>
    /// <param name="textArray">Array of TMP_Text fields to update</param>
    /// <param name="dataArray">Array of data to update from</param>
    /// <param name="defaultValue">Default value to set if data is missing</param>
    private void UpdateTextArray(TMP_Text[] textArray, Array dataArray, string defaultValue)
    {
        int count = Mathf.Min(textArray.Length, dataArray.Length);
        for (int i = 0; i < count; i++)
        {
            textArray[i].text = dataArray.GetValue(i)?.ToString() ?? defaultValue;
        }

        // Ensure remaining textArray items are set to the default value
        for (int i = count; i < textArray.Length; i++)
        {
            textArray[i].text = defaultValue;
        }
    }
}
