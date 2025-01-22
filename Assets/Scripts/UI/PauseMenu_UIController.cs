using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu_UIController : MonoBehaviour
{
    private enum MenuState { None, PauseMenu, OptionsMenu }
    private MenuState currentMenuState;

    public GameObject PauseMenu;
    public GameObject OptionsMenu;
    
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Start()
    {
        SwitchMenuState(MenuState.None);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (LevelManager.Instance?.AbilityScreenActive == false)
            {
                SwitchMenuState(MenuState.PauseMenu);
            }
        }
    }

    public void OnPlayButtonClicked()
    {
        SwitchMenuState(MenuState.None);
    }
    public void OptionsButtonClicked()
    {
        SwitchMenuState(MenuState.OptionsMenu);
        musicVolumeSlider.value = AudioManager.Instance.musicSource.volume;
        sfxVolumeSlider.value = AudioManager.Instance.sfxSource.volume;
    }
    public void OnQuitToMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void OnBackButtonClicked()
    {
        SwitchMenuState(MenuState.PauseMenu);
    }

    public void OnMusicVolumeSliderChanged()
    {
        AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);
    }
    public void OnSFXVolumeSliderChanged()
    {
        AudioManager.Instance.SetSFXVolume(sfxVolumeSlider.value);
    }

    private void SwitchMenuState(MenuState newState)
    {
        PauseMenu.SetActive(newState == MenuState.PauseMenu);
        OptionsMenu.SetActive(newState == MenuState.OptionsMenu);

        if (newState == MenuState.None)
        {
            Time.timeScale = 1.0f;
            
            // Lock cursor for FPS-style camera controls
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0.0f;
            
            // Lock cursor for FPS-style camera controls
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
