using UnityEngine;

public class GameState : MonoBehaviour
{
    #region Fields
    public bool IsPaused { get; private set; }
    #endregion

    #region Pause Logic
    public void TogglePause()
    {
        // Toggle the pause state and adjust the time scale
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }
    #endregion
}
