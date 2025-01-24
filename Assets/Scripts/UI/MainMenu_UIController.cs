#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_UIController : MonoBehaviour
{
    private enum MenuState { None, MainMenu, OptionsMenu }
    private MenuState currentMenuState;

    [SerializeField] private AudioClip hellschampiondotwav;

    public GameObject MainMenu;
    public GameObject LevelSelectionMenu;
    public GameObject OptionsMenu;
    
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SwitchMenuState(MenuState.MainMenu);
    }

    private Coroutine play;
    public void OnPlayButtonClicked()
    {
        play = StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        AudioManager.Instance.sfxSource.PlayOneShot(hellschampiondotwav);
        
        yield return new WaitForSeconds(hellschampiondotwav.length);
        
        SceneManager.LoadScene("SampleScene");
    }
    public void OptionsButtonClicked()
    {
        SwitchMenuState(MenuState.OptionsMenu);
        musicVolumeSlider.value = AudioManager.Instance.musicSource.volume;
        sfxVolumeSlider.value = AudioManager.Instance.sfxSource.volume;
    }
    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OnBackButtonClicked()
    {
        SwitchMenuState(MenuState.MainMenu);
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
        MainMenu.SetActive(newState == MenuState.MainMenu);
        OptionsMenu.SetActive(newState == MenuState.OptionsMenu);
    }
}