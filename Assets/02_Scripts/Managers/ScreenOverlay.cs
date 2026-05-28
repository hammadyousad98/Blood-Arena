using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenOverlay : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    private Coroutine overlayCoroutine;

    private void Awake()
    {
        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = 0f;
            overlayImage.color = c;
            overlayImage.raycastTarget = false;
        }
    }

    public void NormalHit()
    {
        if (ColorUtility.TryParseHtmlString("#FF4500", out Color c))
        {
            c.a = 0.4f;
            TriggerOverlay(c, 0.3f);
        }
    }

    public void HeavyHit()
    {
        if (ColorUtility.TryParseHtmlString("#FF0000", out Color c))
        {
            c.a = 0.5f;
            TriggerOverlay(c, 0.4f);
        }
    }

    public void Block()
    {
        if (ColorUtility.TryParseHtmlString("#4A90D9", out Color c))
        {
            c.a = 0.35f;
            TriggerOverlay(c, 0.2f);
        }
    }

    public void Parry()
    {
        if (ColorUtility.TryParseHtmlString("#F39C12", out Color c))
        {
            c.a = 0.6f;
            TriggerOverlay(c, 0.2f);
        }
    }

    private void TriggerOverlay(Color color, float duration)
    {
        if (overlayCoroutine != null) StopCoroutine(overlayCoroutine);
        if (overlayImage != null)
        {
            overlayCoroutine = StartCoroutine(ShowOverlay(color, duration));
        }
    }

    public IEnumerator ShowOverlay(Color color, float duration)
    {
        float elapsed = 0f;
        float startAlpha = color.a;
        overlayImage.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Color current = overlayImage.color;
            current.a = Mathf.Lerp(startAlpha, 0f, t);
            overlayImage.color = current;
            yield return null;
        }

        Color final = overlayImage.color;
        final.a = 0f;
        overlayImage.color = final;
    }
}
