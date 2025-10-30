using UnityEngine;

public class AutoSaveTrigger : MonoBehaviour
{
    private bool hasSaved = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasSaved && other.CompareTag("Player"))
        {
            Vector3 playerPos = other.transform.position;
            SaveManager.SavePlayer(playerPos);
            Debug.Log("Autosave triggered");

            SavingFeedback feedback = FindObjectOfType<SavingFeedback>();
            if (feedback != null)
            {
                feedback.ShowSavingIcon();
            }

            hasSaved = true; // Prevent multiple triggers
        }
    }
}
