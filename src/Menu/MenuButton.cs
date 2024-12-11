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

    // Phương thức này được gọi khi nhấn nút Play
    public void OnPlayButtonClick()
    {
        // Ẩn menu panel và hiển thị game panel
        SetGamePanel(true);
    }

    // Phương thức này để hiển thị hoặc ẩn GamePanel và MenuPanel
    private void SetGamePanel(bool isActive)
    {
        if (gamePanel != null)
        {
            gamePanel.SetActive(isActive);  // Nếu isActive = true thì GamePanel hiển thị
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(!isActive); // Nếu isActive = true thì MenuPanel ẩn
        }

        if (playButton != null)
        {
            playButton.SetActive(!isActive);  // Ẩn nút Play khi bắt đầu game
        }
    }
    public void OpenScoreBoard()
    {
        SceneManager.LoadScene("Scoreboard"); // Thay "Scoreboard" bằng tên scene bạn tạo
    }
}
