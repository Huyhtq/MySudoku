using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelector : MonoBehaviour
{
    public void SelectDifficulty(string difficulty)
    {
        PlayerPrefs.SetString("Difficulty", difficulty);  
        SceneManager.LoadScene("GameScene");  
    }
}
