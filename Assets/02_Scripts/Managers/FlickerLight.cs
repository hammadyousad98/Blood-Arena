using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickerLight : MonoBehaviour
{
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 5f;

    private Light targetLight;
    private float targetIntensity;

    private void Awake()
    {
        targetLight = GetComponent<Light>();
        targetIntensity = targetLight.intensity;
    }

    private void Update()
    {
        // Smoothly vary target intensity
        if (Mathf.Abs(targetLight.intensity - targetIntensity) < 0.05f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }

        targetLight.intensity = Mathf.Lerp(targetLight.intensity, targetIntensity, Time.deltaTime * flickerSpeed);
    }
}
