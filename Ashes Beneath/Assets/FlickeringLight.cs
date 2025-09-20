using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour
{
    private Light lightSource;
    public float minDelay = 0.05f;
    public float maxDelay = 0.25f;

    private Coroutine flickerRoutine;

    void Start()
    {
        lightSource = GetComponent<Light>();
        flickerRoutine = StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            lightSource.enabled = !lightSource.enabled;
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }

    public void SetFlickerSpeed(float newMin, float newMax)
    {
        minDelay = newMin;
        maxDelay = newMax;

        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
            flickerRoutine = StartCoroutine(Flicker());
        }
    }
}
