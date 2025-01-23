using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public bool abilityScreenActive;
    
    [SerializeField] private float timer = 30f;
    [SerializeField] private float timerInterval = 30f;
    
    private PlayerController player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID)[0];
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        
        AbilityUIController.Instance.UpdateTimer(timer);

        if (timer <= -5f)
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

        //Debug
        if (Input.GetKeyDown(KeyCode.P))
        {
            timer += 10f;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            timer -= 10f;
        }
        //
    }

    public void EnemyDied()
    {
        timer += 10f;
    }
}
