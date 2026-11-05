using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text comboText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text bestScoreText;

    [Header("References")]
    public GridManager gridManager;
    public PieceSpawner pieceSpawner;

    private int score;
    private int combo;
    private int bestScore;

    public bool IsGameOver { get; private set; }

    private void Start()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        IsGameOver = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateUI();
    }

    public void RegisterMove(int placedBlocks, int clearedLines)
    {
        if (IsGameOver)
            return;

        score += placedBlocks;

        if (clearedLines > 0)
        {
            combo++;

            int multiplier = Mathf.Max(1, combo);

            score += clearedLines * 10 * multiplier;
        }
        else
        {
            combo = 0;
        }

        if (score > bestScore)
        {
            bestScore = score;

            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();

        if (comboText != null)
        {
            comboText.text = combo >= 2
                ? "COMBO x" + combo
                : "";
        }
    }

    public void ShowGameOver()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;

        if (finalScoreText != null)
            finalScoreText.text = score.ToString();

        if (bestScoreText != null)
            bestScoreText.text = bestScore.ToString();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        IsGameOver = false;

        score = 0;
        combo = 0;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateUI();

        if (gridManager != null)
            gridManager.ResetBoard();

        if (pieceSpawner != null)
            pieceSpawner.RestartPieces();
    }
}