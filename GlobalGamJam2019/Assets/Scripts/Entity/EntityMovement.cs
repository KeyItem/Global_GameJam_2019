﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    [Header("Entity Movement Attributes")]
    private Vector2 lastDirection = Vector2.down;

    [Space(10)] public bool isActive = false;
        
    public void Move(InputInfo input, bool isControlled, EntityMovementAttributes movementAttributes, CameraViewPortInfo viewPortInfo, int rootIndex, List<EntityMovement> activeRoots, List<EntityMovement> aliveRoots)
    {
        if (isActive)
        {
            Vector2 newMovement = ReturnMovementVector(ReturnInputMovementVector(input), isControlled, movementAttributes, viewPortInfo, rootIndex, activeRoots, aliveRoots);
        
            transform.Translate(newMovement * Time.deltaTime);
            
            Rotate(newMovement * Time.deltaTime);

            lastDirection = newMovement;
        }        
    }

    public void Rotate(Vector2 inputVector)
    {
        //transform.rotation = Quaternion.Euler(inputVector);
    }

    public void EnableMovement()
    {
        isActive = true;
    }

    public void DisableMovement()
    {
        isActive = false;
    }

    public Vector2 ReturnInputMovementVector(InputInfo input)
    {
        float xAxisRaw = input.ReturnCurrentAxis("HorizontalAxis").axisValue;
        float yAxisRaw = input.ReturnCurrentAxis("VerticalAxis").axisValue;
        
        return new Vector2(xAxisRaw, 0);
    }

    public Vector2 ReturnMovementVector(Vector2 inputVector, bool isControlled, EntityMovementAttributes movementAttributes, CameraViewPortInfo viewPortInfo, int rootIndex, List<EntityMovement> activeRoots, List<EntityMovement> aliveRoots)
    {
        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }
        
        Vector2 newMovementVector = movementAttributes.ReturnOngoingSpeed(aliveRoots.Count);
        
        if (isControlled)
        {
            newMovementVector = inputVector * movementAttributes.movementSpeed + movementAttributes.ReturnOngoingSpeed(aliveRoots.Count);
        }

        if (CheckIfMovementIsOutOfBounds(newMovementVector, viewPortInfo))
        {
            newMovementVector.x = 0;
        }
        
        if (CheckIfMovementIsOverlapping(newMovementVector, movementAttributes, viewPortInfo, rootIndex, activeRoots))
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

    public bool CheckIfMovementIsOverlapping(Vector2 movementVelocity, EntityMovementAttributes movementAttributes, CameraViewPortInfo viewPortInfo, int rootIndex, List<EntityMovement> roots) //Garbage code just to test my theory
    {
        float newXPosition = transform.position.x + (movementVelocity.x * Time.deltaTime);
        
        float targetXMin = 0f;
        float targetXMax = 0f;

        float offsetValue = movementAttributes.collisionOffset;
        
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
                targetXMax = roots[1].isActive ? roots[1].transform.position.x - offsetValue : viewPortInfo.viewportWorldMax.x;

                if (newXPosition < targetXMin || newXPosition > targetXMax)
                {
                    return true;
                }
            }
            else
            {
                targetXMin = roots[0].isActive ? roots[0].transform.position.x + offsetValue : viewPortInfo.viewportWorldMin.x;
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

                if (roots[1].isActive)
                {
                    targetXMax = roots[1].transform.position.x - offsetValue;
                }
                else if (roots[2].isActive)
                {
                    targetXMax = roots[2].transform.position.x - offsetValue;
                }
                else
                {
                    targetXMax = viewPortInfo.viewportWorldMax.x;
                }
                                
                if (newXPosition < targetXMin || newXPosition > targetXMax)
                {
                    return true;
                }
            }
            else if (rootIndex == 1)
            {
                targetXMin = roots[0].isActive ? roots[0].transform.position.x + offsetValue : viewPortInfo.viewportWorldMin.x;

                targetXMax = roots[2].isActive ? roots[2].transform.position.x - offsetValue : viewPortInfo.viewportWorldMax.x;
           
                if (newXPosition < targetXMin || newXPosition > targetXMax)
                {
                    return true;
                }
            }
            else
            {
                if (roots[1].isActive)
                {
                    targetXMin = roots[1].transform.position.x + offsetValue;
                }
                else if (roots[0].isActive)
                {
                    targetXMin = roots[0].transform.position.x + offsetValue;
                }
                else
                {
                    targetXMin = viewPortInfo.viewportWorldMin.x;
                }
                
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
    public float oneRootSpeed;
    public float twoRootSpeed;
    public float threeRootSpeed;

    [Space(10)] public Vector2 ongoingDirection;

    [Header("Entity Collision Offset")] 
    [Range(0.05f, 1.25f)]
    public float collisionOffset;

    public Vector2 ReturnOngoingSpeed(int rootCount)
    {
        return (ongoingDirection * ReturnRootSpeed(rootCount));
    }

    private float ReturnRootSpeed(int rootCount)
    {
        if (rootCount == 1)
        {
            return oneRootSpeed;
        }
        else if (rootCount == 2)
        {
            return twoRootSpeed;
        }
        else
        {
            return threeRootSpeed;
        }
    }
}