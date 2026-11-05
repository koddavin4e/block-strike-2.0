
using UnityEngine;

public class PieceView : MonoBehaviour
{
    public Vector2Int[] cells;
    public Color color;

    private float cellSize;

    public void Setup(
        Vector2Int[] shape,
        Color pieceColor,
        Sprite sprite,
        float size
    )
    {
        cells = shape;
        color = pieceColor;
        cellSize = size;

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Vector2Int cell in cells)
        {
            minX = Mathf.Min(minX, cell.x);
            minY = Mathf.Min(minY, cell.y);
            maxX = Mathf.Max(maxX, cell.x);
            maxY = Mathf.Max(maxY, cell.y);
        }

        // Центрируем фигуру относительно её родителя.
        float centerX = (minX + maxX) * 0.5f;
        float centerY = (minY + maxY) * 0.5f;

        foreach (Vector2Int cell in cells)
        {
            GameObject block = new GameObject("PieceBlock");
            block.transform.SetParent(transform, false);

            block.transform.localPosition = new Vector3(
                (cell.x - centerX) * cellSize,
                (cell.y - centerY) * cellSize,
                0f
            );

            block.transform.localScale =
                Vector3.one * cellSize * 0.90f;

            SpriteRenderer renderer =
                block.AddComponent<SpriteRenderer>();

            renderer.sprite = sprite;
            renderer.color = pieceColor;
            renderer.sortingOrder = 3;
        }

        // Коллайдер охватывает всю фигуру.
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();

        collider.size = new Vector2(
            (maxX - minX + 1) * cellSize,
            (maxY - minY + 1) * cellSize
        );

        collider.offset = Vector2.zero;
    }
}
