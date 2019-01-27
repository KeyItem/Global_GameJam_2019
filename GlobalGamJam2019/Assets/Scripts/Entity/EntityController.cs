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

    [Header("Entity Trail Renderer Attributes")]
    private TrailRenderer[] trailRenderers;

    [Header("Grow Animator")] public Animator growAnimator;

    public void InitializeController()
    {        
        EntityMovement[] movementControllers = GetComponentsInChildren<EntityMovement>();

        for (int i = 0; i < movementControllers.Length; i++)
        {
            movement.Add(movementControllers[i]);
        }

        entityCamera = Camera.main.GetComponent<EntityCamera>();

        trailRenderers = GetComponentsInChildren<TrailRenderer>();
    }

    public void UpdateController(InputInfo inputValues, List<EntityMovement> aliveRoots)
    {   
        if (isCapturingCamera)
        {
            ManageCamera();
        }

        if (isCapturingMovement)
        {
            ManageMovement(inputValues, aliveRoots);
        }     
    }
    
    private void ManageMovement(InputInfo input, List<EntityMovement> aliveRoots)
    {
        List<EntityMovement> activeRoots = ReturnActiveRoots(input);
        
        for (int i = 0; i < movement.Count; i++)
        {
            movement[i].Move(input, ReturnIfRootIsActive(movement[i], activeRoots), movementAttributes, viewPortInfo, i, movement, aliveRoots);
        }
    }

    private void ManageCamera()
    {
        viewPortInfo = entityCamera.ReturnViewportInfo();
    }

    public void StartPlayerMovement()
    {
        for (int i = 0; i < movement.Count; i++)
        {
            movement[i].isActive = true;
        }      
    }

    public void StopPlayerMovement()
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

    public void StartPlayerTrailRenderer()
    {
        for (int i = 0; i < trailRenderers.Length; i++)
        {
            
        }
    }

    public void StopPlayerTrailRenderer()
    {
        for (int i = 0; i < trailRenderers.Length; i++)
        {
            trailRenderers[i].Clear();
        }
    }

    public void StartGrow()
    {
        growAnimator.SetTrigger("isGrow");
    }

    public void StopGrow()
    {
        growAnimator.SetTrigger("isIdle");
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
