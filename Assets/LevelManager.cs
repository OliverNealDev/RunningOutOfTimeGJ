using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    public bool AbilityScreenActive { get; private set; }
    
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

        if (timer <= 0)
        {
            timer = timerInterval;
            AbilityScreenActive = true;
            StartCoroutine(AbilityUIController.Instance.ActivateAbilityScreen(player));
        }
    }
}
