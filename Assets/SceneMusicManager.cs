using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicManager : MonoBehaviour
{
    public static SceneMusicManager Instance { get; private set; }
    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private AudioClip level1Music;
    [SerializeField] private AudioClip level2Music;

    private void Awake()
    {
        // Singleton pattern implementation to prevent duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy a duplicate instance
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep this object between scenes
    }
    
    private void Start()
    {
        // Register scene change event listener
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Play default music for the starting scene
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        // Unregister scene change event listener
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = defaultMusic;

        // Assign the appropriate song for the scene
        if (sceneName == "Lv_01")
        {
            clipToPlay = level1Music;
        }
        else if (sceneName == "Lv_02") // Example for when level 2 is made, delete this comment when it is
        {
            clipToPlay = level2Music;
        }

        // Play the assigned music
        AudioManager.Instance.PlaySong(clipToPlay);
    }
}