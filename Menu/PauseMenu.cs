using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private static bool gameIsPaused = false;

    private bool interacting = false;

    [Header("References")]
    [SerializeField] private Image crosshair;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject hotBarUI;
    [SerializeField] private GameObject interactTextHolder;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject interactor = null;
    [SerializeField] private SavingAndLoadingManager savingAndLoadingManager;

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
        else if(!interacting)
        {
            Pause();
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        hotBarUI.SetActive(true);
        interactor.SetActive(true);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        Time.maximumDeltaTime = 1 / 3f;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        crosshair.enabled = true;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); //set menu to show
        hotBarUI.SetActive(false);
        inventoryUI.SetActive(false);
        interactor.SetActive(false);
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

    public void LaunchSettingsMenu()
    {
        PlayerPrefs.SetInt("PreviousScene", SceneManager.GetActiveScene().buildIndex);
        savingAndLoadingManager.Save();
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void SetInteracting(bool isPlayerInteracting)
    {
        interacting = isPlayerInteracting;
    }
}
