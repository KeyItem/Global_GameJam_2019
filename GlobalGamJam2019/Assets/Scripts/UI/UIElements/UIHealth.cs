using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : UIElement
{
    [Header("UI Health Attributes")] 
    public Image healthBarImage;

    public override void InitializeUIElement()
    {
        healthBarImage = GetComponent<Image>();
    }
    
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }
}
