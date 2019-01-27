using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityController : MonoBehaviour
{
    [Header("Entity Movement Attributes")] public List<EntityMovement> movement = new List<EntityMovement>();

    [Space(10)] public EntityMovementAttributes movementAttributes;

    [Space(10)] public bool isCapturingMovement = true;

    [Header("Entity Camera Attributes")] public EntityCamera entityCamera;

    private CameraViewPortInfo viewPortInfo;

    [Space(10)] public bool isCapturingCamera = true;

    public void InitializeController()
    {        
        EntityMovement[] movementControllers = GetComponentsInChildren<EntityMovement>();

        for (int i = 0; i < movementControllers.Length; i++)
        {
            movement.Add(movementControllers[i]);
        }

        entityCamera = Camera.main.GetComponent<EntityCamera>();
    }

    public void UpdateController(InputInfo inputValues)
    {   
        if (isCapturingCamera)
        {
            ManageCamera();
        }

        if (isCapturingMovement)
        {
            ManageMovement(inputValues);
        }     
    }
    
    private void ManageMovement(InputInfo input)
    {
        List<EntityMovement> activeRoots = ReturnActiveRoots(input);
        
        for (int i = 0; i < movement.Count; i++)
        {
            movement[i].Move(input, ReturnIfRootIsActive(movement[i], activeRoots), movementAttributes, viewPortInfo, i, movement);
        }
    }

    private void ManageCamera()
    {
        viewPortInfo = entityCamera.ReturnViewportInfo();
    }

    public void StartPlayer()
    {
        for (int i = 0; i < movement.Count; i++)
        {
            movement[i].isActive = true;
        }      
    }

    public void StopPlayer()
    {
        for (int i = 0; i < movement.Count; i++)
        {
            movement[i].isActive = false;
        }      
    }

    public void StartCamera()
    {
        entityCamera.StartCamera();
    }

    public void StopCamera()
    {
        entityCamera.StopCamera();
    }

    public void Reset(EntityResetAttributes resetAttributes)
    {
        for (int i = 0; i < movement.Count; i++)
        {
            movement[i].transform.position = resetAttributes.resetPosition[i];
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
