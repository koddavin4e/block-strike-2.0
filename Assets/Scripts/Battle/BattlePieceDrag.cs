
using UnityEngine;

public class BattlePieceDrag : MonoBehaviour
{
    private BattlePieceSpawner spawner;
    private BattleGrid grid;
    private BattleManager battleManager;

    private Vector2Int[] shape;
    private Color pieceColor;
    private int slotIndex;

    private Camera mainCamera;
    private Vector3 dragOffset;
    private Vector3 originalPosition;
    private bool dragging;

    public void Setup(
        BattlePieceSpawner pieceSpawner,
        BattleGrid battleGrid,
        Vector2Int[] pieceShape,
        Color color,
        int index)
    {
        spawner = pieceSpawner;
        grid = battleGrid;
        shape = pieceShape;
        pieceColor = color;
        slotIndex = index;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        battleManager = FindFirstObjectByType<BattleManager>();
    }

    private void OnMouseDown()
    {
        if (mainCamera == null || grid == null)
            return;

        if (battleManager != null && !battleManager.IsPlayerTurn)
            return;

        originalPosition = transform.position;

        Vector3 mouse = mainCamera.ScreenToWorldPoint(
            Input.mousePosition
        );

        mouse.z = 0;

        dragOffset = transform.position - mouse;
        dragging = true;
    }

    private void OnMouseDrag()
    {
        if (!dragging || mainCamera == null)
            return;

        if (battleManager != null && !battleManager.IsPlayerTurn)
            return;

        Vector3 mouse = mainCamera.ScreenToWorldPoint(
            Input.mousePosition
        );

        mouse.z = 0;
        transform.position = mouse + dragOffset;
    }

    private void OnMouseUp()
    {
        if (!dragging)
            return;

        dragging = false;

        if (battleManager != null && !battleManager.IsPlayerTurn)
        {
            transform.position = originalPosition;
            return;
        }

        Vector3 localPosition =
            grid.transform.InverseTransformPoint(transform.position);

        float half =
            (grid.gridSize - 1) * grid.cellSize / 2f;

        int startX = Mathf.RoundToInt(
            (localPosition.x + half) / grid.cellSize
        );

        int startY = Mathf.RoundToInt(
            (localPosition.y + half) / grid.cellSize
        );

        bool bombExploded;
        int clearedLines;

        bool success = grid.TryPlaceShape(
            shape,
            startX,
            startY,
            pieceColor,
            out bombExploded,
            out clearedLines
        );

        if (success)
        {
            spawner.OnPiecePlaced(slotIndex, bombExploded);
            Destroy(gameObject);
        }
        else
        {
            transform.position = originalPosition;
        }
    }
}
