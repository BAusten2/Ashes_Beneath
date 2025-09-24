using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioSource))]
public class FlickeringLight : MonoBehaviour
{
    private Light lightSource;
    private AudioSource audioSource;

    public float minDelay = 2.25f;
    public float maxDelay = 2.75f;

    private Coroutine flickerRoutine;

    void Start()
    {
        lightSource = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;         // Play only once per flicker
        audioSource.playOnAwake = false;  // You control when it plays

        flickerRoutine = StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            bool isOn = !lightSource.enabled;
            lightSource.enabled = isOn;

            if (isOn && audioSource != null)
            {
                audioSource.Play(); // Play buzz when light turns ON
            }

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
