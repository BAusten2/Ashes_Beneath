using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;
    public AudioClip toggleSound;
    private AudioSource audioSource;
    private bool isOn = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (flashlight != null)
        {
            flashlight.enabled = false; // Start with light off
        }
    }

    void Update()
    {
        // Toggle with F key
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        isOn = !isOn;
        
        if (flashlight != null)
        {
            flashlight.enabled = isOn;
        }
        
        if (audioSource && toggleSound)
        {
            audioSource.PlayOneShot(toggleSound);
        }
    }
}