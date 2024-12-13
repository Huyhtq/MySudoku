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
        gamePanel.SetActive(true);
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

    public void ShowMainMenu(float campaignTime = -1)
    {
        if (campaignTime >= 0)
        {
            TimeSpan time = TimeSpan.FromSeconds(campaignTime);
            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);

            PlayerPrefs.SetString("LastCampaignTime", formattedTime);
            Debug.Log($"Total Campaign Time Saved: {formattedTime}");
        }

        
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); 
    }

}
