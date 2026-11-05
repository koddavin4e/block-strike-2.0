using UnityEngine;

public class ClearBlockEffect : MonoBehaviour
{
    public float duration = 0.22f;

    private float timer;
    private Vector3 startScale;
    private SpriteRenderer spriteRenderer;
    private Color startColor;

    private void Start()
    {
        startScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            startColor = spriteRenderer.color;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float progress = Mathf.Clamp01(timer / duration);
        float scale = 1f - progress;
        scale *= scale;

        transform.localScale = startScale * scale;

        if (spriteRenderer != null)
        {
            Color color = startColor;
            color.a = 1f - progress;
            spriteRenderer.color = color;
        }

        if (progress >= 1f)
            Destroy(gameObject);
    }
}