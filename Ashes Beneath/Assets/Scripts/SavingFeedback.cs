using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SavingFeedback : MonoBehaviour
{
    public GameObject savingIcon;
    public GameObject savingText;
    public float displayTime = 2f; // Time to show the icon

    public void ShowSavingIcon()
    {
        if (savingIcon != null)
            StartCoroutine(DisplaySaving());
    }

    private IEnumerator DisplaySaving()
    {
        if (savingIcon != null) savingIcon.SetActive(true);
        if (savingText != null) savingText.SetActive(true);
        

        yield return new WaitForSecondsRealtime(displayTime);

        if (savingIcon != null) savingIcon.SetActive(false);
        if (savingText != null) savingText.SetActive(false);
    }
}
