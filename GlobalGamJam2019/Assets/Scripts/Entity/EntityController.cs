using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [Header("Entity Input Attributes")] public EntityInput input;

    [Space(10)] public InputInfo inputValues;

    [Space(10)] public bool isCapturingInput = true;

    [Header("Entity Movement Attributes")] public List<EntityMovement> movement = new List<EntityMovement>();

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
        
        EntityMovement[] movementControllers = GetComponentsInChildren<EntityMovement>();

        for (int i = 0; i < movementControllers.Length; i++)
        {
            movement.Add(movementControllers[i]);
        }
    }

    private void UpdateController()
    {
        if (isCapturingInput)
        {
            ManageInput();
        }

        if (isCapturingMovement)
        {
            ManageMovement(inputValues);
        }
    }

    private void ManageInput()
    {
        input.GetInput();

        inputValues = input.ReturnInput();
    }

    private void ManageMovement(InputInfo input)
    {
        List<EntityMovement> activeRoots = ReturnActiveRoots(input);
        
        for (int i = 0; i < movement.Count; i++)
        {
            movement[i].Move(inputValues, ReturnIfRootIsActive(movement[i], activeRoots));
        }
    }

    private List<EntityMovement> ReturnActiveRoots(InputInfo input)
    {
        List<EntityMovement> activeRoots = new List<EntityMovement>();

        if (input.ReturnCurrentButtonState("Button0"))
        {
            activeRoots.Add(movement[0]);
        }

        if (input.ReturnCurrentButtonState("Button1"))
        {
            activeRoots.Add(movement[1]);
        }

        if (input.ReturnCurrentButtonState("Button2"))
        {
            activeRoots.Add(movement[2]);
        }

        return activeRoots;
    }

    private bool ReturnIfRootIsActive(EntityMovement targetRoot, List<EntityMovement> activeRoots)
    {
        if (activeRoots.Contains(targetRoot))
        {
            return true;
        }

        return false;
    }
}
