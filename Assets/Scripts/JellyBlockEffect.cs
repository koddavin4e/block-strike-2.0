using UnityEngine;

public class JellyBlockEffect : MonoBehaviour
{
    public float duration = 0.25f;
    public float startScale = 0.72f;

    private float timer;
    private Vector3 targetScale;

    private void Awake()
    {
        targetScale = transform.localScale;
        transform.localScale = targetScale * startScale;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float progress = Mathf.Clamp01(timer / duration);
        float smooth = 1f - Mathf.Pow(1f - progress, 3f);

        float bounce = Mathf.Sin(progress * Mathf.PI) * 0.12f;

        transform.localScale =
            targetScale * (startScale + (1f - startScale) * smooth + bounce);

        if (progress >= 1f)
        {
            transform.localScale = targetScale;
            enabled = false;
        }
    }
}