
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text bestScoreText;

    [Header("Scene")]
    public string gameSceneName = "GameScene";

    private void Start()
    {
        UpdateBestScore();
    }

    public void UpdateBestScore()
    {
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (bestScoreText != null)
        {
            bestScoreText.text = bestScore.ToString();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked!");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
