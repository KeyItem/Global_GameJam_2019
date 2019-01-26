using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EntityMovement : MonoBehaviour
{
    [Header("Base Entity Character Controller")]
    [HideInInspector]
    public CharacterController entityCharacterController;

    [Header("Base Entity Movement Attributes")]
    public EntityMovementAttributes entityMovementAttributes;
    public EntityMovementSmoothingAttributes entityMovementSmoothingAttributes;

    [Header("Base Entity Rotational Smoothing Attributes")]
    public EntityRotationalSmoothingAttributes entityRotationalSmoothingAttributes;

    [Header("Base Entity Jumping Attributes")]
    public EntityJumpingAttributes entityJumpingAttributes;

    [Header("Base Entity Slope Attributes")]
    public EntitySlopeAttributes entitySlopeAttributes;

    [Header("Base Entity Slide Smoothing Attributes")]
    public EntitySlidingSmoothingAttributes entitySlidingSmoothingAttributes;

    private float entitySlideGraceTime = 0;

    [Header("Base Entity Direction Attributes")]
    public Vector3 entityMovementVector;
    public Vector3 entityMovementDirection;

    [Space(10)] 
    private Quaternion entityMovementRotationalOffset;

    [Header("Base Entity Movement Values")]
    public float entityCurrentMovementSmoothedVelocity;

    [Space(10)]
    public float entityCurrentSlideSmoothedVelocity;

    [HideInInspector]
    public float entityTargetMovementSpeed;

    [HideInInspector]
    public float entityMoveSmoothVelocity;

    [HideInInspector]
    public float entitySlideSmoothVelocity;

    [Header("Base Entity Rotational Values")]
    public float entityCurrentRotationalSmoothedVelocity;

    [HideInInspector]
    public float entityTargetRotationalSpeed;

    [HideInInspector]
    public float entityRotationalSmoothVelocity;

    [Header("Base Entity Gravity Values")]
    public Vector3 entityGravityVelocity;

    public virtual void Start()
    {
        InitializeEntityMovement();
        SetNewMovementOffset(Camera.main);
    }

    public virtual void InitializeEntityMovement()
    {
        entityCharacterController = GetComponent<CharacterController>();        
    }

    public virtual void Move(InputInfo currentInput, EntityCollisionInfo collisionInfo, EntityStatusInfo statusInfo, MovementInfo movementOverride, MovementModifierInfo movementModifier, bool useGravity)
    {
        MovementInfo newMovementInfo = new MovementInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f, Space.World, false, false);

        if (movementOverride.movementSource != MOVEMENT_SOURCE.NONE)
        {
            newMovementInfo = movementOverride;    
        }
        else
        {
            newMovementInfo = new MovementInfo(MOVEMENT_SOURCE.INPUT, ReturnRawInputMovementVector(currentInput), 0f, Space.World, true, false);
        }

        Vector3 newMovementVector = ReturnMovementVelocity(newMovementInfo, movementModifier, collisionInfo, currentInput, statusInfo);
        
        MoveController(newMovementVector);
    }

    public virtual void Rotate(InputInfo inputValues, EntityCollisionInfo collisionInfo, EntityStatusInfo statusInfo, MovementInfo movementOverride, MovementModifierInfo movementModifierInfo)
    {
        Vector3 newRotationVector = Vector3.zero;

        if (movementOverride.movementSource != MOVEMENT_SOURCE.NONE)
        {
            newRotationVector = ReturnRotationVelocity(movementOverride, movementModifierInfo, collisionInfo, inputValues, statusInfo);
        }
        else
        {
            movementOverride = new MovementInfo(MOVEMENT_SOURCE.INPUT, ReturnRawInputRotationalVector(inputValues), 0f, Space.World, true, false);

            newRotationVector = ReturnRotationVelocity(movementOverride, movementModifierInfo, collisionInfo, inputValues, statusInfo);
        }

        if (newRotationVector != Vector3.zero)
        {
            float newTargetRotation = Mathf.Atan2(newRotationVector.x, newRotationVector.y) * Mathf.Rad2Deg;
            float targetRotationSmoothing = 1 * Mathf.SmoothDampAngle(transform.eulerAngles.y, newTargetRotation, ref entityRotationalSmoothVelocity, ReturnRotationalSmoothTime(collisionInfo));

            Vector3 newEulerAngle = new Vector3(transform.rotation.eulerAngles.x, targetRotationSmoothing, transform.rotation.eulerAngles.z);

            transform.rotation = Quaternion.Euler(newEulerAngle);      
        }
    }

    public virtual void MoveController(Vector3 moveDirection)
    {
        entityCharacterController.Move(moveDirection * Time.deltaTime);
    }

    public virtual void RotateController(Vector3 moveDirection)
    {
        transform.Rotate(moveDirection * Time.deltaTime);
    }

    public virtual void SlopeRotation(EntityCollisionInfo collisionInfo)
    {
        Quaternion newSlopeRotation = Quaternion.LookRotation(collisionInfo.entityGroundForward);

        transform.rotation = Quaternion.Slerp(transform.rotation, newSlopeRotation, entityRotationalSmoothingAttributes.slopeRotationalSmoothing * Time.deltaTime);
    }

    public virtual void SetNewMovementOffset(Camera targetCamera)
    {
        entityMovementRotationalOffset = Quaternion.Euler(0, targetCamera.transform.rotation.eulerAngles.y, 0);
    }

    public virtual void StickEntityToGround(EntityCollisionInfo collisionInfo)
    {
        if (collisionInfo.isGrounded)
        {
            transform.position -= transform.up * collisionInfo.baseGroundData.groundHitDistance;
        }
    }

    public virtual Vector3 ReturnRawInputMovementVector(InputInfo inputValues)
    {
        return Vector3.zero;
    }

    public virtual Vector3 ReturnRawInputRotationalVector(InputInfo inputValues)
    {
        return Vector3.zero;
    }

    public virtual Vector3 ReturnMovementVelocity(MovementInfo movementInfo, MovementModifierInfo movementModifierInfo, EntityCollisionInfo collisionInfo, InputInfo inputValues, EntityStatusInfo statusInfo)
    {
        Vector3 newMovementVelocity = Vector3.zero;

        switch(movementInfo.movementSource)
        {
            case MOVEMENT_SOURCE.INPUT:
                newMovementVelocity = ReturnInputMovementVelocity(movementInfo, movementModifierInfo, collisionInfo, inputValues, statusInfo);
                break;

            case MOVEMENT_SOURCE.NAVIGATION:
                newMovementVelocity = ReturnNavigationMovementVelocity(movementInfo, movementModifierInfo, collisionInfo, inputValues, statusInfo);
                break;

            case MOVEMENT_SOURCE.ACTION:
                newMovementVelocity = ReturnActionMovementVelocity(movementInfo, movementModifierInfo, collisionInfo, inputValues, statusInfo);
                break;

            case MOVEMENT_SOURCE.STATUS:
                newMovementVelocity = ReturnStatusMovementVelocity(movementInfo, movementModifierInfo, collisionInfo, inputValues, statusInfo);
                break;
        }

        return newMovementVelocity;
    }

    public virtual Vector3 ReturnInputMovementVelocity(MovementInfo movementInfo, MovementModifierInfo movementModifierInfo, EntityCollisionInfo collisionInfo, InputInfo input, EntityStatusInfo statusInfo)
    {
        Vector3 newInputMovementVector = movementInfo.movement;
        
        if (movementInfo.useForward)
        {
            newInputMovementVector = collisionInfo.entityGroundForward;
        }
        else
        {            
            newInputMovementVector = entityMovementRotationalOffset * newInputMovementVector;     
            
            newInputMovementVector = Vector3.ProjectOnPlane(newInputMovementVector, collisionInfo.groundInfo.averageGroundNormal);
        }

        if (newInputMovementVector.magnitude > 1)
        {
            newInputMovementVector.Normalize();
        }

        entityCurrentMovementSmoothedVelocity = Mathf.SmoothDamp(entityCurrentMovementSmoothedVelocity, ReturnTargetMoveSpeed(newInputMovementVector, movementInfo), ref entityMoveSmoothVelocity, ReturnMovementSmoothTime(collisionInfo));

        newInputMovementVector *= entityCurrentMovementSmoothedVelocity;

        if (movementModifierInfo.movementSource != MOVEMENT_SOURCE.NONE)
        {
            newInputMovementVector *= movementModifierInfo.movementEffectiveness;
        }

        if (movementInfo.useGravity)
        {
            newInputMovementVector += ReturnGravityVelocity(collisionInfo);
        }

        if (IsEntityOnMaxSlope(collisionInfo))
        {
            if (IsEntitySlipping())
            {
                newInputMovementVector = ReturnSlopeVelocity(collisionInfo);
            }
        }

        return newInputMovementVector;
    }

    public virtual Vector3 ReturnNavigationMovementVelocity(MovementInfo movementInfo, MovementModifierInfo movementModifierInfo, EntityCollisionInfo collisionInfo, InputInfo input, EntityStatusInfo statusInfo)
    {
        Vector3 newNavigationMovementVector = movementInfo.movement;

        if (movementInfo.useForward)
        {
            newNavigationMovementVector = collisionInfo.entityGroundForward;
        }
        else
        {
            newNavigationMovementVector = Vector3.ProjectOnPlane(newNavigationMovementVector, collisionInfo.groundInfo.averageGroundNormal);
        }

        entityCurrentMovementSmoothedVelocity = Mathf.SmoothDamp(entityCurrentMovementSmoothedVelocity, ReturnTargetMoveSpeed(newNavigationMovementVector, movementInfo), ref entityMoveSmoothVelocity, ReturnMovementSmoothTime(collisionInfo));

        newNavigationMovementVector = newNavigationMovementVector.normalized * entityCurrentMovementSmoothedVelocity;

        if (movementModifierInfo.movementSource != MOVEMENT_SOURCE.NONE)
        {
            newNavigationMovementVector *= movementModifierInfo.movementEffectiveness;
        }

        if (movementInfo.useGravity)
        {
            newNavigationMovementVector += ReturnGravityVelocity(collisionInfo);
        }

        if (IsEntityOnMaxSlope(collisionInfo))
        {
            if (IsEntitySlipping())
            {
                newNavigationMovementVector = ReturnSlopeVelocity(collisionInfo);
            }
        }

        return newNavigationMovementVector;
    }

    public virtual Vector3 ReturnActionMovementVelocity(MovementInfo movementInfo, MovementModifierInfo movementModifierInfo, EntityCollisionInfo collisionInfo, InputInfo input, EntityStatusInfo statusInfo)
    {
        Vector3 newActionMovementVector = movementInfo.movement;

        if (movementInfo.useForward)
        {
            newActionMovementVector = collisionInfo.entityGroundForward * movementInfo.movement.magnitude;
        }
        else
        {
            newActionMovementVector = Vector3.ProjectOnPlane(newActionMovementVector, collisionInfo.groundInfo.averageGroundNormal);
        }

        if (movementInfo.inputModifier > 0)
        {
            Vector3 newInputModifier = ReturnRawInputMovementVector(input) * movementInfo.inputModifier;

            float movementLength = newActionMovementVector.magnitude;

            Vector3 actionNormalized = newActionMovementVector.normalized;

            newActionMovementVector = (actionNormalized + newInputModifier).normalized * movementLength;
        }

        if (movementInfo.useGravity)
        {
            movementInfo.movement += ReturnGravityVelocity(collisionInfo);
        }

        return newActionMovementVector;
    }

    public virtual Vector3 ReturnStatusMovementVelocity(MovementInfo movementInfo, MovementModifierInfo movementModifierInfo, EntityCollisionInfo collisionInfo, InputInfo input, EntityStatusInfo statusInfo)
    {
        Vector3 newStatusMovementVelocity = movementInfo.movement;

        if (movementInfo.useForward)
        {
            newStatusMovementVelocity = collisionInfo.entityGroundForward * movementInfo.movement.magnitude;
        }
        else
        {
            newStatusMovementVelocity = Vector3.ProjectOnPlane(newStatusMovementVelocity, collisionInfo.groundInfo.averageGroundNormal);
        }

        if (movementInfo.inputModifier > 0)
        {
            Vector3 newInputModifier = ReturnRawInputMovementVector(input) * movementInfo.inputModifier;

            float movementLength = newStatusMovementVelocity.magnitude;

            Vector3 movementNormalized = newStatusMovementVelocity.normalized;

            newStatusMovementVelocity = (movementNormalized + newInputModifier).normalized * movementLength;
        }

        if (movementModifierInfo.movementSource != MOVEMENT_SOURCE.NONE)
        {
            newStatusMovementVelocity *= movementModifierInfo.movementEffectiveness;
        }

        if (movementInfo.useGravity)
        {
            movementInfo.movement += ReturnGravityVelocity(collisionInfo);
        }

        return newStatusMovementVelocity;
    }

    public virtual Vector3 ReturnRotationVelocity(MovementInfo movementInfo, MovementModifierInfo movementModifierInfo, EntityCollisionInfo collisionInfo, InputInfo input, EntityStatusInfo statusInfo)
    {
        Vector3 newRotationVelocity = Vector3.zero;

        switch (movementInfo.movementSource)
        {
            case MOVEMENT_SOURCE.INPUT:
                newRotationVelocity = ReturnInputRotationVelocity(movementInfo, movementModifierInfo, collisionInfo, input, statusInfo);
                break;

            case MOVEMENT_SOURCE.NAVIGATION:
                newRotationVelocity = ReturnNavigationRotationVelocity(movementInfo, movementModifierInfo, collisionInfo, input, statusInfo);
                break;

            case MOVEMENT_SOURCE.ACTION:
                newRotationVelocity = ReturnActionRotationVelocity(movementInfo, movementModifierInfo, collisionInfo, input, statusInfo);
                break;
        }

        return newRotationVelocity;
    }

    public virtual Vector3 ReturnInputRotationVelocity(MovementInfo movementInfo, MovementModifierInfo movementModifierInfo, EntityCollisionInfo collisionInfo, InputInfo input, EntityStatusInfo statusInfo)
    {
        Vector3 newInputRotationalVector = movementInfo.movement;
        
        newInputRotationalVector = entityMovementRotationalOffset * newInputRotationalVector;     
        
        newInputRotationalVector.Set(newInputRotationalVector.x, newInputRotationalVector.z, newInputRotationalVector.y);
        
        return newInputRotationalVector;
    }

    public virtual Vector3 ReturnNavigationRotationVelocity(MovementInfo movementInfo, MovementModifierInfo movementModifierInfo, EntityCollisionInfo collisionInfo, InputInfo input, EntityStatusInfo statusInfo)
    {
        Vector3 newNavigationRotationVelocity = new Vector3(movementInfo.movement.x, movementInfo.movement.z, movementInfo.movement.y);

        return newNavigationRotationVelocity;
    }

    public virtual Vector3 ReturnActionRotationVelocity(MovementInfo movementInfo, MovementModifierInfo movementModifierInfo, EntityCollisionInfo collisionInfo, InputInfo input, EntityStatusInfo statusInfo)
    {
        return Vector3.zero;
    }

    public virtual Vector3 ReturnGravityVelocity(EntityCollisionInfo collisionInfo)
    {
        if (collisionInfo.isGrounded)
        {
            entityGravityVelocity = Vector3.zero;

            return Vector3.zero;
        }
        else
        {
            entityGravityVelocity.y += (Physics.gravity.y * Time.deltaTime);

            return entityGravityVelocity;
        }
    }

    public virtual Vector3 ReturnSlopeVelocity(EntityCollisionInfo collisionInfo)
    {
        Vector3 slipVector = Vector3.zero;
        Vector3 slipDirection = ReturnSlopeDownward(collisionInfo.groundInfo.ReturnAverageSlopeNormal());

        entityCurrentSlideSmoothedVelocity = Mathf.SmoothDamp(entityCurrentSlideSmoothedVelocity, ReturnSlideSpeed(collisionInfo), ref entitySlideSmoothVelocity, entitySlidingSmoothingAttributes.entitySlideSmoothing);

        slipVector = slipDirection * entityCurrentSlideSmoothedVelocity;

        Debugger.DrawCustomDebugRay(transform.position, slipDirection * entityCurrentSlideSmoothedVelocity, Color.yellow);

        return slipVector;
    }

    public virtual float ReturnTargetMoveSpeed(Vector3 inputMovementVector, MovementInfo movementInfo)
    {     
        if (movementInfo.movementSource == MOVEMENT_SOURCE.NAVIGATION)
        {
            float navigationSpeed = movementInfo.movement.magnitude;

            if (navigationSpeed < 1)
            {
                return 1;
            }

            return movementInfo.movement.magnitude;
        }

        if (inputMovementVector.sqrMagnitude > 1)
        {
            inputMovementVector.Normalize();
        }

        return inputMovementVector.magnitude * entityMovementAttributes.entityBaseMovementSpeed;
    }

    public virtual float ReturnMovementSmoothTime(EntityCollisionInfo collisionInfo)
    {
        if (collisionInfo.isGrounded)
        {
            return entityMovementSmoothingAttributes.groundedMovementSmoothing;
        }
        else
        {
            return entityMovementSmoothingAttributes.airborneMovementSmoothing;
        }
    }

    public virtual float ReturnRotationalSmoothTime(EntityCollisionInfo collisionInfo)
    {
        if (collisionInfo.isGrounded)
        {
            return entityRotationalSmoothingAttributes.groundedRotationalSmoothing;
        }
        else
        {
            return entityRotationalSmoothingAttributes.airborneRotationSmoothing;
        }
    }

    public virtual float ReturnSlideSpeed(EntityCollisionInfo collisionInfo)
    {
        float slopeSpeedRatio = 1 + (collisionInfo.groundInfo.ReturnAverageSlopeAngle() / entitySlopeAttributes.entityMaxSlopeSlideAngle);

        float newSlideSpeed = entitySlopeAttributes.entitySlideSpeed * slopeSpeedRatio;

        return newSlideSpeed;
    }

    public virtual float ReturnDotProductOfDirection(Vector3 moveDirection, Vector3 targetDirection)
    {
        return Vector3.Dot(moveDirection, targetDirection);
    }

    public virtual Vector3 ReturnLocalMoveDirection(Vector3 moveDirection)
    {
        return transform.TransformDirection(moveDirection);
    }

    public virtual Vector3 ReturnSlopeDownward(Vector3 surfaceNormal)
    {
        Vector3 slopeCross = Vector3.Cross(surfaceNormal, Vector3.up);
        Vector3 slideDirection = -Vector3.Cross(slopeCross, surfaceNormal);

        return slideDirection;
    }

    public virtual bool IsEntityOnMaxSlope(EntityCollisionInfo collisionInfo)
    {
        if (collisionInfo.isOnSlope)
        {
            if (collisionInfo.groundInfo.ReturnSmallestSlopeAngle() > entitySlopeAttributes.entityMaxWalkableSlopeAngle)
            {
                return true;
            }
        }

        entityCurrentSlideSmoothedVelocity = 0f;

        entitySlideGraceTime = entitySlopeAttributes.entitySlipTime;

        return false;
    }

    public virtual Vector3 ReturnEntityVelocity()
    {
        return entityCharacterController.velocity;
    }

    public virtual bool IsEntitySlipping()
    {
        if (entitySlideGraceTime <= 0)
        {
            return true;
        }

        entitySlideGraceTime -= Time.deltaTime;

        return false;
    }
}

