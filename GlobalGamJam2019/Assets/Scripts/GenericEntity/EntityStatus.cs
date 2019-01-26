using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatus : MonoBehaviour
{
    [Header("Entity Status Attributes")]
    public float entityHealth;

    private EntityStatusInfo entityStatusInfo;

    [Header("Entity Stats Attributes")]
    public EntityStatsInfo entityStats;

    [Header("Entity Active Status Effects")]
    public List<ActiveStatusEffect> activeStatusEffects = new List<ActiveStatusEffect>();

    private MovementInfo statusEffectMovementInfo = new MovementInfo();
    private MovementModifierInfo statusEffectMovementModifierInfo = new MovementModifierInfo();

    private void Start()
    {
        InitializeResources();
    }

    private void InitializeResources()
    {
        entityHealth = entityStats.entityMaxHealth;
    }

    public virtual void ManageStatusEffects()
    {
        ManageActiveStatusEffects();
        ManageActiveStatusEffectsTimings();
    }

    public virtual void ManageActiveStatusEffects()
    {
        statusEffectMovementInfo = new MovementInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f, Space.World, false, false);
        statusEffectMovementModifierInfo = new MovementModifierInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f);

        if (activeStatusEffects.Count > 0)
        {
            for (int i = 0; i < activeStatusEffects.Count; i++)
            {
                switch (activeStatusEffects[i].statusEffect.statusEffect)
                {
                    case STATUS_EFFECT.SLOWED:
                        statusEffectMovementModifierInfo = ReturnStatusEffectMovementModifierInfo(activeStatusEffects[i]);
                        break;

                    case STATUS_EFFECT.STUNNED:
                        statusEffectMovementModifierInfo = ReturnStatusEffectMovementModifierInfo(activeStatusEffects[i]);
                        break;

                    case STATUS_EFFECT.KNOCKBACK:
                        statusEffectMovementInfo = ReturnStatusEffectMovementInfo(activeStatusEffects[i]);
                        break;
                }
            }
        }
    }

    public virtual void ManageActiveStatusEffectsTimings()
    {
        if (activeStatusEffects.Count > 0)
        {
            List<ActiveStatusEffect> activeStatusEffectsToRemove = new List<ActiveStatusEffect>(); 

            for (int i = 0; i < activeStatusEffects.Count; i++)
            {
                if (Time.time > activeStatusEffects[i].statusEffectEndTime)
                {
                    activeStatusEffectsToRemove.Add(activeStatusEffects[i]);
                }
            }

            for (int o = 0; o < activeStatusEffectsToRemove.Count; o++)
            {
                activeStatusEffects.Remove(activeStatusEffectsToRemove[o]);
            }
        }
    }

    public virtual Vector3 ReturnStatusMovementVelocity(ActiveStatusEffect activeStatusEffect)
    {
        Vector3 newStatusVelocity = activeStatusEffect.statusHitDirection;

        float newStatusTargetVelocity = activeStatusEffect.statusEffect.effectStrength / activeStatusEffect.statusEffect.effectLength;

        float speedMultiplier = Helper.ReturnEvalutationOfAnimationCurve(Helper.ReturnTimeRatio(Time.time, activeStatusEffect.statusEffectStartTime, activeStatusEffect.statusEffectEndTime), activeStatusEffect.statusEffect.effectLength, activeStatusEffect.statusEffect.effectCurve);

        return (newStatusVelocity * newStatusTargetVelocity) * speedMultiplier;
    }

    public virtual MovementInfo ReturnStatusEffectMovementInfo(ActiveStatusEffect activeStatusEffect)
    {
        MovementInfo newStatusMovement = new MovementInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f, Space.World, false, false);

        switch (activeStatusEffect.statusEffect.statusEffect)
        {
            case STATUS_EFFECT.KNOCKBACK:
                newStatusMovement = new MovementInfo(MOVEMENT_SOURCE.STATUS, ReturnStatusMovementVelocity(activeStatusEffect), 0f, Space.World, false, false);
                break;
        }

        return newStatusMovement;
    }

    public virtual MovementModifierInfo ReturnStatusEffectMovementModifierInfo(ActiveStatusEffect activeStatusEffect)
    {
        MovementModifierInfo newStatusMovementModifier = new MovementModifierInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f);

        switch (activeStatusEffect.statusEffect.statusEffect)
        {
            case STATUS_EFFECT.SLOWED:
                newStatusMovementModifier = new MovementModifierInfo(MOVEMENT_SOURCE.STATUS, Vector3.zero, activeStatusEffect.statusEffect.effectStrength);
                break;

            case STATUS_EFFECT.STUNNED:
                newStatusMovementModifier = new MovementModifierInfo(MOVEMENT_SOURCE.STATUS, Vector3.zero, 0f);
                break;
        }

        return newStatusMovementModifier;
    }

    public virtual List<ActiveStatusEffect> ReturnActiveStatusEffects()
    {
        return activeStatusEffects;
    }

    public virtual void TakeDamage(EntityDamageInfo damageInfo, Vector3 hitPosition)
    {
        if (damageInfo.damageStatusEffect.statusEffect != STATUS_EFFECT.NONE)
        {
            AddEffect(damageInfo.damageStatusEffect, hitPosition);
        }

        RemoveHealth(CalculatedResistedDamage(damageInfo));

        if (IsEntityDead())
        {
            Debug.Log(gameObject.name + " is Dead");
        }
    }

    public virtual void AddEffect(EntityStatusEffect newStatusEffect, Vector3 hitPosition)
    {
        int sameStatusEffectOnEntityCount = ReturnSameStatusEffectOnEntity(newStatusEffect);
        float effectEffectiveness = 1;

        if (sameStatusEffectOnEntityCount != 0)
        {
            effectEffectiveness /= sameStatusEffectOnEntityCount;

            return; // Exit for now until implementation of stacking status effects
        }

        activeStatusEffects.Add(new ActiveStatusEffect(newStatusEffect, Time.time, transform.position, hitPosition));
    }

    public virtual float CalculatedResistedDamage(EntityDamageInfo damageInfo)
    {
        float resistAmount = 0f;

        if (damageInfo.damageType == DAMAGE_TYPE.TRUE)
        {
            return damageInfo.damageValue;
        }

        if (damageInfo.damageType == DAMAGE_TYPE.PHYSICAL && entityStats.defenseAttributes.baseEntityPhysicalDefense == 0)
        {
            return damageInfo.damageValue;
        }
        else if (damageInfo.damageType == DAMAGE_TYPE.MAGICAL && entityStats.defenseAttributes.baseEntityMagicalDefense == 0)
        {
            return damageInfo.damageValue;
        }

        if (damageInfo.damageType == DAMAGE_TYPE.PHYSICAL)
        {
            resistAmount = damageInfo.damageValue * entityStats.defenseAttributes.baseEntityPhysicalDefense;
        }
        else if (damageInfo.damageType == DAMAGE_TYPE.MAGICAL)
        {
            resistAmount = damageInfo.damageValue * entityStats.defenseAttributes.baseEntityMagicalDefense;
        }

        return damageInfo.damageValue - resistAmount;
    }

    public virtual void AddHealth(float healthValue)
    {
        float healthValueToAdd = healthValue;

        if (entityHealth + healthValue > entityStats.entityMaxHealth)
        {
            healthValueToAdd = (entityHealth + healthValue) - entityStats.entityMaxHealth;
        }

        entityHealth += healthValueToAdd;

        UpdateEntityStatusInfo();
    }

    public virtual void RemoveHealth(float healthValue)
    {
        if (entityHealth - healthValue < 0)
        {
            entityHealth = 0;

            return;
        }

        entityHealth -= healthValue;

        UpdateEntityStatusInfo();
    }

    public virtual int ReturnSameStatusEffectOnEntity(EntityStatusEffect newStatusEffect)
    {
        int sameStatusEffectCount = 0;

        for (int i = 0; i < activeStatusEffects.Count; i++)
        {
            if (activeStatusEffects[i].statusEffect.statusEffect == newStatusEffect.statusEffect)
            {
                sameStatusEffectCount++;
            }
        }

        return sameStatusEffectCount;
    }

    public virtual bool IsEntityAlreadySufferingFromStatusEffect(EntityStatusEffect targetStatusEffect)
    {
        if (ReturnSameStatusEffectOnEntity(targetStatusEffect) > 0)
        {
            return true;
        }

        return false;
    }

    public virtual bool IsEntityDead()
    {
        if (entityHealth > 0)
        {
            return false;
        }

        return true;
    }

    public virtual void UpdateEntityStatusInfo()
    {
        entityStatusInfo = new EntityStatusInfo(entityHealth, entityStats.entityMaxHealth, activeStatusEffects);
    }

    public virtual EntityStatusInfo ReturnEntityStatusInfo()
    {
        return entityStatusInfo;
    }

    public virtual MovementInfo ReturnEntityStatusMovement()
    {
        return statusEffectMovementInfo;
    }

    public virtual MovementModifierInfo ReturnEntityStatusMovementModifer()
    {
        return statusEffectMovementModifierInfo;
    }
}

