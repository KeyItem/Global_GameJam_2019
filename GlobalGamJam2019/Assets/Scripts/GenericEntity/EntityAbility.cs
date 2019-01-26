using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAbility : MonoBehaviour
{
    [Header("Entity Actions")]
    public List<BaseAbilityCard> entityAbilities;

    private List<BaseAbility> entityUsedAbilities = new List<BaseAbility>();

    [Space(10)]
    public GameObject[] entityAbilityObjects;

    [Space(10)]
    public List<ActiveAbility> entityActiveAbilities = new List<ActiveAbility>();
    private List<AbilityCooldown> entityActiveCooldowns = new List<AbilityCooldown>();

    [Space(10)]
    private EntityAbilityStateInfo abilityStateInfo;

    public void Start()
    {
        InitializeAbilitySystem();
    }

    public virtual void InitializeAbilitySystem()
    {
        
    }

    public virtual void PerformAbilities(InputInfo inputInfo, EntityCollisionInfo collisionInfo, EntityDetectionInfo detectionInfo)
    {
        if (CheckInput(inputInfo))
        {
            int requestActionID = ReturnRequestAbilityIndex(inputInfo);

            BaseAbility requestedAbility = entityAbilities[requestActionID].cardAbility; //TODO: Add dynamic ability selection for NPC
            ActiveAbility requestActiveAbility = new ActiveAbility(requestedAbility, requestActionID);

            if (CheckIfAbilityIfOnCooldown(requestedAbility))
            {
                PrepareAbility(requestedAbility);
                UseAbility(requestedAbility);
                AddAbilityToActive(requestActiveAbility);
            }
        }
    }

    public virtual void ManageAbilities(InputInfo inputInfo, EntityCollisionInfo collisionInfo, EntityDetectionInfo detectionInfo)
    {
        ManageActiveAbilities(inputInfo, collisionInfo, detectionInfo);
        ManageCompletedAbilities();
        ManageActiveAbilityCooldowns();
        ManageAbilityStateInfo();
    }

    public virtual void ManageActiveAbilities(InputInfo inputInfo, EntityCollisionInfo collisionInfo, EntityDetectionInfo detectionInfo)
    {
        for (int i = 0; i < entityActiveAbilities.Count; i++)
        {
            if (entityActiveAbilities[i].activeAction.abilityLogic.isAbilityActive)
            {
                int abilityIndex = entityActiveAbilities[i].abilityInputIndex;

                entityActiveAbilities[i].activeAction.abilityLogic.ManageAbility(inputInfo, collisionInfo, detectionInfo, CheckForHeldInput(inputInfo, abilityIndex));
            }       
        }
    }

    public virtual void ManageActiveAbilityCooldowns()
    {
        List<AbilityCooldown> completedCooldowns = new List<AbilityCooldown>();

        for (int i = 0; i < entityActiveCooldowns.Count; i++)
        {
            if (entityActiveCooldowns[i].ability.abilityCooldown.cooldownType == ABILITY_COOLDOWN_TYPE.TIMED)
            {
                if (Time.time > entityActiveCooldowns[i].targetCooldownTime) 
                {
                    completedCooldowns.Add(entityActiveCooldowns[i]); 
                }
            }
        }

        for (int o = 0; o < completedCooldowns.Count; o++)
        {
            entityActiveCooldowns.Remove(completedCooldowns[o]);
        }
    }

    public virtual void ManageAbilityStateInfo()
    {
        abilityStateInfo = new EntityAbilityStateInfo(entityActiveAbilities);
    }

    public virtual void PrepareAbility(BaseAbility abilityToPrepare)
    {
        GameObject newAbilityObject = ReturnFreeAbilityObject();

        abilityToPrepare.InitializeAbility(newAbilityObject);
    }

    public virtual void UseAbility(BaseAbility abilityToUse)
    {
        abilityToUse.ActivateAbility();

        entityUsedAbilities.Add(abilityToUse);
    }

    public virtual void ManageCompletedAbilities()
    {
        List<ActiveAbility> abilitiesToComplete = new List<ActiveAbility>();

        for (int i = 0; i < entityActiveAbilities.Count; i++)
        {
            if (entityActiveAbilities[i].activeAction.abilityLogic.isAbilityComplete)
            {
                abilitiesToComplete.Add(entityActiveAbilities[i]);
            }
        }

        for (int o = 0; o < abilitiesToComplete.Count; o++)
        {
            entityActiveAbilities.Remove(abilitiesToComplete[o]);

            entityActiveCooldowns.Add(new AbilityCooldown(abilitiesToComplete[o].activeAction));
        }
    }

    public virtual void AddAbilityToActive(ActiveAbility activeAbility)
    {
        if (entityActiveAbilities.Contains(activeAbility))
        {
            return;
        }

        entityActiveAbilities.Add(activeAbility);
    }

    public virtual MovementInfo ReturnAbilityMovement()
    {
        MovementInfo newAbilityMovement = new MovementInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f, Space.World, false, false);

        for (int i = 0; i < entityActiveAbilities.Count; i++)
        {
            MovementInfo abilityMovement = entityActiveAbilities[i].activeAction.ReturnActionMovement();

            if (abilityMovement.movement != Vector3.zero)
            {
                newAbilityMovement = abilityMovement;
            }
        }

        return newAbilityMovement;
    }

    public virtual bool CheckIfAbilityIfOnCooldown(BaseAbility activeAbility)
    {
        for (int i = 0; i < entityActiveCooldowns.Count; i++)
        {
            if (entityActiveCooldowns[i].ability == activeAbility)
            {
                return false;
            }
        }

        for (int o = 0; o < entityActiveAbilities.Count; o++)
        {
            if (entityActiveAbilities[o].activeAction == activeAbility)
            {
                return false;
            }
        }

        return true;
    }

    public virtual int ReturnRequestAbilityIndex(InputInfo input)
    {
        if (input.ReturnCurrentButtonState("Action0"))
        {
            return 0;
        }
        else if (input.ReturnCurrentButtonState("Action1"))
        {
            return 1;
        }
        else if (input.ReturnCurrentButtonState("Action2"))
        {
            return 2;
        }
        else if (input.ReturnCurrentButtonState("Action3"))
        {
            return 3;
        }

        return 0;
    }

    public virtual bool CheckInput(InputInfo input)
    {
        if (input.ReturnCurrentButtonState("Action0") || input.ReturnCurrentButtonState("Action1") || input.ReturnCurrentButtonState("Action2"))
        {
            return true;
        }

        return false;
    }
    
    public virtual bool CheckForHeldInput(InputInfo input, int abilityIndex)
    {
        string inputButtonName = "Action" + abilityIndex;

        return input.ReturnCurrentButtonState(inputButtonName);
    }

    public virtual GameObject ReturnFreeAbilityObject()
    {
        GameObject newAbilityObject = null;

        for (int i = 0; i < entityAbilityObjects.Length; i++)
        {
            if (entityAbilityObjects[i].GetComponent<BaseAbilityLogic>().isAbilityActive)
            {
                continue;
            }
            else
            {
                newAbilityObject = entityAbilityObjects[i];

                break;
            }
        }

        return newAbilityObject;
    }

    public virtual EntityAbilityStateInfo ReturnAbilityStateInfo()
    {
        return abilityStateInfo;
    }
}

[System.Serializable]
public struct EntityAbilityStateInfo
{
    [Header("Ability State Info")]
    public List<ActiveAbility> activeAbilities;

    [Space(10)]
    public bool isUsingAbility;

    public EntityAbilityStateInfo (List<ActiveAbility> newListOfActiveAbilities)
    {
        this.activeAbilities = newListOfActiveAbilities;

        bool areAbilitiesActive = newListOfActiveAbilities.Count > 0 ? true : false;

        this.isUsingAbility = areAbilitiesActive;
    }
}


