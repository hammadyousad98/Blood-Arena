using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalLocalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    public void ShakeNormalHit()
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(Shake(4, 0.2f, 0.10f));
    }

    public void ShakeHeavyHit()
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(Shake(6, 0.3f, 0.15f));
    }

    public void ShakeParry()
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(Shake(3, 0.15f, 0.08f));
    }

    public IEnumerator Shake(int oscillations, float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float timeRatio = elapsed / duration;
            float damping = 1f - timeRatio;
            float sineValue = Mathf.Sin(timeRatio * Mathf.PI * 2f * oscillations);

            float xOffset = sineValue * magnitude * damping;
            float yOffset = Mathf.Sin(timeRatio * Mathf.PI * 2f * (oscillations * 1.5f)) * (magnitude * 0.5f) * damping;

            transform.localPosition = originalLocalPosition + new Vector3(xOffset, yOffset, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition;
    }
}
