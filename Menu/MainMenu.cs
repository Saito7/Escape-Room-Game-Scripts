using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject resetMenu;
    private const string RebindsKey = "rebinds";

    public void PlayGame()
    {
        if (PlayerPrefs.HasKey("currentScene"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("currentScene"));
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }

    public void Reset()
    {
        //Delete the save file
        if (File.Exists($"{Application.persistentDataPath}/SaveData.txt"))
        {
            File.Delete($"{Application.persistentDataPath}/SaveData.txt");
        }
        //Delete all persistent player prefs data
        PlayerPrefs.DeleteKey("TilePuzzle");
        PlayerPrefs.DeleteKey(RebindsKey);
        PlayerPrefs.DeleteKey("Maze");
        PlayerPrefs.DeleteKey("Historic");
        PlayerPrefs.DeleteKey("currentScene");
    }

    public void LaunchSettingsMenu()
    {
        //save the scene the player was previously in
        PlayerPrefs.SetInt("PreviousScene", SceneManager.GetActiveScene().buildIndex);
        //load settings menu scene
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void openResetMenu()
    {
        resetMenu.SetActive(true);
    }

    public void confirmReset()
    {
        Reset();
        resetMenu.SetActive(false);
    }

    public void cancelReset()
    {
        resetMenu.SetActive(false);
    }

}
