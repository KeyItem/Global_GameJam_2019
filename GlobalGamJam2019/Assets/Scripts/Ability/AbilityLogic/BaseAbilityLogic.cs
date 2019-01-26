using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbilityLogic : MonoBehaviour
{
    [Header("Base Ability Attributes")]
    public AbilityEvent[] abilityEvents;

    private AbilityEvent currentAbilityEvent;

    private BaseAbility baseAbility;

    private Vector3 baseAbilityPosition;

    [Space(10)]
    public bool isAbilityActive = false;
    public bool isAbilityComplete = false;

    [Header("Ability Event Attributes")]
    private int currentAbilityEventIndex;
    private int maxAbilityEventIndex;

    private float startAbilityEventTime = 0;

    private float maxAbilityMovementTime = 0f;
    private float maxAbilityInteractionTime = 0f;
    private float maxAbilityEffectTime = 0f;

    private bool isAbilityMovementCompleted = false;
    private bool isAbilityInteractionCompleted = false;
    private bool isAbilityEffectCompleted = false;

    [Header("Movement Ability Attributes")]
    private MovementInfo abilityMovement;

    [Header("Collider Ability Attributes")]
    private List<HitEntityInfo> hitColliders = new List<HitEntityInfo>();
    private List<HitEntityInfo> completedHitColliders = new List<HitEntityInfo>();

    private Vector3 abilityColliderPoint = Vector3.zero;

    public virtual void Initialize(AbilityEvent[] newAbilityActions, BaseAbility newBaseAbility)
    {
        baseAbility = newBaseAbility;
        abilityEvents = newAbilityActions;

        currentAbilityEventIndex = 0;
        maxAbilityEventIndex = abilityEvents.Length;

        baseAbilityPosition = transform.localPosition;
        abilityColliderPoint = baseAbilityPosition;
    }

    public virtual void ActivateAbility()
    {
        if (!isAbilityActive)
        {
            ImportEvent(0);

            abilityColliderPoint = transform.position;

            isAbilityActive = true;
            isAbilityComplete = false;
        }     
    }

    public virtual void ManageAbility(InputInfo inputInfo, EntityCollisionInfo collisionInfo, EntityDetectionInfo detectionInfo, bool isAbilityBeingHeld)
    {
        if (isAbilityActive)
        {
            if (!ReturnActionsCompleted())
            {
                if (baseAbility.abilityActivation.activationType == ABILITY_ACTIVATION_TYPE.HELD)
                {
                    if (!isAbilityBeingHeld)
                    {
                        CompleteAbility();
                    }          
                }
                else if (baseAbility.abilityActivation.activationType == ABILITY_ACTIVATION_TYPE.TOGGLE)
                {
                    if (isAbilityBeingHeld)
                    {
                        CompleteAbility();
                    }
                }

                ParseAbility(currentAbilityEvent, collisionInfo, detectionInfo);
                ManageHitColliders();
            }
            else
            {
                CycleToNextAction();
            }
        }  
    }

    public virtual void ParseAbility(AbilityEvent newAbilityEvent, EntityCollisionInfo collisionInfo, EntityDetectionInfo detectionInfo)
    {
        if (!isAbilityMovementCompleted)
        {
            ManageMovement(newAbilityEvent.abilityMovement);
        }

        if (!isAbilityInteractionCompleted)
        {
            ManageInteraction(newAbilityEvent.abilityInteraction, newAbilityEvent.abilityInteraction.abilityInteractionMovementInfo, newAbilityEvent.abilityInteraction.abilityColliderInfo);
        }

        if (!isAbilityEffectCompleted)
        {
            ManageEffect(newAbilityEvent.abilityEffect);
        }
    }

    public virtual void ManageMovement(AbilityMovementInfo abilityMovementInfo)
    {
        if (Time.time < maxAbilityMovementTime || currentAbilityEvent.abilityMovement.isInfinite)
        {
            if (abilityMovementInfo.movementDirection != Vector3.zero || abilityMovementInfo.useForward)
            {
                abilityMovement = ReturnMovementInteractionInfo(abilityMovementInfo.useForward ? transform.forward : abilityMovementInfo.movementDirection, abilityMovementInfo);
            }
        }
        else
        {
            abilityMovement = new MovementInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f, Space.World, false, false);

            isAbilityMovementCompleted = true;
        }
    }

    public virtual void ManageInteraction(AbilityInteractionInfo abilityInteractionInfo, AbilityInteractionMovementInfo abilityInteractionMovementInfo, AbilityColliderInfo abilityColliderInfo)
    {
        if (Time.time < maxAbilityInteractionTime || currentAbilityEvent.abilityInteraction.isInfinite)
        {
            if (abilityInteractionMovementInfo.movementDirection != Vector3.zero)
            {
                ManageAbilityInteractionMovement(abilityInteractionInfo, abilityInteractionMovementInfo);
            }

            if (abilityColliderInfo.abilityColliderType != ABILITY_COLLIDER_TYPE.NONE)
            {
                ManageAbilityCollider(abilityInteractionInfo, abilityColliderInfo);
            }
        }
        else
        {
            isAbilityInteractionCompleted = true;
        }
    }

    public virtual void ManageEffect(AbilityEffectInfo abilityEffectInfo)
    {
        if (Time.time < maxAbilityEffectTime || currentAbilityEvent.abilityEffect.isInfinite)
        {

        }
        else
        {
            isAbilityEffectCompleted = true;
        }
    }

    public virtual void ManageAbilityInteractionMovement(AbilityInteractionInfo abilityInteractionInfo, AbilityInteractionMovementInfo abilityInteractionMovementInfo)
    {
        if (abilityInteractionMovementInfo.movementDirection != Vector3.zero)
        {
            abilityColliderPoint = ReturnInteractionMovementVector(abilityInteractionInfo, abilityInteractionMovementInfo);
        }
    }

    public virtual void ManageAbilityCollider(AbilityInteractionInfo abilityInteractionInfo, AbilityColliderInfo abilityColliderInfo)
    {
        Collider[] sweepedColliders = new Collider[0];

        switch (abilityColliderInfo.abilityColliderType)
        {
            case ABILITY_COLLIDER_TYPE.BOX:
                sweepedColliders = Physics.OverlapBox(abilityColliderPoint, abilityColliderInfo.colliderSize / 2, transform.rotation, abilityColliderInfo.colliderHitMask);
                break;

            case ABILITY_COLLIDER_TYPE.SPHERE:
                sweepedColliders = Physics.OverlapSphere(abilityColliderPoint, abilityColliderInfo.colliderRadius, abilityColliderInfo.colliderHitMask);
                break;
        }

        for (int i = 0; i < sweepedColliders.Length; i++)
        {
            HitEntityInfo hitInfo = new HitEntityInfo(sweepedColliders[i], abilityColliderPoint, abilityInteractionInfo.abilityDamageInfo.damageInfo);

            if (!CheckIfAlreadyHitEntity(hitInfo))
            {
                hitColliders.Add(hitInfo);
            }
        }
    }

    public virtual void ManageHitColliders()
    {
        for (int i = 0; i < hitColliders.Count; i++)
        {
            EntityStatus hitEntityStatus = hitColliders[i].hitCollider.GetComponent<EntityStatus>();

            if (hitEntityStatus != null)
            {
                hitEntityStatus.TakeDamage(hitColliders[i].hitAbilityDamageInfo, hitColliders[i].hitPosition);
            }

            completedHitColliders.Add(hitColliders[i]);
        }

        hitColliders.Clear();
    }

    public virtual void ImportEvent(int actionIndex)
    {       
        currentAbilityEvent = abilityEvents[actionIndex];

        startAbilityEventTime = Time.time;

        isAbilityMovementCompleted = currentAbilityEvent.abilityMovement.abilityTime > 0 ? false : true;
        isAbilityInteractionCompleted = currentAbilityEvent.abilityInteraction.abilityTime > 0 ? false : true;
        isAbilityEffectCompleted = currentAbilityEvent.abilityEffect.abilityTime > 0 ? false : true;

        maxAbilityMovementTime = Time.time + currentAbilityEvent.abilityMovement.abilityTime;
        maxAbilityInteractionTime = Time.time + currentAbilityEvent.abilityInteraction.abilityTime;
        maxAbilityEffectTime = Time.time + currentAbilityEvent.abilityEffect.abilityTime;

        hitColliders.Clear();
        completedHitColliders.Clear();

        if (currentAbilityEvent.abilityInteraction.abilityInteractionMovementInfo.resetColliderToBasePosition)
        {
            abilityColliderPoint = transform.position;
        }

        if (currentAbilityEvent.abilityInteraction.abilityInteractionMovementInfo.movementStartPosition != Vector3.zero)
        {
            abilityColliderPoint = transform.position;
            abilityColliderPoint = transform.TransformPoint(currentAbilityEvent.abilityInteraction.abilityInteractionMovementInfo.movementStartPosition);
        }

        abilityMovement = new MovementInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f, Space.World, false, false);
    }

    public virtual void CompleteAbility()
    {
        abilityMovement = new MovementInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f, Space.World, false, false);

        currentAbilityEventIndex = 0;

        transform.localPosition = baseAbilityPosition;
        abilityColliderPoint = transform.position;

        hitColliders.Clear();
        completedHitColliders.Clear();

        isAbilityActive = false;
        isAbilityComplete = true;
    }

    public virtual MovementInfo ReturnAbilityEntityMovementInfo()
    {
        return abilityMovement;
    }

    public virtual void CycleToNextAction()
    {
        if (CanCycleToNextAction(currentAbilityEventIndex))
        {
            currentAbilityEventIndex++;

            ImportEvent(currentAbilityEventIndex);
        }
        else
        {
            CompleteAbility();
        }
    }

    public virtual bool CanCycleToNextAction(int currentActionIndex)
    {
        if (maxAbilityEventIndex > 1)
        {
            if (++currentActionIndex > maxAbilityEventIndex - 1)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public virtual MovementInfo ReturnMovementInteractionInfo(Vector3 directionVector, AbilityMovementInfo abilityMovementInfo)
    {
        Vector3 newMovementVector = directionVector.normalized;

        float movementVelocity = abilityMovementInfo.actionMoveDistance / abilityMovementInfo.abilityTime;

        float speedMultiplier = Helper.ReturnEvalutationOfAnimationCurve(Helper.ReturnTimeRatio(Time.time, startAbilityEventTime, maxAbilityMovementTime), abilityMovementInfo.abilityTime, abilityMovementInfo.movementCurve);

        newMovementVector = (newMovementVector * movementVelocity) * speedMultiplier;

        return new MovementInfo(MOVEMENT_SOURCE.ACTION, newMovementVector, abilityMovementInfo.actionInputModifier, abilityMovementInfo.movementSpace, abilityMovementInfo.useGravity, abilityMovementInfo.useForward);
    }

    public virtual Vector3 ReturnInteractionMovementVector(AbilityInteractionInfo abilityInteractionInfo, AbilityInteractionMovementInfo abilityInteractionMovementInfo)
    {
        Vector3 newMovementPosition = Vector3.zero;

        if (abilityInteractionMovementInfo.movementType == ABILITY_MOVEMENT_TYPE.VELOCITY)
        {
            if (abilityInteractionMovementInfo.movementSpace == Space.Self)
            {
                newMovementPosition = transform.TransformDirection(abilityInteractionMovementInfo.movementDirection.normalized);
            }
            else
            {
                newMovementPosition = abilityInteractionMovementInfo.movementDirection.normalized;
            }

            float movementVelocity = abilityInteractionMovementInfo.actionMoveDistance / abilityInteractionInfo.abilityTime;

            float speedMultiplier = Helper.ReturnEvalutationOfAnimationCurve(Helper.ReturnTimeRatio(Time.time, startAbilityEventTime, maxAbilityInteractionTime), abilityInteractionInfo.abilityTime, abilityInteractionMovementInfo.movementCurve);

            newMovementPosition = (newMovementPosition * movementVelocity) * speedMultiplier;

            newMovementPosition = newMovementPosition * Time.deltaTime;
        }
        else if (abilityInteractionMovementInfo.movementType == ABILITY_MOVEMENT_TYPE.POSITION)
        {
            if (abilityInteractionMovementInfo.movementSpace == Space.Self)
            {
                newMovementPosition = transform.TransformPoint(abilityInteractionMovementInfo.movementDirection);
            }
            else
            {
                newMovementPosition = abilityInteractionMovementInfo.movementDirection;
            }
        }

        return newMovementPosition;
    }

    public virtual float ReturnTotalActionTime()
    {
        float newActionTotalTime = 0f;

        for (int i = 0; i < abilityEvents.Length; i++)
        {
            float actionTime = abilityEvents[i].abilityMovement.abilityTime;

            if (abilityEvents[i].abilityInteraction.abilityTime > actionTime)
            {
                actionTime = abilityEvents[i].abilityInteraction.abilityTime;
            }

            if (abilityEvents[i].abilityEffect.abilityTime > actionTime)
            {
                actionTime = abilityEvents[i].abilityEffect.abilityTime;
            }

            newActionTotalTime += actionTime;
        }

        return newActionTotalTime;
    }

    public virtual bool ReturnActionsCompleted()
    {
        if (isAbilityMovementCompleted && isAbilityInteractionCompleted && isAbilityEffectCompleted)
        {
            return true;
        }

        return false;
    }

    public virtual bool CheckIfAlreadyHitEntity(HitEntityInfo hitEntity)
    {
        for (int i = 0; i < completedHitColliders.Count; i++)
        {
            if (completedHitColliders[i].hitCollider == hitEntity.hitCollider)
            {
                return true;
            }
        }

        return false;
    }

    public void OnDrawGizmos()
    {
        if (isAbilityActive)
        {
            Gizmos.color = Color.red;

            if (currentAbilityEvent.abilityInteraction.abilityColliderInfo.abilityColliderType == ABILITY_COLLIDER_TYPE.BOX)
            {
                Matrix4x4 cubeMatrix = Matrix4x4.TRS(abilityColliderPoint, transform.rotation, currentAbilityEvent.abilityInteraction.abilityColliderInfo.colliderSize);
                Matrix4x4 oldMatrix = Gizmos.matrix;

                Gizmos.matrix *= cubeMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);

                Gizmos.matrix = oldMatrix;
            }
            else if (currentAbilityEvent.abilityInteraction.abilityColliderInfo.abilityColliderType == ABILITY_COLLIDER_TYPE.SPHERE)
            {
                Gizmos.DrawSphere(abilityColliderPoint, 1f);
            }  
        }
    }
}

[System.Serializable]
public struct HitEntityInfo
{
    [Header("Hit Entity Info")]
    public Collider hitCollider;

    [Space(10)]
    public Vector3 hitPosition;

    [Space(10)]
    public EntityDamageInfo hitAbilityDamageInfo;

    public HitEntityInfo (Collider newHitCollider, Vector3 newHitPosition, EntityDamageInfo newDamageInfo)
    {
        this.hitCollider = newHitCollider;
        this.hitPosition = newHitPosition;
        this.hitAbilityDamageInfo = newDamageInfo;
    }
}
