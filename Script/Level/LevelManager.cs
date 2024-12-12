using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void InitializeFromPlayerPrefs()
    {
        string mode = PlayerPrefs.GetString("PlayMode", "Easy");

        if (mode == "Campaign")
        {
            Initialize(GameMode.Campaign);
        }
        else
        {
            Initialize(GameMode.Play);
        }
    }

    public void StartNewLevel()
    {
        Generator.DifficultyLevel difficulty = GetDifficultyLevel();
        CurrentGrid = Generator.GeneratePuzzle(difficulty);

        if (CurrentMode == GameMode.Campaign)
        {
            levelText.text = $"Floor: {CurrentLevel}";
        }
        else
        {
            levelText.text = GetDifficultyString(difficulty);
        }

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
            CurrentLevel++;
            if (CurrentLevel % 3 == 0 || CurrentLevel % 5 == 0)
            {
                CurrentLives = Mathf.Min(CurrentLives + 1, MaxLives);
            }

            StartNewLevel();
        }
        else
        {
            UIManager.Instance.ShowMainMenu(); 
        }
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
            SceneManager.LoadScene("MainMenu");
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
            return PlayerPrefs.GetString("PlayMode") switch
            {
                "Easy" => Generator.DifficultyLevel.EASY,
                "Medium" => Generator.DifficultyLevel.MEDIUM,
                "Hard" => Generator.DifficultyLevel.HARD,
                "VeryHard" => Generator.DifficultyLevel.VERY_HARD,
                "Campaign" => Generator.DifficultyLevel.CAMPAIGN,
                _ => Generator.DifficultyLevel.EASY,
            };
        }
        else if (CurrentMode == GameMode.Campaign)
        {
            return CurrentLevel switch
            {
                <= 3 => Generator.DifficultyLevel.EASY,
                <= 6 => Generator.DifficultyLevel.MEDIUM,
                <= 9 => Generator.DifficultyLevel.HARD,
                _ => Generator.DifficultyLevel.EXTREME,
            };
        }

        return Generator.DifficultyLevel.EASY;
    }


    public void UpdateUI()
    {
        livesText.text = $"Lives: {CurrentLives}/{MaxLives}";
        hintsText.text = $"Hints: {RemainingHints}/{MaxHints}";

        if (CurrentMode == GameMode.Campaign)
        {
            levelText.text = $"Floor: {CurrentLevel}";
        }
        else
        {
            levelText.text = GetDifficultyString(GetDifficultyLevel());
        }
    }


    public void UpdateTotalTime()
    {
        if (CurrentMode == GameMode.Campaign)
        {
            TotalTimeElapsed = Time.time - CampaignStartTime;
        }
    }

    public string GetDifficultyString(Generator.DifficultyLevel difficultyLevel)
    {
        return difficultyLevel switch
        {
            Generator.DifficultyLevel.EASY => "EASY",
            Generator.DifficultyLevel.MEDIUM => "MEDIUM",
            Generator.DifficultyLevel.HARD => "HARD",
            Generator.DifficultyLevel.VERY_HARD => "VERY HARD",
            Generator.DifficultyLevel.EXTREME => "EXTREME",
            Generator.DifficultyLevel.CAMPAIGN => "CAMPAIGN",
            _ => "UNKNOWN",
        };
    }


}
