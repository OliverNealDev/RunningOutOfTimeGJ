using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public bool abilityScreenActive;
    
    [SerializeField] private float timer = 30f;
    [SerializeField] private float timerInterval = 30f;
    
    [SerializeField] private AudioClip levelMusic;
    
    private PlayerController player;
    
    void Start()
    {
        Instance = this;
        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID)[0];
        
        AudioManager.Instance.PlaySong(levelMusic);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        
        AbilityUIController.Instance.UpdateTimer(timer);

        if (timer <= 0f)
        {
            timer = timerInterval;
            abilityScreenActive = true;
            StartCoroutine(AbilityUIController.Instance.DeactivateAbilityScreen(player));
        }
        else if (timer >= timerInterval + 5f)
        {
            timer = timerInterval;
            StartCoroutine(AbilityUIController.Instance.ActivateAbilityScreen(player));
        }
    }

    public void EnemyDied()
    {
        timer += 1f;
    }
}
