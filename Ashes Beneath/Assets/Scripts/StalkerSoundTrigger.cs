//Jasmit Gosal 21137879

using UnityEngine;

public class StalkerSoundTrigger : MonoBehaviour
{
    [Header("Stalker Sound Settings")]
    public AudioClip[] scarySounds; // Assign footsteps, whispers, scratching, etc.
    public float delayBeforeFirstSound = 5f;
    public float timeBetweenSounds = 10f;

    private float timer = 0f;
    private bool playerInside = false;
    private AudioSource audioSource;

    void Start()
    {
        // You can put this script on an empty GameObject and add an AudioSource to it
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 1f; // 3D audio
        audioSource.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            timer = 0f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            timer = 0f;
        }
    }

    void Update()
    {
        if (playerInside && scarySounds.Length > 0)
        {
            timer += Time.deltaTime;

            if (!audioSource.isPlaying)
            {
                if (timer >= delayBeforeFirstSound)
                {
                    PlayRandomScarySound();
                    timer = -timeBetweenSounds; // wait before next
                }
            }
        }
    }

    void PlayRandomScarySound()
    {
        AudioClip clip = scarySounds[Random.Range(0, scarySounds.Length)];
        audioSource.clip = clip;
        audioSource.Play();
    }
}
