using UnityEngine;

public class ClearParticles : MonoBehaviour
{
    private Vector3 velocity;
    private float lifetime = 0.35f;
    private float timer;

    private Vector3 startScale;
    private SpriteRenderer spriteRenderer;
    private Color startColor;

    public void Setup(
        Sprite sprite,
        Color color,
        Vector3 direction,
        float size
    )
    {
        velocity = direction;
        startScale = Vector3.one * size;
        transform.localScale = startScale;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = 5;

        startColor = color;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float progress = Mathf.Clamp01(timer / lifetime);

        transform.position += velocity * Time.deltaTime;
        velocity *= 0.96f;

        float scale = 1f - progress;
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

    public static void Play(
        Vector3 position,
        Color color,
        Sprite sprite
    )
    {
        int particleCount = 6;

        for (int i = 0; i < particleCount; i++)
        {
            float angle = (360f / particleCount) * i
                          + Random.Range(-20f, 20f);

            float radians = angle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(radians),
                Mathf.Sin(radians),
                0f
            );

            GameObject particle = new GameObject("ClearParticle");
            particle.transform.position = position;

            ClearParticles effect =
                particle.AddComponent<ClearParticles>();

            float size = Random.Range(0.07f, 0.13f);

            effect.Setup(
                sprite,
                color,
                direction * Random.Range(1.5f, 3f),
                size
            );
        }
    }
}