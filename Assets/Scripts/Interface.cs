using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Interface : MonoBehaviour
{
    public GameObject menuPause;
    public GameObject pausePanel;
    public GameObject menuSettings;
    public GameObject settingsPanel;
    public static bool isPaused;
    public static bool isSettings;
    
    public AudioMixer audioMixer;

    public Sprite buttonImage;
    public Sprite buttonImage2;
    public Button button;

    public Sprite buttonVolumeImage;
    public Sprite buttonVolumeImage2;
    public Button buttonVolume;

    public Dropdown resolutionDropdown;
    Resolution[] resolutions;

    bool isVolume;

    // Start is called before the first frame update
    void Start()
    {
        menuPause.SetActive(false);
        pausePanel.SetActive(false);

        menuSettings.SetActive(false);
        settingsPanel.SetActive(false);

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();

        int currentResolution = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && 
            resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolution = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();

        audioMixer.SetFloat("volume", -20);
        isVolume = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !isSettings)
            {
                mPause();
            }
    }

     public void Pause()
    {
        menuPause.SetActive(true);
        //Time.timeScale = 0f;
        isPaused = true;
        button.image.sprite = buttonImage;
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        menuPause.SetActive(false);
        //Time.timeScale = 1f;
        isPaused = false;
        button.image.sprite = buttonImage2;
        pausePanel.SetActive(false);
    }

    public void Restart()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
        Time.timeScale = 1;
        Board.preGame = false;
        isPaused = false;
        QualitySettings.SetQualityLevel(0);
        Board.turn = 0;
    }

    public void mPause()
    {
        if(isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Settings()
    {
        menuSettings.SetActive(true);
        settingsPanel.SetActive(true);
        //Time.timeScale = 1f;
        isPaused = true;
        button.image.sprite = buttonImage;
        isSettings = true;
    }

    public void Return()
    {
        menuSettings.SetActive(false);
        settingsPanel.SetActive(false);
        //Time.timeScale = 0f;
        isSettings = false;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool fullScreen)
    {
        Screen.fullScreen = fullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume2()
    {
        if(isVolume)
        {
            audioMixer.SetFloat("volume", -20);
            buttonVolume.image.sprite = buttonVolumeImage2;
            isVolume = false;
        }
        else
        {
            audioMixer.SetFloat("volume", -80);
            buttonVolume.image.sprite = buttonVolumeImage;
            isVolume = true;
        }
    }
}
