//Jasmit Gosal 21137879

using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public SavingFeedback savingFeedback;
    public GameObject pauseMenuUI;
    public Transform playerTransform;
    public static bool GameIsPaused = false;
    private bool isPaused = false;

    //pause if esc is pressed
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    //resume game + resume time
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //pasue game + freeze time
    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        GameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SaveGame()
    {
        if (playerTransform != null)
        {
            SaveManager.SavePlayer(playerTransform.position);
            Debug.Log("Game Saved");

            //show saving icon
            FindObjectOfType<SavingFeedback>()?.ShowSavingIcon();
        }
        else
        {
            Debug.Log("PlayerTransform not assigned.");
        }
    }

    //exit to main menu
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
