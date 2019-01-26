using System.Collections;
using UnityEngine;

public abstract class BaseAbility : ScriptableObject
{
    [HideInInspector]
    public EntityAbility actionManager;

    [Header("Base Ability Attributes")]
    public string abilityName;

    [Header("Abilty Activation Attributes")]
    public AbilityActivationInfo abilityActivation;

    [Header("Ability Cooldown Info")]
    public AbilityCooldownInfo abilityCooldown;

    [Header("Base Ability Actions Attributes")]
    public AbilityEvent[] abilityActions;

    [Space(10)]
    public BaseAbilityLogic abilityLogic;

    public virtual void InitializeAbility(GameObject newAbilityObject)
    {
        abilityLogic = newAbilityObject.GetComponent<BaseAbilityLogic>();

        if (abilityLogic.ReturnTotalActionTime() > abilityCooldown.cooldownTime)
        {
            Debug.LogWarning("Error, Actions duration is longer than cooldown on " + abilityName);
        }

        abilityLogic.Initialize(abilityActions, this);
    }

    public virtual void ActivateAbility()
    {
        if (abilityLogic != null)
        {
            abilityLogic.ActivateAbility();
        }
    }

    public virtual MovementInfo ReturnActionMovement()
    {
        if (abilityLogic.isAbilityActive)
        {
            return abilityLogic.ReturnAbilityEntityMovementInfo();
        }

        return new MovementInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f, Space.World, false, false);
    }
}

[System.Serializable]
public struct AbilityEvent
{
    [Header("Ability Movements")]
    public AbilityMovementInfo abilityMovement;

    [Header("Ability Interactions")]
    public AbilityInteractionInfo abilityInteraction;

    [Header("Ability Effects")]
    public AbilityEffectInfo abilityEffect;
}

[System.Serializable]
public enum ABILITY_COOLDOWN_TYPE
{
    TIMED,
    TOGGLE,
    INFINITE
}

[System.Serializable]
public struct AbilityMovementInfo
{
    [Header("Base Ability Attributes")]
    public float abilityTime;

    [Space(10)]
    public bool isInfinite;

    [Header("Ability Movement Attributes")]
    public Vector3 movementDirection;

    [Space(10)]
    public AnimationCurve movementCurve;

    [Space(10)]
    public float actionMoveDistance;

    [Space(10)]
    [Range(0, 1)]
    public float actionInputModifier;

    [Space(10)]
    public Space movementSpace;

    [Space(10)]
    public bool useGravity;

    [Space(10)]
    public bool useForward;
}

[System.Serializable]
public struct AbilityEffectInfo
{
    [Header("Base Ability Attributes")]
    public float abilityTime;

    [Space(10)]
    public bool isInfinite;

    [Header("Ability Effect Attributes")]
    public float damageValue;

    [Space(10)]
    public float effectValue;

    [Space(10)]
    public float effectDuration;
}

[System.Serializable]
public struct AbilityInteractionInfo
{
    [Header("Base Ability Attributes")]
    public float abilityTime;

    [Space(10)]
    public bool isInfinite;

    [Header("Ability Interaction Attributes")]
    public AbilityInteractionMovementInfo abilityInteractionMovementInfo;

    [Header("Ability Collider Attributes")]
    public AbilityColliderInfo abilityColliderInfo;

    [Header("Ability Damage Attributes")]
    public AbilityDamageInfo abilityDamageInfo;
}

[System.Serializable]
public struct AbilityInteractionMovementInfo
{
    [Header("Ability Movement Attributes")]
    public Vector3 movementDirection;

    [Space(10)]
    public float actionMoveDistance;

    [Space(10)]
    public AnimationCurve movementCurve;

    [Space(10)]
    public Vector3 movementStartPosition;

    [Space(10)]
    public ABILITY_MOVEMENT_TYPE movementType;

    [Space(10)]
    public Space movementSpace;

    [Space(10)]
    public bool resetColliderToBasePosition;
}

[System.Serializable]
public struct AbilityColliderInfo
{
    [Header("Ability Collider Attributes")]
    public ABILITY_COLLIDER_TYPE abilityColliderType;

    [Space(10)]
    public Vector3 colliderSize;

    [Space(10)]
    public float colliderRadius;

    [Space(10)]
    public LayerMask colliderHitMask;
}

[System.Serializable]
public struct AbilityDamageInfo
{
    public EntityDamageInfo damageInfo;
}

[System.Serializable]
public enum ABILITY_TYPE
{
    NONE,
    INTERACTION,
    MOVEMENT,
    EFFECT,
}

[System.Serializable]
public enum ABILITY_ACTIVATION_TYPE
{
    TRIGGER,
    HELD,
    TOGGLE
}

[System.Serializable]
public enum ABILITY_COLLIDER_TYPE
{
    NONE,
    BOX,
    SPHERE,
}

[System.Serializable]
public struct ActiveAbility
{
    [Header("Active Action")]
    public BaseAbility activeAction;

    [Space(10)]
    public int abilityInputIndex;

    public ActiveAbility(BaseAbility newActiveAbility, int newAbilityInputIndex)
    {
        this.activeAction = newActiveAbility;
        this.abilityInputIndex = newAbilityInputIndex;
    }
}

[System.Serializable]
public struct AbilityCooldown
{
    [Header("Ability")]
    public BaseAbility ability;

    [Space(10)]
    public float targetCooldownTime;

    public AbilityCooldown(BaseAbility newAbility)
    {
        this.ability = newAbility;
        this.targetCooldownTime = newAbility.abilityCooldown.cooldownTime + Time.time;
    }
}

[System.Serializable]
public struct AbilityCooldownInfo
{
    [Header("Ability Cooldown Info")]
    public ABILITY_COOLDOWN_TYPE cooldownType;

    [Space(10)]
    public float cooldownTime;
}

[System.Serializable]
public struct AbilityActivationInfo
{
    [Header("Base Ability Activation Info")]
    public ABILITY_ACTIVATION_TYPE activationType;

    [Space(10)]
    public int activationCount;
}

[System.Serializable]
public enum ABILITY_MOVEMENT_TYPE
{
    NONE,
    VELOCITY,
    POSITION
}
