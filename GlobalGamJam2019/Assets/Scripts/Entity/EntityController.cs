using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [Header("Entity Input Attributes")] public EntityInput input;

    [Space(10)] public InputInfo inputValues;

    [Space(10)] public bool isCapturingInput = true;

    [Header("Entity Movement Attributes")] public EntityMovement[] movement;

    [Space(10)] public bool isCapturingMovement = true;

    private void Start()
    {
        InitializeController();
    }

    private void Update()
    {
        UpdateController();
    }

    private void InitializeController()
    {
        input = GetComponent<EntityInput>();
        movement = GetComponentsInChildren<EntityMovement>();
    }

    private void UpdateController()
    {
        if (isCapturingInput)
        {
            ManageInput();
        }

        if (isCapturingMovement)
        {
            ManageMovement();
        }
    }

    private void ManageInput()
    {
        input.GetInput();

        inputValues = input.ReturnInput();
    }

    private void ManageMovement()
    {
        for (int i = 0; i < movement.Length; i++)
        {
            movement[i].Move(inputValues);
        }
    }
}
