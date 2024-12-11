/*
 * using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _livesText;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _hintText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private GameObject _pausePanel;

    public static UIManager Instance { get; private set; }

    public void Initialize()
    {
        UpdateLives(GameControl.Instance.LevelManager.CurrentLives);
        UpdateLevel(GameControl.Instance.LevelManager.CurrentLevel);
    }

    public void UpdateLives(int currentLives)
    {
        _livesText.text = $"Lives: {currentLives}/3";
    }

    public void UpdateLevel(int level)
    {
        if (GameControl.Instance.LevelManager.CurrentMode == LevelManager.GameMode.Campaign)
        {
            _levelText.text = $"LEVEL: {level}";
        }
        else
        {
            _levelText.text = $"{GameControl.Instance.LevelManager.GetDifficultyLevel()}";
        }
    }

    public void ShowPauseMenu(bool isPaused)
    {
        _pausePanel.SetActive(isPaused);
    }

    public void ShowMainMenu(float campaignTime = -1)
    {
        if (campaignTime >= 0)
        {
            Debug.Log($"Total Campaign Time: {campaignTime} seconds");
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void UpdateHints(int hints)
    {
        _hintText.text = $"Lives: {hints}/3";
    }

    public void UpdateTimer(float timeElapsed)
    {
        System.TimeSpan time = System.TimeSpan.FromSeconds(timeElapsed);
        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);

        _timerText.text = formattedTime;
    }

    public void ShowGameOverScreen()
    {
        Debug.Log("Game Over! Display Game Over screen.");

    }

}
*/
using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // Singleton Instance

    [Header("UI References")]
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text hintsText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gamePanel;

    public event Action OnGameOver;  // Observer Event for Game Over
    public event Action OnPause;    // Observer Event for Pause
    public event Action OnResume;   // Observer Event for Resume

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
        ShowMainMenu();
    }

    public void Initialize()
    {
        
    }

    public void UpdateLives(int lives)
    {
        livesText.text = $"Lives: {lives}/3";
    }

    public void UpdateHints(int hints)
    {
        hintsText.text = $"Hints: {hints}/3";
    }

    public void UpdateLevel(int level, bool isCampaign)
    {
        levelText.text = isCampaign ? $"Floor: {level}" : $"Level: {level}";
    }

    public void ShowPauseMenu(bool show)
    {
        pauseMenu.SetActive(true);
        gamePanel.SetActive(false);
        OnPause?.Invoke(); 
    }

    public void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
        gamePanel.SetActive(true);
        OnResume?.Invoke();
    }

    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        gamePanel.SetActive(false);
        OnGameOver?.Invoke(); 
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        gamePanel.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        gamePanel.SetActive(true);
        gameOverScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
