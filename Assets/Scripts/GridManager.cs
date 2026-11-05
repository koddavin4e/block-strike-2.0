using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 10;
    public float cellSize = 0.65f;
    public Vector2 boardCenter = new Vector2(0f, 2f);

    [Header("Colors")]
    public Color emptyColor = new Color(0.40f, 0.45f, 0.53f);
    public Color blockColor = new Color(0.65f, 0.78f, 0.68f);

    [Header("Board Design")]
    public Color frameColor = new Color(0.72f, 0.76f, 0.83f);
    public Color panelColor = new Color(0.12f, 0.15f, 0.20f);
    public Color shadowColor = new Color(0.04f, 0.05f, 0.08f, 0.75f);

    [Header("Cell Appearance")]
    [Range(0.85f, 1f)]
    public float emptyCellScale = 0.94f;

    [Header("References")]
    public ScoreManager scoreManager;

    private bool[,] occupied;
    private GameObject[,] placedObjects;

    private Sprite blockSprite;
    private Sprite emptySprite;
    private Sprite panelSprite;

    public Sprite BlockSprite => blockSprite;

    private Vector2 BottomLeft =>
        boardCenter - Vector2.one * (gridSize * cellSize / 2f);

    private float BoardWidth => gridSize * cellSize;

    private void Awake()
    {
        occupied = new bool[gridSize, gridSize];
        placedObjects = new GameObject[gridSize, gridSize];

        CreateSprites();
    }

    private void Start()
    {
        CreateBoardDecoration();
        CreateBoard();
    }

    private void CreateSprites()
    {
        blockSprite = CreateRoundedSprite(true);
        emptySprite = CreateRoundedSprite(false);
        panelSprite = CreatePanelSprite();
    }

    private Sprite CreateRoundedSprite(bool glossy)
    {
        int size = 128;

        Texture2D texture = new Texture2D(
            size, size, TextureFormat.RGBA32, false
        );

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        float radius = glossy ? 0.25f : 0.18f;
        float border = 1f - radius;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float px = (x + 0.5f) / size * 2f - 1f;
                float py = (y + 0.5f) / size * 2f - 1f;

                float qx = Mathf.Abs(px) - border;
                float qy = Mathf.Abs(py) - border;

                float distance =
                    Mathf.Min(Mathf.Max(qx, qy), 0f) +
                    Mathf.Sqrt(
                        Mathf.Max(qx, 0f) * Mathf.Max(qx, 0f) +
                        Mathf.Max(qy, 0f) * Mathf.Max(qy, 0f)
                    ) - radius;

                float alpha = 1f - Mathf.SmoothStep(
                    -0.015f, 0.015f, distance
                );

                float shade;

                if (glossy)
                {
                    float gradient = 0.72f +
                        0.20f * ((py + 1f) / 2f);

                    float rim = Mathf.Exp(
                        -Mathf.Pow((distance + 0.045f) / 0.075f, 2f)
                    ) * 0.24f;

                    float shine = Mathf.Exp(
                        -Mathf.Pow((px + 0.30f) / 0.55f, 2f)
                        - Mathf.Pow((py - 0.62f) / 0.15f, 2f)
                    ) * 0.65f;

                    float smallShine = Mathf.Exp(
                        -Mathf.Pow((px - 0.55f) / 0.12f, 2f)
                        - Mathf.Pow((py - 0.35f) / 0.30f, 2f)
                    ) * 0.20f;

                    float bottomShadow = Mathf.Exp(
                        -Mathf.Pow((py + 0.88f) / 0.13f, 2f)
                    ) * 0.20f;

                    float innerShade = Mathf.Clamp01(
                        -distance / 0.20f
                    );

                    shade = gradient + rim + shine +
                            smallShine - bottomShadow;

                    shade *= 0.90f + innerShade * 0.10f;
                }
                else
                {
                    shade = 0.88f +
                        0.08f * ((py + 1f) / 2f);
                }

                shade = Mathf.Clamp01(shade);

                texture.SetPixel(
                    x, y,
                    new Color(shade, shade, shade, alpha)
                );
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            size
        );
    }

    private Sprite CreatePanelSprite()
    {
        int size = 128;

        Texture2D texture = new Texture2D(
            size, size, TextureFormat.RGBA32, false
        );

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        float radius = 0.08f;
        float border = 1f - radius;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float px = (x + 0.5f) / size * 2f - 1f;
                float py = (y + 0.5f) / size * 2f - 1f;

                float qx = Mathf.Abs(px) - border;
                float qy = Mathf.Abs(py) - border;

                float distance =
                    Mathf.Min(Mathf.Max(qx, qy), 0f) +
                    Mathf.Sqrt(
                        Mathf.Max(qx, 0f) * Mathf.Max(qx, 0f) +
                        Mathf.Max(qy, 0f) * Mathf.Max(qy, 0f)
                    ) - radius;

                float alpha = 1f - Mathf.SmoothStep(
                    -0.015f, 0.015f, distance
                );

                texture.SetPixel(
                    x, y,
                    new Color(1f, 1f, 1f, alpha)
                );
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            size
        );
    }

    private void CreateBoardDecoration()
    {
        float width = BoardWidth;

        CreatePanel(
            "Board_Shadow",
            boardCenter + new Vector2(0.06f, -0.10f),
            new Vector2(width + 0.52f, width + 0.52f),
            shadowColor,
            -4
        );

        CreatePanel(
            "Board_Frame",
            boardCenter,
            new Vector2(width + 0.32f, width + 0.32f),
            frameColor,
            -3
        );

        CreatePanel(
            "Board_Glass",
            boardCenter,
            new Vector2(width + 0.12f, width + 0.12f),
            panelColor,
            -2
        );
    }

    private void CreatePanel(
        string objectName,
        Vector2 position,
        Vector2 size,
        Color color,
        int sortingOrder
    )
    {
        GameObject panel = new GameObject(objectName);

        panel.transform.SetParent(transform);
        panel.transform.position = position;

        panel.transform.localScale = new Vector3(
            size.x, size.y, 1f
        );

        SpriteRenderer renderer =
            panel.AddComponent<SpriteRenderer>();

        renderer.sprite = panelSprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
    }

    private void CreateBoard()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject cell = new GameObject(
                    $"Cell_{x}_{y}"
                );

                cell.transform.SetParent(transform);
                cell.transform.position = GetCellPosition(x, y);

                cell.transform.localScale = new Vector3(
                    cellSize * emptyCellScale,
                    cellSize * emptyCellScale,
                    1f
                );

                SpriteRenderer renderer =
                    cell.AddComponent<SpriteRenderer>();

                renderer.sprite = emptySprite;
                renderer.color = emptyColor;
                renderer.sortingOrder = 0;
            }
        }
    }

    public Vector3 GetCellPosition(int x, int y)
    {
        Vector2 position = BottomLeft +
            new Vector2(
                (x + 0.5f) * cellSize,
                (y + 0.5f) * cellSize
            );

        return position;
    }

    public bool TryPlace(PieceView piece, Vector3 worldPosition)
    {
        if (piece == null ||
            piece.cells == null ||
            piece.cells.Length == 0)
            return false;

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Vector2Int cell in piece.cells)
        {
            minX = Mathf.Min(minX, cell.x);
            minY = Mathf.Min(minY, cell.y);
            maxX = Mathf.Max(maxX, cell.x);
            maxY = Mathf.Max(maxY, cell.y);
        }

        float centerX = (minX + maxX) * 0.5f;
        float centerY = (minY + maxY) * 0.5f;

        Vector2 origin = BottomLeft +
            Vector2.one * (cellSize * 0.5f);

        int startX = Mathf.RoundToInt(
            (worldPosition.x - origin.x) / cellSize - centerX
        );

        int startY = Mathf.RoundToInt(
            (worldPosition.y - origin.y) / cellSize - centerY
        );

        foreach (Vector2Int cell in piece.cells)
        {
            int x = startX + cell.x;
            int y = startY + cell.y;

            if (x < 0 || x >= gridSize ||
                y < 0 || y >= gridSize)
                return false;

            if (occupied[x, y])
                return false;
        }

        foreach (Vector2Int cell in piece.cells)
        {
            int x = startX + cell.x;
            int y = startY + cell.y;

            occupied[x, y] = true;

            GameObject block = new GameObject("PlacedBlock");

            block.transform.position = GetCellPosition(x, y);

            block.transform.localScale =
                Vector3.one * cellSize * 0.90f;

            SpriteRenderer renderer =
                block.AddComponent<SpriteRenderer>();

            renderer.sprite = blockSprite;
            renderer.color = piece.color;
            renderer.sortingOrder = 2;

            block.AddComponent<JellyBlockEffect>();

            placedObjects[x, y] = block;
        }

        int lines = ClearLines();

        if (scoreManager != null)
        {
            scoreManager.RegisterMove(
                piece.cells.Length,
                lines
            );
        }

        return true;
    }

    // Проверяет, можно ли поставить фигуру хотя бы в одно место.
    public bool CanPlaceShape(Vector2Int[] shape)
    {
        if (shape == null || shape.Length == 0)
            return false;

        int minX = int.MaxValue;
        int minY = int.MaxValue;

        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Vector2Int cell in shape)
        {
            minX = Mathf.Min(minX, cell.x);
            minY = Mathf.Min(minY, cell.y);

            maxX = Mathf.Max(maxX, cell.x);
            maxY = Mathf.Max(maxY, cell.y);
        }

        for (int startX = 0; startX < gridSize; startX++)
        {
            for (int startY = 0; startY < gridSize; startY++)
            {
                bool canPlace = true;

                foreach (Vector2Int cell in shape)
                {
                    int x = startX + cell.x - minX;
                    int y = startY + cell.y - minY;

                    if (x < 0 || x >= gridSize ||
                        y < 0 || y >= gridSize ||
                        occupied[x, y])
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                    return true;
            }
        }

        return false;
    }

    // Полностью очищает поле для новой игры.
    public void ResetBoard()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                occupied[x, y] = false;

                if (placedObjects[x, y] != null)
                {
                    Destroy(placedObjects[x, y]);
                    placedObjects[x, y] = null;
                }
            }
        }
    }

    private int ClearLines()
    {
        bool[] fullRows = new bool[gridSize];
        bool[] fullColumns = new bool[gridSize];

        int rowCount = 0;
        int columnCount = 0;

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

            if (full)
            {
                fullRows[y] = true;
                rowCount++;
            }
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

            if (full)
            {
                fullColumns[x] = true;
                columnCount++;
            }
        }

        int totalLines = rowCount + columnCount;

        if (totalLines == 0)
            return 0;

        ComboEffect.Play(
            new Vector3(boardCenter.x, boardCenter.y, 0f),
            totalLines
        );

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!fullRows[y] && !fullColumns[x])
                    continue;

                occupied[x, y] = false;

                if (placedObjects[x, y] != null)
                {
                    GameObject block = placedObjects[x, y];

                    SpriteRenderer renderer =
                        block.GetComponent<SpriteRenderer>();

                    if (renderer != null)
                    {
                        ClearParticles.Play(
                            block.transform.position,
                            renderer.color,
                            blockSprite
                        );
                    }

                    if (block.GetComponent<ClearBlockEffect>() == null)
                        block.AddComponent<ClearBlockEffect>();

                    placedObjects[x, y] = null;
                }
            }
        }

        return totalLines;
    }
}