[System.Serializable]
public struct EntityMovementAttributes
{
    [Header("Entity Movement Speed Attributes")]
    public float entityBaseMovementSpeed;
}

[System.Serializable]
public struct EntityMovementSmoothingAttributes
{
    [Header("Entity Movement Smoothing Attributes")]
    [Range(0, 1)]
    public float groundedMovementSmoothing;
    [Range(0, 1)]
    public float airborneMovementSmoothing;
}

[System.Serializable]
public struct EntitySlidingSmoothingAttributes
{
    [Header("Entity Sliding Smoothing Attributes")]
    [Range(0, 1)]
    public float entitySlideSmoothing;
}

[System.Serializable]
public struct EntityRotationalSmoothingAttributes
{
    [Header("Entity Rotational Smoothing Attributes")]
    [Range(0, 1)]
    public float groundedRotationalSmoothing;
    [Range(0, 1)]
    public float airborneRotationSmoothing;
    [Range(0, 10)]
    public float slopeRotationalSmoothing;
}

[System.Serializable]
public struct EntityJumpingAttributes
{
    [Header("Entity Jumping Attributes")]
    public float entityMinJumpHeight;
    public float entityMaxJumpHeight;

    [Space(10)]
    public float entityJumpTime;
}

[System.Serializable]
public struct EntitySlopeAttributes
{
    [Header("Entity Slope Attributes")]
    [Range(0, 90)]
    public float entityMaxWalkableSlopeAngle;

