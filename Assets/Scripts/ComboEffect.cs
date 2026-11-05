
using UnityEngine;

public class ComboEffect : MonoBehaviour
{
    private float timer;
    private float duration = 1.1f;

    private TextMesh textMesh;
    private Color startColor;
    private Vector3 startScale;
    private Vector3 startPosition;

    private void Start()
    {
        textMesh = GetComponent<TextMesh>();

        if (textMesh == null)
            return;

        startColor = textMesh.color;
        startScale = transform.localScale;
        startPosition = transform.position;

        transform.localScale = startScale * 0.5f;
    }

    private void Update()
    {
        if (textMesh == null)
            return;

        timer += Time.deltaTime;

        float progress = Mathf.Clamp01(timer / duration);

        float scaleIn = Mathf.SmoothStep(0f, 1f,
            Mathf.Clamp01(progress * 5f));

        float bounce = 1f + Mathf.Sin(progress * Mathf.PI * 5f)
            * 0.08f * (1f - progress);

        transform.localScale =
            startScale * scaleIn * bounce;

        transform.position = startPosition +
            Vector3.up * (progress * 0.8f);

        Color color = startColor;
        color.a = 1f - progress;

        textMesh.color = color;

        if (progress >= 1f)
            Destroy(gameObject);
    }

    public static void Play(Vector3 position, int lines)
    {
        if (lines < 2)
            return;

        string message;
        Color color;

        if (lines == 2)
        {
            message = "DOUBLE!";
            color = new Color(0.2f, 0.85f, 1f);
        }
        else if (lines == 3)
        {
            message = "TRIPLE!";
            color = new Color(1f, 0.8f, 0.15f);
        }
        else
        {
            message = "AMAZING!";
            color = new Color(1f, 0.35f, 0.65f);
        }

        GameObject combo = new GameObject("ComboText");

        combo.transform.position =
            position + Vector3.up * 0.4f;

        TextMesh text = combo.AddComponent<TextMesh>();

        text.text = message;
        text.fontSize = 80;
        text.characterSize = 0.08f;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = color;
        text.fontStyle = FontStyle.Bold;

        MeshRenderer renderer =
            combo.GetComponent<MeshRenderer>();

        renderer.sortingOrder = 20;

        combo.transform.localScale = Vector3.one;

        combo.AddComponent<ComboEffect>();
    }
}
