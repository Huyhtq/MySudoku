using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject playButton;

    public void OnPlayButtonClick()
    {
        SetGamePanel(true);
    }

    private void SetGamePanel(bool isActive)
    {
        if (gamePanel != null)
        {
            gamePanel.SetActive(isActive);  
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(!isActive); 
        }

        if (playButton != null)
        {
            playButton.SetActive(!isActive);  
        }
    }
    public void OpenScoreBoard()
    {
        SceneManager.LoadScene("Scoreboard"); 
    }
}
