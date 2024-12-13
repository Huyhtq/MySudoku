using UnityEngine;

public class GameControl : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private GameState _gameState;

    public static GameControl Instance { get; private set; }

    private Cell selectedCell;
    private bool hasGameFinished = false;
    private int selectedNumber = 0;

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

    private void Update()
    {
        if (hasGameFinished || !Input.GetMouseButton(0)) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        Cell tempCell;

        if (hit
            && hit.collider.gameObject.TryGetComponent(out tempCell)
            && tempCell != selectedCell
            && tempCell.State != Cell.CellState.Locked)
        {
            ResetGrid();
            selectedCell = tempCell;
            HighlightSelectedCell();

            if (selectedNumber > 0)
            {
                PerformCellAction(selectedCell, selectedNumber);
            }
        }
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
        _uiManager.UpdateHints(_levelManager.RemainingHints);
        _uiManager.UpdateLevel(_levelManager.CurrentLevel, _levelManager.CurrentMode == LevelManager.GameMode.Campaign);
    }

    /// <summary>
    /// Perform specific actions when a cell is clicked.
    /// </summary>
    private void PerformCellAction(Cell cell, int value)
    {
        bool isCorrect = _gridManager.ProcessCell(cell.Row, cell.Col);

        if (isCorrect)
        {
            UpdateUI();
        }
        else
        {
            LoseLife();
        }
    }

    /// <summary>
    /// Handles the logic for losing a life.
    /// </summary>
    public void LoseLife()
    {
        _levelManager.LoseLife();
        UpdateUI();

        if (_levelManager.CurrentLives <= 0)
        {
            hasGameFinished = true;

            if (_levelManager.CurrentMode == LevelManager.GameMode.Campaign)
            {
                _levelManager.UpdateTotalTime();
                _uiManager.ShowMainMenu(_levelManager.TotalTimeElapsed);
            }
            else
            {
                _uiManager.ShowMainMenu();
            }
        }
    }

    /// <summary>
    /// Resets the grid (e.g., removes previous highlights).
    /// </summary>
    private void ResetGrid()
    {
        _gridManager.ResetGrid();
    }

    /// <summary>
    /// Highlights the currently selected cell.
    /// </summary>
    private void HighlightSelectedCell()
    {
        if (selectedCell != null)
        {
            selectedCell.Highlight();
        }
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

        if (_levelManager.CurrentMode == LevelManager.GameMode.Campaign)
        {
            _levelManager.UpdateTotalTime();
        }

        Time.timeScale = 0f; // Freeze game time when game over
    }

    /// <summary>
    /// Set the game mode (Play or Campaign) and save it to PlayerPrefs.
    /// </summary>
    public void SetGameMode(string mode)
    {
        if (mode == "Campaign" || mode == "Easy" || mode == "Medium" || mode == "Hard" || mode == "VeryHard")
        {
            PlayerPrefs.SetString("PlayMode", mode);
        }
        else
        {
            Debug.LogWarning($"Invalid mode: {mode}");
        }

        Debug.Log($"Current Mode: {_levelManager.CurrentMode}, Current Level: {_levelManager.CurrentLevel}");

    }

    /// <summary>
    /// Sets the selected number when a number button is clicked.
    /// </summary>
    public void SetSelectedNumber(int number)
    {
        selectedNumber = number;
    }

    public void UpdateCellValue(int newValue)
    {
        if (hasGameFinished || selectedCell == null) return;

        selectedCell.UpdateValue(newValue);

        if (!IsValid(selectedCell, _gridManager.cells))
        {
            _levelManager.LoseLife();
        }

        Highlight();
        CheckWin();
        Debug.Log("Update value " + newValue);
    }

    private bool IsValid(Cell cell, Cell[,] cells)
    {
        int row = cell.Row;
        int col = cell.Col;
        int value = cell.Value;

        if (value == 0) return true;

        for (int i = 0; i < 3; i++)
        {
            if (cells[row, i].Value == value || cells[i, col].Value == value)
            {
                return false;
            }
        }

        int subGridRow = row - row % 3;
        int subGridCol = col - col % 3;

        for (int r = subGridRow; r < subGridRow + 3; r++)
        {
            for (int c = subGridCol; c < subGridCol + 3; c++)
            {
                if (cells[r, c].Value == value)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Highlights the grid based on the current state.
    /// This will highlight cells based on correctness and selection.
    /// </summary>
    private void Highlight()
    {
        for (int i = 0; i < _gridManager.GetGridSize(); i++)
        {
            for (int j = 0; j < _gridManager.GetGridSize(); j++)
            {
                Cell cell = _gridManager.cells[i, j];

                // Highlight cells that are incorrect
                if (!IsValid(cell, _gridManager.cells))
                {
                    cell.Highlight();
                }
                else
                {
                    cell.Highlight();
                }

                // Highlight the selected row and column
                if (cell.Row == selectedCell.Row || cell.Col == selectedCell.Col)
                {
                    cell.Highlight();
                }
            }
        }

        // Highlight the selected cell itself
        selectedCell.Highlight();
    }

    private void CheckWin()
    {
        if (_gridManager.IsGridSolved())
        {
            hasGameFinished = true;
        }
    }
}
