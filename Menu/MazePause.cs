using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MazePause : MonoBehaviour
{
    private static bool gameIsPaused = false;


    [Header("References")]
    [SerializeField] private Image crosshair;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject interactTextHolder;
    [SerializeField] private Interactor interactor = null;
    [SerializeField] private SavingAndLoadingManager savingAndLoadingManager;
    [SerializeField] private Transform player;
    [SerializeField] private Transform start;

    private void OnEnable()
    {
        PlayerPrefs.SetInt("Maze", 0);
    }


    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Escape();
        }
    }


    public void Escape() { 
        if(gameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        interactor.enabled = true;
        Time.timeScale = 1f;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        crosshair.enabled = true;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); //set menu to show
        interactor.enabled = false;
        interactTextHolder.SetActive(false);
        Time.timeScale = 0f;// freeze the game
        gameIsPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        crosshair.enabled = false;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        savingAndLoadingManager.Save();
        gameIsPaused = false;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SaveGame()
    {
        Debug.Log("Saving Game");
        savingAndLoadingManager.Save();
        PlayerPrefs.SetInt("currentScene", SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToStart()
    {
        player.position = start.position;
        player.rotation = start.rotation;
    }


    public void LaunchSettingsMenu()
    {
        PlayerPrefs.SetInt("PreviousScene", SceneManager.GetActiveScene().buildIndex);
        savingAndLoadingManager.Save();
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }
}
