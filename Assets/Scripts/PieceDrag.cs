
using UnityEngine;

public class PieceDrag : MonoBehaviour
{
    public GridManager gridManager;
    public PieceSpawner pieceSpawner;
    public int slotIndex;

    [Header("Preview")]
    public float previewScale = 0.65f;

    private Vector3 startPosition;
    private Vector3 dragOffset;

    private Camera mainCamera;
    private PieceView pieceView;

    private bool dragging;
    private int activeTouchId = -1;

    private void Start()
    {
        mainCamera = Camera.main;
        pieceView = GetComponent<PieceView>();

        startPosition = transform.position;

        // Масштабируем только модель фигуры.
        transform.localScale = Vector3.one * previewScale;
    }

    private Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        float distance = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 point = mainCamera.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, distance)
        );

        point.z = 0f;
        return point;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            HandleTouch();
            return;
        }

        HandleMouse();
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            TryBeginDrag(Input.mousePosition, -1);

        if (dragging && activeTouchId == -1)
        {
            if (Input.GetMouseButton(0))
                MovePiece(Input.mousePosition);

            if (Input.GetMouseButtonUp(0))
                FinishDrag();
        }
    }

    private void HandleTouch()
    {
        if (!dragging)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                    TryBeginDrag(touch.position, touch.fingerId);
            }

            return;
        }

        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId != activeTouchId)
                continue;

            if (touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary)
                MovePiece(touch.position);

            if (touch.phase == TouchPhase.Ended ||
                touch.phase == TouchPhase.Canceled)
                FinishDrag();

            break;
        }
    }

    private void TryBeginDrag(Vector2 screenPosition, int touchId)
    {
        Vector3 worldPosition = ScreenToWorld(screenPosition);

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit == null || hit.transform != transform)
            return;

        dragging = true;
        activeTouchId = touchId;

        startPosition = transform.position;
        dragOffset = transform.position - worldPosition;

        transform.localScale = Vector3.one;
    }

    private void MovePiece(Vector2 screenPosition)
    {
        Vector3 target = ScreenToWorld(screenPosition) + dragOffset;
        transform.position = new Vector3(target.x, target.y, 0f);
    }

    private void FinishDrag()
    {
        if (!dragging)
            return;

        dragging = false;
        activeTouchId = -1;

        bool placed = gridManager.TryPlace(
            pieceView,
            transform.position
        );

        if (placed)
        {
            pieceSpawner.PieceUsed(this);
            Destroy(gameObject);
        }
        else
        {
            transform.position = startPosition;
            transform.localScale = Vector3.one * previewScale;
        }
    }
}
