
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public ScoreManager scoreManager;

    [Header("Spawn Positions")]
    public Vector3[] spawnPositions =
    {
        new Vector3(-2.7f, -3.7f, 0),
        new Vector3(0f, -3.7f, 0),
        new Vector3(2.7f, -3.7f, 0)
    };

    private PieceDrag[] activePieces = new PieceDrag[3];

    private Vector2Int[][] shapes =
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
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(3, 0)
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
            new Vector2Int(0, 1),
            new Vector2Int(0, 2),
            new Vector2Int(1, 0)
        },

        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(1, 1)
        },

        new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0)
        },

        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 0)
        },

        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(2, 1),
            new Vector2Int(0, 2),
            new Vector2Int(1, 2),
            new Vector2Int(2, 2)
        }
    };

    private Color[] colors =
    {
        new Color(1f, 0.20f, 0.35f),
        new Color(0.10f, 0.75f, 1f),
        new Color(0.15f, 0.95f, 0.48f),
        new Color(1f, 0.65f, 0.10f),
        new Color(0.72f, 0.30f, 1f)
    };

    private void Start()
    {
        SpawnPieces();
    }

    private void SpawnPieces()
    {
        for (int i = 0; i < 3; i++)
        {
            CreatePiece(i);
        }

        CheckGameOver();
    }

    private void CreatePiece(int index)
    {
        GameObject pieceObject =
            new GameObject("Piece_" + index);

        pieceObject.transform.position =
            spawnPositions[index];

        PieceView view =
            pieceObject.AddComponent<PieceView>();

        Vector2Int[] shape =
            shapes[Random.Range(0, shapes.Length)];

        Color color =
            colors[Random.Range(0, colors.Length)];

        view.Setup(
            shape,
            color,
            gridManager.BlockSprite,
            gridManager.cellSize
        );

        PieceDrag drag =
            pieceObject.AddComponent<PieceDrag>();

        drag.gridManager = gridManager;
        drag.pieceSpawner = this;
        drag.slotIndex = index;

        activePieces[index] = drag;
    }

    public void PieceUsed(PieceDrag piece)
    {
        if (scoreManager != null && scoreManager.IsGameOver)
            return;

        activePieces[piece.slotIndex] = null;

        bool allUsed = true;

        foreach (PieceDrag active in activePieces)
        {
            if (active != null)
            {
                allUsed = false;
                break;
            }
        }

        // Если использованы все три фигуры,
        // сначала создаём новую тройку.
        if (allUsed)
        {
            SpawnPieces();
            return;
        }

        // Проверяем только оставшиеся фигуры.
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (scoreManager != null && scoreManager.IsGameOver)
            return;

        if (gridManager == null)
            return;

        // Если фигур пока нет, поражение не проверяем.
        bool hasActivePiece = false;

        foreach (PieceDrag active in activePieces)
        {
            if (active != null)
            {
                hasActivePiece = true;
                break;
            }
        }

        if (!hasActivePiece)
            return;

        bool hasPossibleMove = false;

        foreach (PieceDrag active in activePieces)
        {
            if (active == null)
                continue;

            PieceView view =
                active.GetComponent<PieceView>();

            if (view != null &&
                gridManager.CanPlaceShape(view.cells))
            {
                hasPossibleMove = true;
                break;
            }
        }

        if (!hasPossibleMove && scoreManager != null)
        {
            scoreManager.ShowGameOver();
        }
    }

    public void RestartPieces()
    {
        for (int i = 0; i < activePieces.Length; i++)
        {
            if (activePieces[i] != null)
                Destroy(activePieces[i].gameObject);

            activePieces[i] = null;
        }

        SpawnPieces();
    }
}
