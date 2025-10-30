//Jasmit Gosal 21137879

using UnityEngine;

public class FlickerTrigger : MonoBehaviour
{
    public FlickeringLight flickerLight;
    public float fasterMinDelay = 1.25f;
    public float fasterMaxDelay = 1.75f;
    public float originalMinDelay = 2.25f;
    public float originalMaxDelay = 2.75f;

    //activate when player steps
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger zone");
            flickerLight.SetFlickerSpeed(fasterMinDelay, fasterMaxDelay);
        }
    }

    //deactive when player steps off
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger zone");
            flickerLight.SetFlickerSpeed(originalMinDelay, originalMaxDelay);
        }
    }

    
}