[System.Serializable]
public struct EntityStatsInfo
{
    [Header("Base Entity Status Attributes")]
    public float entityMaxHealth;

    [Header("Base Entity Stats Attributes")]
    public EntityAttackAttributes attackAttributes;

    [Space(10)]
    public EntityDefenseAttributes defenseAttributes;
}

[System.Serializable]
public struct EntityStatusInfo
{
    [Header("Entity Status Info Attributes")]
    public float entityHealth;

    [Space(10)]
    public float entityHealthPercent;

    [Space(10)]
    public List<ActiveStatusEffect> entityActiveStatusEffects;

    public EntityStatusInfo (float newCurrentHealth, float newMaxHealth, List<ActiveStatusEffect> newActiveStatusEffects)
    {
        this.entityHealth = newCurrentHealth;
        
        if (newCurrentHealth == 0f)
        {
            this.entityHealthPercent = 0f;
        }
        else
        {
            this.entityHealthPercent = newCurrentHealth / newMaxHealth;
        }

        this.entityActiveStatusEffects = newActiveStatusEffects;
    }
}

[System.Serializable]
public struct EntityAttackAttributes
{
    [Header("Entity Attack Attributes")]
    [Range(-1, 1)]
    public float baseEntityPhysicalAttack;

    [Space(10)]
    [Range(-1, 1)]
    public float baseEntityMagicalAttack;
}

