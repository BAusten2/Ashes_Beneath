//Jasmit Gosal 21137879

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

    //start light flicker
    void Start()
    {
        lightSource = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;        
        audioSource.playOnAwake = false; 

        flickerRoutine = StartCoroutine(Flicker());
    }

    //play audio sorce synced with flicker
    IEnumerator Flicker()
    {
        while (true)
        {
            bool isOn = !lightSource.enabled;
            lightSource.enabled = isOn;

            if (isOn && audioSource != null)
            {
                audioSource.Play();
            }

            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }

    //set flicker speed
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