    [Space(10)]
    [Range(0, 90)]
    public float entityMaxSlopeSlideAngle;

    [Space(10)]
    public float entitySlipTime;

    [Space(10)]
    public float entitySlideSpeed;
}

[System.Serializable]
public struct MovementInfo
{
    [Header("Movement Info")]
    public MOVEMENT_SOURCE movementSource;

    [Space(10)]
    public Vector3 movement;

    [Space(10)]
    public Space movementSpace;

    [Space(10)]
    public float inputModifier;

    [Space(10)]
    public bool useGravity;

    [Space(10)]
    public bool useForward;

    public MovementInfo(MOVEMENT_SOURCE newMovementSource, Vector3 newMovement, float newInputModifier, Space newMovementSpace, bool newUseGravity, bool newUseForward)
    {
        this.movementSource = newMovementSource;
        this.movement = newMovement;
        this.inputModifier = newInputModifier;
        this.movementSpace = newMovementSpace;
        this.useGravity = newUseGravity;
        this.useForward = newUseForward;
    }
}

[System.Serializable]
public struct MovementModifierInfo
{
    [Header("Movement Modifier Info")]
    public MOVEMENT_SOURCE movementSource;

    [Space(10)]
    public Vector3 movementForce;

    [Space(10)]
    [Range(0, 1)]
    public float movementEffectiveness;

    public MovementModifierInfo (MOVEMENT_SOURCE newMovementSource, Vector3 newMovementForce, float newMovementEffectiveness)
    {
        this.movementSource = newMovementSource;
        this.movementForce = newMovementForce;
        this.movementEffectiveness = newMovementEffectiveness;
    }
}

public enum MOVEMENT_TYPE
{
    NONE,
    INPUT,
    VELOCITY,
    WARP
}

public enum MOVEMENT_SOURCE
{
    NONE,
    INPUT,
    ACTION,
    NAVIGATION,
    STATUS
}