[System.Serializable]
public struct EntityDefenseAttributes
{
    [Header("Entity Defense Attributes")]
    [Range(-1, 1)]
    public float baseEntityPhysicalDefense;

    [Space(10)]
    [Range(-1, 1)]
    public float baseEntityMagicalDefense;
}

[System.Serializable]
public struct EntityDamageInfo
{
    [Header("Entity Damage Values")]
    public float damageValue;

    [Space(10)]
    public DAMAGE_TYPE damageType;

    [Space(10)]
    public EntityStatusEffect damageStatusEffect;

    public EntityDamageInfo (int newDamageValue, DAMAGE_TYPE newDamageType, EntityStatusEffect newStatusEffect)
    {
        this.damageValue = newDamageValue;
        this.damageType = newDamageType;
        this.damageStatusEffect = newStatusEffect;
    }
}

[System.Serializable]
public struct EntityStatusEffect
{
    [Header("Status Effect Attributes")]
    public STATUS_EFFECT statusEffect;

    [Space(10)]
    public float effectLength;

    [Space(10)]
    public AnimationCurve effectCurve;

    [Space(10)]
    public float effectStrength;

    public EntityStatusEffect(STATUS_EFFECT newStatusEffect, float newEffectLength, AnimationCurve newEffectCurve, float newEffectStrength)
    {
        this.statusEffect = newStatusEffect;
        this.effectLength = newEffectLength;
        this.effectCurve = newEffectCurve;
        this.effectStrength = newEffectStrength;
    }
}

[System.Serializable]
public struct ActiveStatusEffect
{
    [Header("Active Status Effect Attributes")]
    public EntityStatusEffect statusEffect;

    [Space(10)]
    public Vector3 statusStartPosition;
    public Vector3 statusCollidePoint;
    public Vector3 statusHitDirection;

    [Space(10)]
    public float statusEffectStartTime;
    public float statusEffectEndTime;

    public ActiveStatusEffect(EntityStatusEffect newStatusEffect, float newStatusEffectStartTime, Vector3 newStatusStartPosition, Vector3 newCollidePoint)
    {
        this.statusEffect = newStatusEffect;
        this.statusStartPosition = newStatusStartPosition;
        this.statusCollidePoint = newCollidePoint;

        this.statusHitDirection = (newStatusStartPosition - newCollidePoint).normalized;

        this.statusEffectStartTime = newStatusEffectStartTime;
        this.statusEffectEndTime = Time.time + newStatusEffect.effectLength;
    }
}

public enum STATUS_EFFECT
{
    NONE,
    STUNNED,
    SLOWED,
    KNOCKBACK
}

public enum DAMAGE_TYPE
{
    NONE,
    PHYSICAL,
    MAGICAL,
    TRUE
}
