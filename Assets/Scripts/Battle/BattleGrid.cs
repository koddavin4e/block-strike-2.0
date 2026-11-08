
using UnityEngine;

public class BattleGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 10;
    public float cellSize = 0.32f;

    [Header("Colors")]
    public Color emptyColor = new Color(0.20f, 0.24f, 0.30f);
    public Color playerColor = new Color(0.35f, 0.85f, 0.65f);
    public Color botColor = new Color(0.95f, 0.55f, 0.45f);
    public Color bombColor = new Color(1f, 0.20f, 0.16f);

    private bool[,] occupied;
    private bool[,] bombs;
    private GameObject[,] blockObjects;

    private Sprite blockSprite;

    public int PiecesPlaced { get; private set; }

    private void Awake()
    {
        occupied = new bool[gridSize, gridSize];
        bombs = new bool[gridSize, gridSize];
        blockObjects = new GameObject[gridSize, gridSize];

        CreateSprite();
    }

    private void Start()
    {
        CreateBoard();
    }

    private void CreateSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        blockSprite = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1
        );
    }

    private void CreateBoard()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject cell = new GameObject(
                    "Cell_" + x + "_" + y
                );

                cell.transform.SetParent(transform, false);

                cell.transform.localPosition = GetCellLocalPosition(x, y);

                cell.transform.localScale =
                    Vector3.one * cellSize * 0.92f;

                SpriteRenderer renderer =
                    cell.AddComponent<SpriteRenderer>();

                renderer.sprite = blockSprite;
                renderer.color = emptyColor;
                renderer.sortingOrder = 0;
            }
        }
    }

    public Vector3 GetCellLocalPosition(int x, int y)
    {
        float offset = (gridSize - 1) * cellSize * 0.5f;

        return new Vector3(
            x * cellSize - offset,
            y * cellSize - offset,
            0f
        );
    }

    public bool CanPlaceShapeAt(
        Vector2Int[] shape,
        int startX,
        int startY
    )
    {
        if (shape == null || shape.Length == 0)
            return false;

        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (Vector2Int cell in shape)
        {
            minX = Mathf.Min(minX, cell.x);
            minY = Mathf.Min(minY, cell.y);
        }

        foreach (Vector2Int cell in shape)
        {
            int x = startX + cell.x - minX;
            int y = startY + cell.y - minY;

            if (x < 0 || x >= gridSize ||
                y < 0 || y >= gridSize)
                return false;

            if (occupied[x, y])
                return false;
        }

        return true;
    }

    public bool CanPlaceAnywhere(Vector2Int[] shape)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (CanPlaceShapeAt(shape, x, y))
                    return true;
            }
        }

        return false;
    }

    public bool TryPlaceShape(
        Vector2Int[] shape,
        int startX,
        int startY,
        Color color,
        out bool bombExploded,
        out int clearedLines
    )
    {
        bombExploded = false;
        clearedLines = 0;

        if (!CanPlaceShapeAt(shape, startX, startY))
            return false;

        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (Vector2Int cell in shape)
        {
            minX = Mathf.Min(minX, cell.x);
            minY = Mathf.Min(minY, cell.y);
        }

        foreach (Vector2Int cell in shape)
        {
            int x = startX + cell.x - minX;
            int y = startY + cell.y - minY;

            occupied[x, y] = true;

            GameObject block = CreateBlock(
                "PlacedBlock",
                x,
                y,
                color,
                2
            );

            blockObjects[x, y] = block;
        }

        PiecesPlaced++;

        ClearLines(out bombExploded, out clearedLines);

        return true;
    }

    private GameObject CreateBlock(
        string objectName,
        int x,
        int y,
        Color color,
        int sortingOrder
    )
    {
        GameObject block = new GameObject(objectName);

        block.transform.SetParent(transform, false);
        block.transform.localPosition = GetCellLocalPosition(x, y);
        block.transform.localScale =
            Vector3.one * cellSize * 0.90f;

        SpriteRenderer renderer =
            block.AddComponent<SpriteRenderer>();

        renderer.sprite = blockSprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        return block;
    }

    public bool SpawnBomb()
    {
        System.Collections.Generic.List<Vector2Int> emptyCells =
            new System.Collections.Generic.List<Vector2Int>();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!occupied[x, y])
                    emptyCells.Add(new Vector2Int(x, y));
            }
        }

        if (emptyCells.Count == 0)
            return false;

        Vector2Int position =
            emptyCells[Random.Range(0, emptyCells.Count)];

        int bombX = position.x;
        int bombY = position.y;

        occupied[bombX, bombY] = true;
        bombs[bombX, bombY] = true;

        GameObject bomb = CreateBlock(
            "Bomb",
            bombX,
            bombY,
            bombColor,
            3
        );

        blockObjects[bombX, bombY] = bomb;

        return true;
    }

    private void ClearLines(
        out bool bombExploded,
        out int clearedLines
    )
    {
        bombExploded = false;
        clearedLines = 0;

        bool[] fullRows = new bool[gridSize];
        bool[] fullColumns = new bool[gridSize];

        for (int y = 0; y < gridSize; y++)
        {
            bool full = true;

            for (int x = 0; x < gridSize; x++)
            {
                if (!occupied[x, y])
                {
                    full = false;
                    break;
                }
            }

            fullRows[y] = full;

            if (full)
                clearedLines++;
        }

        for (int x = 0; x < gridSize; x++)
        {
            bool full = true;

            for (int y = 0; y < gridSize; y++)
            {
                if (!occupied[x, y])
                {
                    full = false;
                    break;
                }
            }

            fullColumns[x] = full;

            if (full)
                clearedLines++;
        }

        if (clearedLines == 0)
            return;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!fullRows[y] && !fullColumns[x])
                    continue;

                if (bombs[x, y])
                    bombExploded = true;

                occupied[x, y] = false;
                bombs[x, y] = false;

                if (blockObjects[x, y] != null)
                {
                    Destroy(blockObjects[x, y]);
                    blockObjects[x, y] = null;
                }
            }
        }
    }

    public void ResetBoard()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                occupied[x, y] = false;
                bombs[x, y] = false;

                if (blockObjects[x, y] != null)
                {
                    Destroy(blockObjects[x, y]);
                    blockObjects[x, y] = null;
                }
            }
        }

        PiecesPlaced = 0;
    }
}
