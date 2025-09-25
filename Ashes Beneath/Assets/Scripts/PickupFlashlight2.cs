using UnityEngine;

public class PickupFlashlight : MonoBehaviour
{
    public GameObject inttext;
    public GameObject flashlight_table;
    public GameObject flashlight_hand;
    public AudioSource pickup;

    public float interactDistance = 3f; // how close the player must be

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.gameObject == flashlight_table)
            {
                inttext.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    inttext.SetActive(false);
                    flashlight_hand.SetActive(true);
                    flashlight_table.SetActive(false);
                    if (pickup != null) pickup.Play();
                }
            }
        }
        else
        {
            inttext.SetActive(false);
        }
    }
}
