using UnityEngine;

public class FlickerTrigger : MonoBehaviour
{
    public FlickeringLight flickerLight;  // Assign in Inspector
    public float fasterMinDelay = 0.01f;
    public float fasterMaxDelay = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            flickerLight.SetFlickerSpeed(fasterMinDelay, fasterMaxDelay);
        }
    }
}
