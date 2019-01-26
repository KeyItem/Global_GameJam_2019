using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbilityCard : ScriptableObject
{
    [Header("Ability Card Attributes")]
    public BaseAbility cardAbility;

    [Space(10)]
    public string abilityName;

    [Space(10)]
    public int abilityID;

    [Header("Ability Card Visual Attributes")]
    public Sprite abilityCardBackgroundSprite;

    [Space(10)]
    public Sprite abilityCardForegroundSprite;
}
