using UnityEngine;

public class GameControl : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private GameState _gameState;

    public static GameControl Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeGame();
    }

    /// <summary>
    /// Initializes the game components at the start of the game.
    /// </summary>
    private void InitializeGame()
    {
        if (_levelManager == null || _gridManager == null || _uiManager == null || _gameState == null)
        {
            Debug.LogError("Missing references in GameControl!");
            return;
        }

        // Get the current mode from PlayerPrefs
        string mode = PlayerPrefs.GetString("PlayMode", "Play");

        // Initialize LevelManager based on selected game mode
        if (mode == "Campaign")
        {
            _levelManager.Initialize(LevelManager.GameMode.Campaign);
        }
        else
        {
            _levelManager.Initialize(LevelManager.GameMode.Play);
        }

        // Initialize Grid and UI
        _gridManager.Initialize(_levelManager.CurrentGrid);
        _uiManager.Initialize();

        // Update UI
        UpdateUI();
    }

    /// <summary>
    /// Updates the UI elements with current game status.
    /// </summary>
    private void UpdateUI()
    {
        _uiManager.UpdateLives(_levelManager.CurrentLives);
        _uiManager.UpdateLevel(_levelManager.CurrentLevel, _levelManager.CurrentMode == LevelManager.GameMode.Campaign);
        _uiManager.UpdateHints(_levelManager.RemainingHints);
    }

    /// <summary>
    /// Restarts the current game level.
    /// </summary>
    public void RestartGame()
    {
        if (_levelManager == null || _gridManager == null || _uiManager == null)
        {
            Debug.LogError("Cannot restart the game due to missing components!");
            return;
        }

        _levelManager.ResetCurrentLevel();
        _gridManager.Initialize(_levelManager.CurrentGrid);
        UpdateUI();
    }

    /// <summary>
    /// Toggles the pause state of the game.
    /// </summary>
    public void TogglePause()
    {
        if (_gameState == null || _uiManager == null)
        {
            Debug.LogError("Cannot toggle pause due to missing components!");
            return;
        }

        _gameState.TogglePause();
        _uiManager.ShowPauseMenu(_gameState.IsPaused);
        Time.timeScale = _gameState.IsPaused ? 0f : 1f; // Freeze game time when paused
    }

    /// <summary>
    /// Proceeds to the next level.
    /// </summary>
    public void NextLevel()
    {
        if (_levelManager == null || _gridManager == null || _uiManager == null)
        {
            Debug.LogError("Cannot proceed to next level due to missing components!");
            return;
        }

        _levelManager.LoadNextLevel();
        _gridManager.Initialize(_levelManager.CurrentGrid);
        UpdateUI();
    }

    /// <summary>
    /// Ends the game and shows the game over UI.
    /// </summary>
    public void GameOver()
    {
        if (_uiManager == null)
        {
            Debug.LogError("Cannot show game over screen due to missing UIManager!");
            return;
        }

        _uiManager.ShowGameOverScreen();
        Time.timeScale = 0f; // Freeze game time when game over
    }

    /// <summary>
    /// Set the game mode (Play or Campaign) and save it to PlayerPrefs.
    /// </summary>
    public void SetGameMode(string mode)
    {
        PlayerPrefs.SetString("PlayMode", mode);
    }
}
