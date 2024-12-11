/*
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text hintsText;
    [SerializeField] private TMP_Text levelText;

    public enum GameMode
    {
        Play,
        Campaign
    }

    public int CurrentLevel { get; private set; }
    public int CurrentLives { get; private set; }
    public int RemainingHints { get; private set; }
    public int[,] CurrentGrid { get; private set; }
    public GameMode CurrentMode { get; private set; }
    public float CampaignStartTime { get; private set; }
    public float TotalTimeElapsed { get; private set; }
    public float CurrentLevelStartTime { get; private set; }

    private const int MaxLives = 3;
    private const int MaxHints = 3;

    public void Initialize(GameMode mode)
    {
        CurrentMode = mode;
        CurrentLevel = 1;
        CurrentLives = MaxLives;
        RemainingHints = MaxHints;
        TotalTimeElapsed = 0f;

        if (mode == GameMode.Campaign)
        {
            CampaignStartTime = Time.time;
        }

        StartNewLevel();
    }

    public void UpdateUI()
    {
        livesText.text = $"Lives: {CurrentLives}/3";
        hintsText.text = $"Hints: {RemainingHints}/3";

        if (CurrentMode == GameMode.Campaign)
        {
            levelText.text = $"Floor: {CurrentLevel}";
        }
        else
        {     
            string difficulty = GetDifficultyString(GetDifficultyLevel());
            levelText.text = difficulty;
        }
    }

    private string GetDifficultyString(Generator.DifficultyLevel difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case Generator.DifficultyLevel.EASY: return "EASY";
            case Generator.DifficultyLevel.MEDIUM: return "MEDIUM";
            case Generator.DifficultyLevel.HARD: return "HARD";
            case Generator.DifficultyLevel.VERY_HARD: return "VERY HARD";
            case Generator.DifficultyLevel.EXTREME: return "EXTREME";
            case Generator.DifficultyLevel.CAMPAIGN: return "CAMPAIGN";
            default: return "UNKNOWN";
        }
    }

    public void StartNewLevel()
    {
        CurrentGrid = Generator.GeneratePuzzle(GetDifficultyLevel());
        CurrentLevelStartTime = Time.time;
        UpdateUI();
    }

    public void ResetCurrentLevel()
    {
        StartNewLevel();
    }

    public void LevelCompleted()
    {
        if (CurrentMode == GameMode.Campaign)
        {
            if (CurrentLevel % 3 == 0 || CurrentLevel % 5 == 0)
            {
                CurrentLives = Mathf.Min(CurrentLives + 1, MaxLives);
            }
        }

        CurrentLevel++;
        StartNewLevel();
    }

    public bool UseHint()
    {
        if (RemainingHints > 0)
        {
            RemainingHints--;
            return true;
        }

        return false;
    }

    public void LoseLife()
    {
        CurrentLives--;
        if (CurrentLives <= 0)
        {
            EndGame();
        }
        UpdateUI();
    }

    public void EndGame()
    {
        if (CurrentMode == GameMode.Campaign)
        {
            TotalTimeElapsed = Time.time - CampaignStartTime;
        }

        UIManager.Instance.ShowMainMenu(CurrentMode == GameMode.Campaign ? TotalTimeElapsed : -1);
    }

    public Generator.DifficultyLevel GetDifficultyLevel()
    {
        if (CurrentMode == GameMode.Play)
        {
            switch (PlayerPrefs.GetString("PlayMode"))
            {
                case "Easy": return Generator.DifficultyLevel.EASY;
                case "Medium": return Generator.DifficultyLevel.MEDIUM;
                case "Hard": return Generator.DifficultyLevel.HARD;
                case "VeryHard": return Generator.DifficultyLevel.VERY_HARD;
                default: return Generator.DifficultyLevel.EASY; 
            }
        }
        else if (CurrentMode == GameMode.Campaign)
        {
            if (CurrentLevel <= 3) return Generator.DifficultyLevel.EASY;
            if (CurrentLevel <= 6) return Generator.DifficultyLevel.MEDIUM;
            if (CurrentLevel <= 9) return Generator.DifficultyLevel.HARD;
            return Generator.DifficultyLevel.EXTREME;
        }

        return Generator.DifficultyLevel.EASY;
    }
    public void LoadNextLevel()
    {
        LevelCompleted();
    }
}
*/
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text hintsText;

    public enum GameMode
    {
        Play,
        Campaign
    }

    private const int MaxLives = 3;
    private const int MaxHints = 3;

    public int CurrentLevel { get; private set; }
    public int CurrentLives { get; private set; }
    public int RemainingHints { get; private set; }
    public int[,] CurrentGrid { get; private set; }
    public GameMode CurrentMode { get; private set; }

    public float TotalTimeElapsed { get; private set; }  
    public float CampaignStartTime { get; private set; }

    private void Awake()
    {
        CurrentLevel = 1;
        CurrentLives = MaxLives;
        RemainingHints = MaxHints;
    }

    public void Initialize(GameMode mode)
    {
        CurrentMode = mode;
        if (CurrentMode == GameMode.Campaign)
        {
            CampaignStartTime = Time.time;
        }

        StartNewLevel();
    }

    public void StartNewLevel()
    {
        CurrentGrid = Generator.GeneratePuzzle(GetDifficultyLevel());
        UpdateUI();
    }

    public void ResetCurrentLevel()
    {
        StartNewLevel();
    }

    public void LevelCompleted()
    {
        CurrentLevel++;

        if (CurrentMode == GameMode.Campaign && (CurrentLevel % 3 == 0 || CurrentLevel % 5 == 0))
        {
            CurrentLives = Mathf.Min(CurrentLives + 1, MaxLives);
        }

        StartNewLevel(); 
    }

    public void LoadNextLevel()
    { 
        LevelCompleted();
    }

    public void LoseLife()
    {
        CurrentLives--;
        UpdateUI();

        if (CurrentLives <= 0)
        {
            GameControl.Instance.GameOver(); 
        }
    }

    public void UseHint()
    {
        if (RemainingHints > 0)
        {
            RemainingHints--;
            UpdateUI();
        }
    }

    public Generator.DifficultyLevel GetDifficultyLevel()
    {
        if (CurrentMode == GameMode.Play)
        {
            switch (PlayerPrefs.GetString("PlayMode"))
            {
                case "Easy": return Generator.DifficultyLevel.EASY;
                case "Medium": return Generator.DifficultyLevel.MEDIUM;
                case "Hard": return Generator.DifficultyLevel.HARD;
                case "VeryHard": return Generator.DifficultyLevel.VERY_HARD;
                default: return Generator.DifficultyLevel.EASY;
            }
        }
        else if (CurrentMode == GameMode.Campaign)
        {
            if (CurrentLevel <= 3) return Generator.DifficultyLevel.EASY;
            if (CurrentLevel <= 6) return Generator.DifficultyLevel.MEDIUM;
            if (CurrentLevel <= 9) return Generator.DifficultyLevel.HARD;
            return Generator.DifficultyLevel.EXTREME;
        }

        return Generator.DifficultyLevel.EASY;
    }

    public void UpdateUI()
    {
        livesText.text = $"Lives: {CurrentLives}/{MaxLives}";
        hintsText.text = $"Hints: {RemainingHints}/{MaxHints}";
        levelText.text = CurrentMode == GameMode.Campaign ? $"Floor: {CurrentLevel}" : $"Level: {CurrentLevel}";
    }

    public void UpdateTotalTime()
    {
        if (CurrentMode == GameMode.Campaign)
        {
            TotalTimeElapsed = Time.time - CampaignStartTime;
        }
    }
}
