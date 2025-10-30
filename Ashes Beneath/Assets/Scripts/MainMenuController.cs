//Jasmit Gosal 21137879

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{


    //load new game + activate time if frozen
    public void NewGame()
    {
        Time.timeScale = 1f;
        SaveManager.DeleteSave();
        PlayerPrefs.SetInt("ShouldLoadSave", 0);
        SceneManager.LoadScene("Environment FINAL");
    }

    //loads game + activate time if frozen
    public void LoadGame()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("ShouldLoadSave", 1);
        SceneManager.LoadScene("Environment FINAL");
    }

    //exits game
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited");
    }
}
