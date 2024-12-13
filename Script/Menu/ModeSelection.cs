using UnityEngine;
using UnityEngine.SceneManagement;
/*
public class ModeSelector : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject normalModeButton;
    [SerializeField] private GameObject campaignModeButton;

    private enum GameMode
    {
        Normal,
        Campaign
    }

    private GameMode currentGameMode;
    public void SelectNormalMode()
    {
        currentGameMode = GameMode.Normal;
        ShowDifficultyOptions(true); 
    }

    public void SelectCampaignMode()
    {
        currentGameMode = GameMode.Campaign;
        ShowDifficultyOptions(false); 
        StartCampaign();
    }

    private void ShowDifficultyOptions(bool show)
    {
        normalModeButton.SetActive(show);
        campaignModeButton.SetActive(!show);
    }

    public void StartGame(string difficulty)
    {
        if (currentGameMode == GameMode.Normal)
        {
            PlayerPrefs.SetString("PlayMode", difficulty); 
            SceneManager.LoadScene("GameScene"); 
        }
    }

    private void StartCampaign()
    {
        PlayerPrefs.SetString("PlayMode", "Campaign");
        SceneManager.LoadScene("GameScene"); 
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
*/

public class ModeSelector : MonoBehaviour
{
    public void SelectNormalMode()
    {
        SceneManager.LoadScene("DifficultySelectScene"); 
    }

    public void SelectCampaignMode()
    {
        PlayerPrefs.SetString("GameMode", "Campaign");
        SceneManager.LoadScene("GameScene"); 
    }
}
