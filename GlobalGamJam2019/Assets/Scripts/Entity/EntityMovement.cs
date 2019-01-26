using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    [Header("Entity Movement Attributes")] public EntityMovementAttributes movementAttributes;

    private Vector2 lastDirection = Vector2.down;
    
    public void Move(InputInfo input, bool isControlled, CameraViewPortInfo viewPortInfo, int rootIndex, List<EntityMovement> activeRoots)
    {
        Vector2 newMovement = ReturnMovementVector(ReturnInputMovementVector(input), isControlled, viewPortInfo, rootIndex, activeRoots);
        
        transform.Translate(newMovement * Time.deltaTime);

        lastDirection = newMovement;
    }

    public Vector2 ReturnInputMovementVector(InputInfo input)
    {
        float xAxisRaw = input.ReturnCurrentAxis("HorizontalAxis").axisValue;
        float yAxisRaw = input.ReturnCurrentAxis("VerticalAxis").axisValue;
        
        return new Vector2(xAxisRaw, 0);
    }

    public Vector2 ReturnMovementVector(Vector2 inputVector, bool isControlled, CameraViewPortInfo viewPortInfo, int rootIndex, List<EntityMovement> activeRoots)
    {
        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }
        
        Vector2 newMovementVector = movementAttributes.ReturnOngoingSpeed();

        if (isControlled)
        {
            newMovementVector = inputVector * movementAttributes.movementSpeed + movementAttributes.ReturnOngoingSpeed();
        }

        if (CheckIfMovementIsOutOfBounds(newMovementVector, viewPortInfo))
        {
            newMovementVector.x = 0;
        }
        
        if (CheckIfMovementIsOverlapping(newMovementVector, viewPortInfo, rootIndex, activeRoots))
        {
            newMovementVector.x = 0;
        }
                
        return newMovementVector;
    }

    public bool CheckIfMovementIsOutOfBounds(Vector2 movementVelocity, CameraViewPortInfo viewportInfo)
    {
        float newXPosition = transform.position.x + (movementVelocity.x * Time.deltaTime);
        
        if (newXPosition < viewportInfo.viewportWorldMin.x)
        {            
            return true;
        }
        else if (newXPosition > viewportInfo.viewportWorldMax.x)
        {
            return true;
        }
        
        return false;
    }

    public bool CheckIfMovementIsOverlapping(Vector2 movementVelocity, CameraViewPortInfo viewPortInfo, int rootIndex, List<EntityMovement> roots) //Garbage code just to test my theory
    {
        float newXPosition = transform.position.x + (movementVelocity.x * Time.deltaTime);
        
        float targetXMin = 0f;
        float targetXMax = 0f;

        float offsetValue = 1f;
        
        if (roots.Count == 1)
        {
            targetXMin = viewPortInfo.viewportWorldMin.x;
            targetXMax = viewPortInfo.viewportWorldMax.x;

            if (newXPosition < targetXMin || newXPosition > targetXMax)
            {
                return true;
            }
        }
        else if (roots.Count == 2)
        {
            if (rootIndex == 0)
            {
                targetXMin = viewPortInfo.viewportWorldMin.x;
                targetXMax = roots[1].transform.position.x - offsetValue;

                if (newXPosition < targetXMin || newXPosition > targetXMax)
                {
                    return true;
                }
            }
            else
            {
                targetXMin = roots[0].transform.position.x + offsetValue;
                targetXMax = viewPortInfo.viewportWorldMax.x;

                if (newXPosition < targetXMin || newXPosition > targetXMax)
                {
                    return true;
                }
            }
        }
        else if (roots.Count == 3)
        {            
            if (rootIndex == 0)
            {
                targetXMin = viewPortInfo.viewportWorldMin.x;
                targetXMax = roots[1].transform.position.x - offsetValue;
                                
                if (newXPosition < targetXMin || newXPosition > targetXMax)
                {
                    return true;
                }
            }
            else if (rootIndex == 1)
            {
                targetXMin = roots[0].transform.position.x + offsetValue;
                targetXMax = roots[2].transform.position.x - offsetValue;

                if (newXPosition < targetXMin || newXPosition > targetXMax)
                {
                    return true;
                }
            }
            else
            {
                targetXMin = roots[1].transform.position.x + offsetValue;
                targetXMax = viewPortInfo.viewportWorldMax.x;

                if (newXPosition < targetXMin || newXPosition > targetXMax)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

[System.Serializable]
public struct EntityMovementAttributes
{
    [Header("Entity Movement Speed")] public float movementSpeed;
    
    [Header("Entity Ongoing Movement Speed")]
    public float ongoingSpeed;

    [Space(10)] public Vector2 ongoingDirection;

    public Vector2 ReturnOngoingSpeed()
    {
        return ongoingDirection * ongoingSpeed;
    }
}