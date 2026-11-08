
using UnityEngine;

public class BattlePieceSpawner : MonoBehaviour
{
    [Header("References")]
    public BattleGrid playerGrid;
    public BattleManager battleManager;

    [Header("Settings")]
    public float blockSize = 0.29f;
    public float pieceSpacing = 1.25f;

    private GameObject[] activePieces = new GameObject[3];
    private Vector2Int[][] activeShapes = new Vector2Int[3][];
    private int placedCount;
    private Sprite blockSprite;

    private readonly Color[] colors =
    {
        new Color(0.25f, 0.75f, 1f),
        new Color(1f, 0.40f, 0.50f),
        new Color(0.55f, 0.90f, 0.35f),
        new Color(0.85f, 0.50f, 1f),
        new Color(1f, 0.75f, 0.25f)
    };

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

    private void Start()
    {
        CreateSprite();
        SpawnPieces();
    }

    private void CreateSprite()
    {
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;

        texture.SetPixels(pixels);
        texture.Apply();

        blockSprite = Sprite.Create(
            texture,
            new Rect(0, 0, 32, 32),
            new Vector2(0.5f, 0.5f),
            32f
        );
    }

    public void SpawnPieces()
    {
        for (int i = 0; i < 3; i++)
        {
            if (activePieces[i] != null)
                Destroy(activePieces[i]);

            int shapeIndex = Random.Range(0, shapes.Length);
            Color pieceColor = colors[Random.Range(0, colors.Length)];

            Vector2Int[] shape = shapes[shapeIndex];
            activeShapes[i] = shape;

            GameObject piece = new GameObject("PlayerPiece_" + i);

            piece.transform.position = new Vector3(
                -5f + (i - 1) * pieceSpacing,
                -3.5f,
                0
            );

            int maxX = 0;
            int maxY = 0;

            foreach (Vector2Int cell in shape)
            {
                maxX = Mathf.Max(maxX, cell.x);
                maxY = Mathf.Max(maxY, cell.y);

                GameObject block = new GameObject("Block");
                block.transform.SetParent(piece.transform);

                block.transform.localPosition = new Vector3(
                    cell.x * blockSize,
                    cell.y * blockSize,
                    0
                );

                SpriteRenderer renderer =
                    block.AddComponent<SpriteRenderer>();

                renderer.sprite = blockSprite;
                renderer.color = pieceColor;
                renderer.sortingOrder = 5;

                block.transform.localScale =
                    Vector3.one * (blockSize * 0.9f);
            }

            BoxCollider2D collider =
                piece.AddComponent<BoxCollider2D>();

            collider.size = new Vector2(
                (maxX + 1) * blockSize,
                (maxY + 1) * blockSize
            );

            collider.offset = new Vector2(
                maxX * blockSize / 2f,
                maxY * blockSize / 2f
            );

            BattlePieceDrag drag =
                piece.AddComponent<BattlePieceDrag>();

            drag.Setup(
                this,
                playerGrid,
                shape,
                pieceColor,
                i
            );

            activePieces[i] = piece;
        }

        placedCount = 0;
    }

    public void OnPiecePlaced(int slotIndex, bool bombExploded)
    {
        if (slotIndex < 0 || slotIndex >= activePieces.Length)
            return;

        activePieces[slotIndex] = null;
        activeShapes[slotIndex] = null;

        placedCount++;

        // Если использованы все три фигуры — выдаём новые.
        if (placedCount >= 3)
            SpawnPieces();

        // Проверяем, можно ли поставить хотя бы одну оставшуюся фигуру.
        if (!HasAnyPossibleMove())
        {
            battleManager.PlayerHasNoMoves();
            return;
        }

        battleManager.RegisterPlayerMove(bombExploded);
    }

    private bool HasAnyPossibleMove()
    {
        if (playerGrid == null)
            return false;

        for (int i = 0; i < activeShapes.Length; i++)
        {
            if (activePieces[i] == null || activeShapes[i] == null)
                continue;

            if (playerGrid.CanPlaceAnywhere(activeShapes[i]))
                return true;
        }

        return false;
    }
}
