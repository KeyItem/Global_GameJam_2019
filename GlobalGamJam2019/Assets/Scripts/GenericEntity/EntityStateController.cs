using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateController : MonoBehaviour
{
    [Header("Entity States")]
    public EntityState state;

    [Space(10)] 
    public EntityState[] anyStates;
    
    [Space(10)]
    public bool isAIActive = true;

    public virtual void ManageStates(EntityController controller, EntityStateInfo entityStateInfo)
    {
        if (!isAIActive)
        {
            return;
        }

        for (int i = 0; i < anyStates.Length; i++)
        {
            if (!state == anyStates[i])
            {
                for (int o = 0; o < anyStates[i].transitions.Length; o++)
                {
                    if (anyStates[i].transitions[o].decision.Decide(controller, this, entityStateInfo))
                    {
                        TransitionToState(anyStates[i].transitions[o].trueState, controller, this, entityStateInfo);

                        Debug.Log("Death State");
                    
                        return;
                    }
                }
            }          
        }

        if (state != null)
        {
            state.UpdateState(controller, this, entityStateInfo);
        }
    }

    public virtual void TransitionToState(EntityState nextState, EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        if (nextState != null)
        {
            state = nextState;

            state.InitializeActions(controller, stateController, stateInfo);
        }
        else
        {
            Debug.LogError("Next State is == Null");
        }
    }
}

[System.Serializable]
public struct EntityStateInfo
{
    [Header("Entity Component State Info")]
    public EntityAbilityStateInfo abilityInfo;
    public EntityCollisionInfo collisionInfo;
    public EntityDetectionInfo detectionInfo;
    public EntityStatusInfo statusInfo;

    public EntityStateInfo(EntityAbilityStateInfo newAbilityInfo, EntityCollisionInfo newCollisionInfo, EntityDetectionInfo newDetectionInfo, EntityStatusInfo newEntityStatusInfo)
    {
        this.abilityInfo = newAbilityInfo;
        this.collisionInfo = newCollisionInfo;
        this.detectionInfo = newDetectionInfo;
        this.statusInfo = newEntityStatusInfo;
    }
}

[System.Serializable]
public struct EntityResolvedStateInfo
{
    [Header("Entity Ability Return State")]
    public ENTITY_ABILITY_ACTIONS entityAbilityAction;

    [Header("Entity Navigation Return States")]
    public ENTITY_NAVIGATION_ACTIONS entityNavigationAction;

    public EntityResolvedStateInfo (ENTITY_ABILITY_ACTIONS newAbilityAction, ENTITY_NAVIGATION_ACTIONS newNavigationAction)
    {
        this.entityAbilityAction = newAbilityAction;
        this.entityNavigationAction = newNavigationAction;
    }
}

public enum ENTITY_NAVIGATION_ACTIONS
{
    NONE,
    CHASE_TARGET,
    RETURN_HOME
}

public enum ENTITY_ABILITY_ACTIONS
{
    NONE,
    ATTACK_TARGET
}
