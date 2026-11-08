
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("Player")]
    public int playerHP = 100;
    public int playerPiecesPlaced = 0;
    public BattleGrid playerGrid;

    [Header("Bot")]
    public int botHP = 100;
    public int botPiecesPlaced = 0;
    public BattleGrid botGrid;
    public BotController botController;

    [Header("UI")]
    public TMP_Text playerHPText;
    public TMP_Text botHPText;
    public TMP_Text turnText;
    public BattleGameOverUI gameOverUI;

    public bool IsPlayerTurn { get; private set; } = true;

    private bool gameOver;

    private void Start()
    {
        Time.timeScale = 1f;
        IsPlayerTurn = true;
        UpdateUI();

        if (turnText != null)
            turnText.text = "YOUR TURN";
    }

    public void RegisterPlayerMove(bool bombExploded)
    {
        if (gameOver || !IsPlayerTurn)
            return;

        playerPiecesPlaced++;

        if (bombExploded)
            DamageBot(10);

        if (gameOver)
            return;

        if (playerPiecesPlaced % 10 == 0)
            playerGrid.SpawnBomb();

        IsPlayerTurn = false;

        if (turnText != null)
            turnText.text = "Попытка бота";

        UpdateUI();

        if (botController != null)
            botController.MakeMove();
        else
            Debug.LogError("BotController is not connected!");
    }

    public void RegisterBotMove(bool bombExploded)
    {
        if (gameOver)
            return;

        botPiecesPlaced++;

        if (bombExploded)
            DamagePlayer(10);

        if (gameOver)
            return;

        if (botPiecesPlaced % 10 == 0)
            botGrid.SpawnBomb();

        IsPlayerTurn = true;

        if (turnText != null)
            turnText.text = "Твой ход";

        UpdateUI();
    }

    public void PlayerHasNoMoves()
    {
        if (!gameOver)
            EndGame(false);
    }

    public void BotHasNoMoves()
    {
        if (!gameOver)
            EndGame(true);
    }

    public void DamagePlayer(int damage)
    {
        if (gameOver)
            return;

        playerHP = Mathf.Max(0, playerHP - damage);
        UpdateUI();

        if (playerHP <= 0)
            EndGame(false);
    }

    public void DamageBot(int damage)
    {
        if (gameOver)
            return;

        botHP = Mathf.Max(0, botHP - damage);
        UpdateUI();

        if (botHP <= 0)
            EndGame(true);
    }

    private void UpdateUI()
    {
        if (playerHPText != null)
            playerHPText.text = "Ты: " + playerHP + " HP";

        if (botHPText != null)
            botHPText.text = "Бот: " + botHP + " HP";
    }

    private void EndGame(bool victory)
    {
        if (gameOver)
            return;

        gameOver = true;
        IsPlayerTurn = false;

        if (turnText != null)
            turnText.text = victory ? "Ты подебил чушпан!" : "Ты проиграл!";

        if (gameOverUI != null)
            gameOverUI.ShowResult(victory);
        else
            Debug.LogError("BattleGameOverUI is not connected!");
    }
}
