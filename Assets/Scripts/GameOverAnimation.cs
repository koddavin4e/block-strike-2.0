
using UnityEngine;
using System.Collections;

public class GameOverAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float duration = 0.4f;
    public float startScale = 0.75f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 targetScale;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        targetScale = rectTransform.localScale;
    }

    private void OnEnable()
    {
        if (rectTransform == null)
            return;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        rectTransform.localScale = targetScale * startScale;
        canvasGroup.alpha = 0f;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / duration);

            float smooth = 1f - Mathf.Pow(1f - progress, 3f);

            float bounce = Mathf.Sin(progress * Mathf.PI) * 0.08f;

            rectTransform.localScale =
                targetScale * (startScale +
                (1f - startScale) * smooth + bounce);

            canvasGroup.alpha = smooth;

            yield return null;
        }

        rectTransform.localScale = targetScale;
        canvasGroup.alpha = 1f;

        animationCoroutine = null;
    }
}
