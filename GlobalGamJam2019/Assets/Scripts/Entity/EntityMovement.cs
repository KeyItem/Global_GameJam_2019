﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    [Header("Entity Movement Attributes")] public EntityMovementAttributes movementAttributes;

    private Vector2 lastDirection = Vector2.down;
    
    public void Move(InputInfo input, bool isControlled)
    {
        Vector2 newMovement = ReturnMovementVector(ReturnInputMovementVector(input), isControlled);
        
        transform.Translate(newMovement * Time.deltaTime);

        lastDirection = newMovement;
    }

    public Vector2 ReturnInputMovementVector(InputInfo input)
    {
        float xAxisRaw = input.ReturnCurrentAxis("HorizontalAxis").axisValue;
        float yAxisRaw = input.ReturnCurrentAxis("VerticalAxis").axisValue;
        
        return new Vector2(xAxisRaw, 0);
    }

    public Vector2 ReturnMovementVector(Vector2 inputVector, bool isControlled)
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
        
        return newMovementVector;
    }

    public bool CheckIfMovementIsOutOfBounds()
    {
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