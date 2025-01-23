using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AbilityUIController : MonoBehaviour
{
    private enum AbilityMenuState
    {
        None = 0,
        Adding = 1,
        Removing = 2,
    }
    
    private AbilityMenuState abilityMenuState = AbilityMenuState.None;
    
    public static AbilityUIController Instance;

    [SerializeField] private TextMeshProUGUI timer;
    
    [SerializeField] private int abilityesAmount = 5;
    
    [SerializeField] private Image ability1;
    [SerializeField] private Image ability2;
    [SerializeField] private Image ability3;
    [SerializeField] private Image ability4;
    [SerializeField] private Image ability5;
    
    [SerializeField] private List<Image> abilityXs = new List<Image>();
    
    [SerializeField] private Slider healthBar;
    
    [SerializeField] private GameObject abilityPanel;
    [SerializeField] private TextMeshProUGUI abilityPanelText;
    [SerializeField] private GameObject hud;
    
    private int abilityToRemove;
    private List<int> abilitiesOwned = new List<int>();
    
    private int abilityToGain;

    private void Start()
    {
        Instance = this;

        for (int i = 0; i < abilityesAmount; i++)
        {
            abilitiesOwned.Add(i + 1);
        }
    }

    void Update()
    {
    }
    
    public void UpdateTimer(float time)
    {
        if (time <= 0)
        {
            timer.color = Color.red;
        }
        else if (time >= 30f)
        {
            timer.color = Color.green;
        }
        else
        {
            timer.color = Color.white;
        }
        timer.text = time.ToString("0.00");
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBar.value = currentHealth / maxHealth;
    }

    private Image IndexToImage(int index)
    {
        Image result = null;
        switch (index)
        {
            case 1:
                result = ability1;
                break;
            case 2:
                result = ability2;
                break;
            case 3:
                result = ability3;
                break;
            case 4:
                result = ability4;
                break;
            case 5:
                result = ability5;
                break;
        }
        return result;
    }
    
    public void UseAbility(int index, float length)
    {
        StartCoroutine(UseAbilityEnum(IndexToImage(index), length));
    }

    private IEnumerator UseAbilityEnum(Image image, float length)
    {
        image.color = new Color32(255, 150, 0, 70);
        image.transform.localScale = Vector3.one;
        
        yield return new WaitForSeconds(length);
        
        image.color = new Color32(0, 0, 0, 200);
        image.transform.localScale = Vector3.right + Vector3.forward;
    }
    
    public void CooldownAbility(int index, float cooldown)
    {
        StartCoroutine(UseCooldownEnum(IndexToImage(index), cooldown));
    }

    private IEnumerator UseCooldownEnum(Image image, float cooldown)
    {
        Vector3 currentScale = Vector3.one;
        float currentCooldown = cooldown;
        
        while (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            currentScale.y = currentCooldown / cooldown;
            image.color = new Color32(0, 0, 0, 200);
            image.transform.localScale = currentScale;
            yield return null;
        }
        image.transform.localScale = Vector3.right + Vector3.forward;
    }

    public IEnumerator DeactivateAbilityScreen(PlayerController player)
    {
        Debug.Log(abilitiesOwned.Count);
        
        if (abilitiesOwned.Count <= 1)
        {
            abilityPanelText.text = "Game Over";
            
            Time.timeScale = 0;
        
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        
            abilityPanel.SetActive(true);
            
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    SceneManager.LoadScene(0);
                }
                yield return null;
            }
        }
        else
        {
            abilityPanelText.text = "Select an Ability to Discard";
        }
        
        abilityMenuState = AbilityMenuState.Removing;
        
        Time.timeScale = 0;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        abilityPanel.SetActive(true);

        abilityToRemove = 0;
        
        while (abilityToRemove == 0)
        {
            yield return null;
        }

        Debug.Log(abilityToRemove);
        
        player.ToggleAbility(false, abilityToRemove);
        abilityXs[abilityToRemove - 1].gameObject.SetActive(true);
        
        abilityToRemove = 0;
        
        abilityPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        LevelManager.Instance.abilityScreenActive = false;
        
        Time.timeScale = 1;
        
        abilityMenuState = AbilityMenuState.None;
    }
    
    public IEnumerator ActivateAbilityScreen(PlayerController player)
    {
        if (abilitiesOwned.Count < 5)
        {
            abilityMenuState = AbilityMenuState.Adding;
            
            Time.timeScale = 0;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            abilityPanelText.text = "Select an Ability to Gain";
            
            abilityPanel.SetActive(true);

            abilityToGain = 0;
            
            while (abilityToGain == 0)
            {
                yield return null;
            }

            Debug.Log(abilityToGain);
            
            player.ToggleAbility(true, abilityToGain);
            abilityXs[abilityToGain - 1].gameObject.SetActive(false);
            
            abilityToGain = 0;
            
            abilityPanel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            LevelManager.Instance.abilityScreenActive = false;
            
            Time.timeScale = 1;
            
            abilityMenuState = AbilityMenuState.None;
        }
    }

    public void OnAbilitySelected(int index)
    {
        switch (abilityMenuState)
        {
            case AbilityMenuState.None:
                break;
            case AbilityMenuState.Adding:
                if (!abilitiesOwned.Contains(index))
                {
                    abilityToGain = index;
                    abilitiesOwned.Add(index);
                }
                break;
            case AbilityMenuState.Removing:
                if (abilitiesOwned.Contains(index))
                {
                    abilityToRemove = index;
                    abilitiesOwned.Remove(index);
                }
                break;
        }
    }
}
