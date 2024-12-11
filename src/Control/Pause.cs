using UnityEngine;

public class PauseManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera pauseCamera;
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;
    #endregion

    #region Initialization
    private void Start()
    {
        // Set initial states for the cameras and pause panel
        mainCamera.enabled = true;
        pauseCamera.enabled = false;
        pausePanel.SetActive(false);
    }
    #endregion

    #region Pause Logic
    public void TogglePause()
    {
        // Toggle the pause state
        isPaused = !isPaused;

        if (isPaused)
        {
            // Activate pause state: switch cameras, show pause panel, and freeze time
            mainCamera.enabled = false;
            pauseCamera.enabled = true;
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            // Deactivate pause state: return to main camera, hide pause panel, and resume time
            mainCamera.enabled = true;
            pauseCamera.enabled = false;
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    #endregion
}
