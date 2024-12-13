using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text campaignTimeText;

    private void Start()
    {
        string lastCampaignTime = PlayerPrefs.GetString("LastCampaignTime", "N/A");
        campaignTimeText.text = $"Last Campaign Time: {lastCampaignTime}";
    }

    public void StartGame(string mode)
    {
        PlayerPrefs.SetString("PlayMode", mode); 
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game"); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartEasyMode()
    {
        PlayerPrefs.SetString("PlayMode", "Easy");
        LoadGameScene();
    }

    public void StartMediumMode()
    {
        PlayerPrefs.SetString("PlayMode", "Medium");
        LoadGameScene();
    }

    public void StartHardMode()
    {
        PlayerPrefs.SetString("PlayMode", "Hard");
        LoadGameScene();
    }

    public void StartVeryHardMode()
    {
        PlayerPrefs.SetString("PlayMode", "VeryHard");
        LoadGameScene();
    }

    public void StartCampaignMode()
    {
        PlayerPrefs.SetString("PlayMode", "Campaign");
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Game"); 
    }
}
