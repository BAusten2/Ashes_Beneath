using UnityEngine;

public class FlickerTrigger : MonoBehaviour
{
    public FlickeringLight flickerLight;
    public float fasterMinDelay = 1.25f;
    public float fasterMaxDelay = 1.75f;
    public float originalMinDelay = 2.25f;
    public float originalMaxDelay = 2.75f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger zone");
            flickerLight.SetFlickerSpeed(fasterMinDelay, fasterMaxDelay);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger zone");
            flickerLight.SetFlickerSpeed(originalMinDelay, originalMaxDelay);
        }
    }

    
}
