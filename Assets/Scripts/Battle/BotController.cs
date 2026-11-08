
using UnityEngine;
using System.Collections;

public class BotController : MonoBehaviour
{
    [Header("References")]
    public BattleGrid botGrid;
    public BattleManager battleManager;

    [Header("Settings")]
    public float thinkingTime = 1f;

    private bool isMoving;

    private readonly Vector2Int[][] shapes =
    {
        new Vector2Int[] { new Vector2Int(0, 0) },

        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0)
        },

        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0)
        },

        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1)
        },

        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        },

        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1)
        },

        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(1, 1)
        }
    };

    public void MakeMove()
    {
        if (!isMoving)
            StartCoroutine(MakeMoveCoroutine());
    }

    private IEnumerator MakeMoveCoroutine()
    {
        isMoving = true;

        yield return new WaitForSeconds(thinkingTime);

        if (battleManager == null || botGrid == null)
        {
            Debug.LogError("BotController: references are missing!");
            isMoving = false;
            yield break;
        }

        bool foundMove = false;

        int randomStart = Random.Range(0, shapes.Length);

        for (int i = 0; i < shapes.Length; i++)
        {
            int shapeIndex = (randomStart + i) % shapes.Length;
            Vector2Int[] shape = shapes[shapeIndex];

            int randomX = Random.Range(0, botGrid.gridSize);
            int randomY = Random.Range(0, botGrid.gridSize);

            for (int offset = 0; offset < botGrid.gridSize * botGrid.gridSize; offset++)
            {
                int x = (randomX + offset) % botGrid.gridSize;
                int y = (randomY + offset / botGrid.gridSize) % botGrid.gridSize;

                if (!botGrid.CanPlaceShapeAt(shape, x, y))
                    continue;

                bool bombExploded;
                int clearedLines;

                bool success = botGrid.TryPlaceShape(
                    shape,
                    x,
                    y,
                    botGrid.botColor,
                    out bombExploded,
                    out clearedLines
                );

                if (success)
                {
                    foundMove = true;
                    Debug.Log("Bot placed a piece!");

                    battleManager.RegisterBotMove(bombExploded);
                    break;
                }
            }

            if (foundMove)
                break;
        }

        if (!foundMove)
        {
            Debug.Log("Bot has no possible moves!");
            battleManager.BotHasNoMoves();
        }

        isMoving = false;
    }
}
