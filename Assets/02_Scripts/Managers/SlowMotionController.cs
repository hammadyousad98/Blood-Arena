using UnityEngine;
using System.Collections;

public class SlowMotionController : MonoBehaviour
{
    private Coroutine slowMoCoroutine;

    public void TriggerSlowMo()
    {
        if (slowMoCoroutine != null) StopCoroutine(slowMoCoroutine);
        slowMoCoroutine = StartCoroutine(DoSlowMo());
    }

    public IEnumerator DoSlowMo()
    {
        Time.timeScale = 0.15f;
        yield return new WaitForSecondsRealtime(1.0f);
        Time.timeScale = 1.0f;
    }
}
