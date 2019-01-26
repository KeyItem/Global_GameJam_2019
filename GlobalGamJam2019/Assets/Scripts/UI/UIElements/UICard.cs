using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UICard : UIElement
{
   [FormerlySerializedAs("cardImage")] [Header("UI Card Attributes")] 
   public Image cardBackgroundImage;

   public Image cardForegroundImage;

   [Space(10)] 
   public Text cardText;

   public override void InitializeUIElement()
   {
        
   }

   public void ImportNewCard(BaseAbilityCard newAbilityCard)
   {
       if (newAbilityCard.abilityCardBackgroundSprite == null)
       {
           Debug.LogWarning(newAbilityCard.abilityName + " is missing it's background sprite!");
       }
       else
       {
           cardBackgroundImage.sprite = newAbilityCard.abilityCardBackgroundSprite;
       }
       
       if (newAbilityCard.abilityCardForegroundSprite == null)
       {
           Debug.LogWarning(newAbilityCard.abilityName + " is missing it's foreground sprite!");
       }
       else
       {
           cardForegroundImage.sprite = newAbilityCard.abilityCardForegroundSprite;
       }   

       cardText.text = newAbilityCard.abilityName;
   }
}
