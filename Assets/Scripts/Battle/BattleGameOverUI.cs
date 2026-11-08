
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleGameOverUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject gameOverPanel;

    [Header("Text")]
    public TMP_Text resultText;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void ShowResult(bool victory)
    {
        if (gameOverPanel == null)
        {
            Debug.LogError("Game Over Panel is not connected!");
            return;
        }

        gameOverPanel.SetActive(true);

        if (resultText != null)
        {
            resultText.text = victory ? "ПОБЕДАААА!" : "Поражение хнык!";
        }

        Time.timeScale = 0f;
    }

    public void RestartBattle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BattleSce   ne");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
