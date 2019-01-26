using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [Header("Entity Controller Base Attributes")]
    public bool isEntityAwake = true;

    [Header("Entity Info")]
    public EntityInfo info;

    [Header("Entity Input")]
    public EntityInput input;

    [Space(10)]
    [HideInInspector]
    public InputInfo inputValues;

    [Space(10)]
    public bool isEntityCapturingInput = true;

    [Header("Entity State Controller")]
    public EntityStateController stateController;

    [Space(10)]
    public bool isEntityCapturingStates = true;

    [Header("Entity Movement")]
    public EntityMovement movement;

    [Space(10)]
    public bool isEntityCapturingMovement = true;

    [Header("Entity Action")]
    public EntityAbility ability;

    [HideInInspector]
    public EntityAbilityStateInfo abilityInfo;

    [Space(10)]
    public bool isEntityCapturingActions = true;

    [Header("Entity Collision")]
    public EntityCollision collision;

    [HideInInspector]
    public EntityCollisionInfo collisionInfo;

    [Space(10)]
    public bool isEntityCapturingCollisions = true;

    [Header("Entity Detection")]
    public EntityDetection detection;

    [HideInInspector]
    public EntityDetectionInfo detectionInfo;

    [Space(10)]
    public bool isEntityCapturingDetection = true;

    [Header("Entity Navigation")]
    public EntityNavigation navigation;

    [Space(10)]
    public bool isEntityCapturingNavigation = true;

    [Header("Entity Status")]
    public EntityStatus status;

    [HideInInspector]
    public EntityStatusInfo statusInfo;

    [Space(10)]
    public bool isEntityCapturingStatus = true;

    public virtual void Start()
    {
        InitializeEntity();
    }

    public virtual void Update()
    {
        if (isEntityAwake)
        {
            ManageInput();
            ManageCollision();
            ManageDetection();
            ManageAction(inputValues, collisionInfo, detectionInfo);
            ManageMovement(inputValues, collisionInfo);
        } 
    }

    public virtual void InitializeEntity()
    {
        input = GetComponent<EntityInput>();
        movement = GetComponent<EntityMovement>();
        collision = GetComponent<EntityCollision>();
        ability = GetComponent<EntityAbility>();
        detection = GetComponent<EntityDetection>();
        navigation = GetComponent<EntityNavigation>();
        status = GetComponent<EntityStatus>();
        stateController = GetComponent<EntityStateController>();
    }

    public virtual void ManageStates()
    {
        if (isEntityCapturingStates)
        {
            EntityStateInfo currentStateInfo = new EntityStateInfo(abilityInfo, collisionInfo, detectionInfo, statusInfo);

            stateController.ManageStates(this, currentStateInfo);
        }
    }

    public virtual void ManageInput()
    {
        if (isEntityCapturingInput)
        {
            if (input != null)
            {
                input.GetInput();

                inputValues = input.ReturnInput();
            }
        }      
    }

    public virtual void ManageCollision()
    {
        if (isEntityCapturingCollisions)
        {
            collisionInfo = collision.ReturnCollisionInfo();
        }
    }

    public virtual void ManageAction(InputInfo input, EntityCollisionInfo collisionInfo, EntityDetectionInfo detectionInfo)
    {
        if (isEntityCapturingActions)
        {
            ability.PerformAbilities(input, collisionInfo, detectionInfo);
            ability.ManageAbilities(input, collisionInfo, detectionInfo);

            abilityInfo = ability.ReturnAbilityStateInfo();
        }
    }

    public virtual void ManageMovement(InputInfo input, EntityCollisionInfo collisionInfo)
    {
        if (isEntityCapturingMovement)
        {
            movement.SlopeRotation(collisionInfo);
            movement.Rotate(input, collisionInfo, statusInfo, ReturnMovementOverride(), ReturnMovementModifier());
            movement.Move(input, collisionInfo, statusInfo, ReturnMovementOverride(), ReturnMovementModifier(), true);
        }
    }

    public virtual void ManageDetection()
    {
        if (isEntityCapturingDetection)
        {
            detectionInfo = detection.ReturnDetectionInfo();
        }
    }

    public virtual void ManageNavigation()
    {
        if (isEntityCapturingNavigation)
        {
            navigation.NavigateEntity(detectionInfo, statusInfo);
        }
    }

    public virtual void ManageStatus()
    {
        if (isEntityCapturingStatus)
        {
            status.ManageStatusEffects();

            statusInfo = status.ReturnEntityStatusInfo();
        }
    }

    public virtual MovementInfo ReturnMovementOverride()
    {
        MovementInfo newMovementOverride = status.ReturnEntityStatusMovement();

        if (newMovementOverride.movementSource == MOVEMENT_SOURCE.NONE)
        {
            newMovementOverride = ability.ReturnAbilityMovement();
        }
        
        if (newMovementOverride.movementSource == MOVEMENT_SOURCE.NONE)
        {
            newMovementOverride = navigation.ReturnPathfindVector();
        }

        return newMovementOverride;
    }

    public virtual MovementModifierInfo ReturnMovementModifier()
    {
        MovementModifierInfo newMovementModifierInfo = status.ReturnEntityStatusMovementModifer();

        return newMovementModifierInfo;
    }
}

[System.Serializable]
public struct EntityInfo
{
    [Header("Base Entity Info")]
    public string entityName;

    [Space(10)]
    public ENTITY_TYPE entityType;

    [Space(10)]
    public GameObject entityObject;

    public EntityInfo (string newEntityName, ENTITY_TYPE newEntityType, GameObject newEntityObject)
    {
        this.entityName = newEntityName;
        this.entityType = newEntityType;
        this.entityObject = newEntityObject;
    }
}

public enum ENTITY_TYPE
{
    NONE,
    PLAYER,
    ENEMY
}
