using System;
using System.Collections;
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

    private void Start()
    {
        Instance = this;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBar.value = currentHealth / maxHealth;
    }

    public void UseAbility(int index, float length)
    {
        switch (index)
        {
            case 1:
                StartCoroutine(UseAbilityEnum(ability1, length));
                break;
            case 2:
                StartCoroutine(UseAbilityEnum(ability2, length));
                break;
            case 3:
                StartCoroutine(UseAbilityEnum(ability3, length));
                break;
            case 4:
                StartCoroutine(UseAbilityEnum(ability4, length));
                break;
            case 5:
                StartCoroutine(UseAbilityEnum(ability5, length));
                break;
        }
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
        switch (index)
        {
            case 1:
                StartCoroutine(UseCooldownEnum(ability1, cooldown));
                break;
            case 2:
                StartCoroutine(UseCooldownEnum(ability2, cooldown));
                break;
            case 3:
                StartCoroutine(UseCooldownEnum(ability3, cooldown));
                break;
            case 4:
                StartCoroutine(UseCooldownEnum(ability4, cooldown));
                break;
            case 5:
                StartCoroutine(UseCooldownEnum(ability5, cooldown));
                break;
        }
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

}
