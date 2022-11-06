using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio; //has to be used to reference audio mixer
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider mouseSlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private TMP_Text sensText;
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;
    [SerializeField] private Toggle fullscreenToggle;

    private float masterVolume;
    private float musicVolume;
    private float sfxVolume;


    [SerializeField] private InputActionAsset playerInput = null;

    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    private const string RebindsKey = "rebinds";

    private void Start()
    {
        //Get only distinct heights and widths from array
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        resolutionDropdown.ClearOptions(); //clear out all options currently in dropdown

        List<string> options = new List<string>(); // create list of strings which is the options

        int currentResoltuionIndex = 0;
        for(int i = 0; i<resolutions.Length; i++)  //loop through each element in resolutions array
        {
            string option = resolutions[i].width + "x" + resolutions[i].height; // for each of them create a nicely formatted string that displays resolution
            options.Add(option); // We add it to our options list 

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResoltuionIndex = i; //adds the value which is equal to the correct resolution to variable
            }
        }

        resolutionDropdown.AddOptions(options); // add options list to resolution dropdown.
        resolutionDropdown.value = currentResoltuionIndex;  //sets game resolution to correct resolution
        resolutionDropdown.RefreshShownValue(); //Refersh what is shown on the screen

        //Set volumes (loading volumes)
        audioMixer.GetFloat("masterVolume", out masterVolume);
        audioMixer.GetFloat("musicVolume", out musicVolume);
        audioMixer.GetFloat("sfxVolume", out sfxVolume);
        masterSlider.value = masterVolume;
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
        masterVolumeText.text = ((1.25f * masterVolume) + 100).ToString();
        musicVolumeText.text = ((1.25f * musicVolume) + 100).ToString();
        sfxVolumeText.text = ((1.25f * sfxVolume) + 100).ToString();

        //Loading sensitivity
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            mouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        sensText.text = mouseSlider.value.ToString();

        //Loading graphics text
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
        graphicsDropdown.RefreshShownValue();

        //Loading fullscreen toggle
        fullscreenToggle.isOn = Screen.fullScreen;

    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex]; // variable type resolution created with name resolution in function so can be used to take height and width from array
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetMusicVolume(float volume)
    {
        musicVolumeText.text = ((1.25f * volume) + 100).ToString();
        audioMixer.SetFloat("musicVolume", volume);  //"musicVolume" is name of sub audioMixer set from the inspector and volume is name of variable which is inputted in function.  
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolumeText.text = ((1.25f * volume) + 100).ToString();
        audioMixer.SetFloat("sfxVolume", volume);  //"sfxVolume" is name of sub audioMixer set from the inspector and volume is name of variable which is inputted in function.  
    }

    public void SetMainVolume(float volume)
    {
        masterVolumeText.text = ((1.25f * volume)+100).ToString();
        audioMixer.SetFloat("masterVolume", volume);  //"masterVolume" is name of audioMixer set from the inspector and volume is name of variable which is inputted in function.  
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        sensText.text = sensitivity.ToString();
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex); //presets by unity can be adjusted and have different qualityIndex so they match up, you can add your own.
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen; //simple just makes it whatever FullScreen is, i.e. when toggle own it is set to true so fullscreen is activated.
    }

    public void BackButton()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("PreviousScene"));                
    }

    public void ResetAllBinidingsButton()
    {
        foreach(InputActionMap map in playerInput.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }

    public void SaveButton()
    {
        string rebinds = playerInput.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(RebindsKey, rebinds);
    }

}
