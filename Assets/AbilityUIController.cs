using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIController : MonoBehaviour
{
    public static AbilityUIController Instance;
    
    [SerializeField] private Image ability1;
    [SerializeField] private Image ability2;
    [SerializeField] private Image ability3;
    [SerializeField] private Image ability4;
    [SerializeField] private Image ability5;

    [SerializeField] private Slider healthBar;
    
    [SerializeField] private GameObject abilityPanel;
    [SerializeField] private GameObject hud;
    
    private int abilitySelected;
    private List<int> abilitiesOwned = new List<int>();
    
    private void Start()
    {
        Instance = this;
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

    public IEnumerator ActivateAbilityScreen(PlayerController player)
    {
        Time.timeScale = 0;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        abilityPanel.SetActive(true);
        hud.SetActive(false);

        while (abilitySelected == 0)
        {
            yield return null;
        }

        Debug.Log(abilitySelected);
        
        player.ToggleAbility(false, abilitySelected);
        abilitySelected = 0;
        
        abilityPanel.SetActive(false);
        hud.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        Time.timeScale = 1;
    }

    public void OnAbilitySelected(int index)
    {
        if (abilitiesOwned.Contains(index))
        {
            return;
        }
        abilitySelected = index;
        abilitiesOwned.Add(index);
    }

}
