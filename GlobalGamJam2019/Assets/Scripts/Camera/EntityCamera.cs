using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class EntityCamera : GenericCamera
{
    [Header("Camera Movement Attributes")]
    private Vector3 cameraMovementSmoothingVelocity;

    [Header("Camera Zoom Attributes")]
    private float cameraZoomSmoothingVelocity;

    [Header("Camera Size Attributes")] public CameraViewPortInfo viewPortInfo;

    [Space(10)] public Vector2 viewportOffset;  
    private void Start()
    {
        InitializeCamera();
    }
    
    public override void LateUpdate()
    {
        ManageCamera();
    }

    public override void InitializeCamera()
    {        
        viewPortInfo = new CameraViewPortInfo(Camera.main, viewportOffset);

        base.InitializeCamera();
    }

    public void StartCamera()
    {
        AddMultipleTargetsToList(cameraTargets);
    }

    public void StopCamera()
    {
        cameraTargetInfo.Clear();
    }

    public override void ManageCamera()
    {
        if (cameraTargetInfo.Count > 0)
        {
            List<CameraTargetInfo> activeTargets = ReturnActiveTargets(cameraTargetInfo);
        
            ManageCameraMovement(activeTargets);
            ManageCameraZooming(activeTargets);
        }
    }

    private void ManageCameraMovement(List<CameraTargetInfo> targetInfo)
    {
        Vector3 newCameraCenterPoint = ReturnTargetsCenter(targetInfo);
        Vector3 newCameraTargetPosition = newCameraCenterPoint + cameraMovementAttributes.cameraOffset;
        Vector3 newCameraPosition = Vector3.SmoothDamp(transform.position, newCameraTargetPosition, ref cameraMovementSmoothingVelocity, cameraMovementAttributes.cameraMoveSmoothing);

        MoveCamera(newCameraPosition);
    }

    private void ManageCameraZooming(List<CameraTargetInfo> targetInfo)
    {
        float maxDistanceBetweenTargets = ReturnLargestDistanceBetweenTargets(targetInfo);
        float newCameraFieldOfView = Mathf.SmoothDamp(targetCamera.fieldOfView, maxDistanceBetweenTargets, ref cameraZoomSmoothingVelocity, cameraZoomingAttributes.cameraZoomSmoothing);

        ZoomCamera(newCameraFieldOfView);
    }

    public CameraViewPortInfo ReturnViewportInfo()
    {
        return viewPortInfo;
    }

    private Vector3 ReturnTargetsCenter(List<CameraTargetInfo> targets)
    {
        if (targets.Count == 0)
        {
            return Vector3.zero;
        }

        if (targets.Count == 1)
        {
            return targets[0].targetTransform.position;
        }

        Bounds centerBounds = new Bounds(targets[0].targetTransform.position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            centerBounds.Encapsulate(targets[i].targetTransform.position);
        }

        return centerBounds.center;
    }

    private float ReturnLargestDistanceBetweenTargets(List<CameraTargetInfo> targets)
    {
        if (targets.Count == 0)
        {
            return 0f;
        }
        
        Bounds maxDistanceBounds = new Bounds(targets[0].targetTransform.position, Vector3.zero);
        float maxDistance = 0;

        for (int i = 0; i < targets.Count; i++)
        {
            maxDistanceBounds.Encapsulate(targets[i].targetTransform.position);
        }

        maxDistance = maxDistanceBounds.size.x;

        if (maxDistance < cameraZoomingAttributes.cameraMinZoom)
        {
            return cameraZoomingAttributes.cameraMinZoom;
        }
        else if (maxDistance > cameraZoomingAttributes.cameraMaxZoom)
        {
            return cameraZoomingAttributes.cameraMaxZoom;
        }

        return maxDistanceBounds.size.x;
    }

    private List<CameraTargetInfo> ReturnActiveTargets(List<CameraTargetInfo> targets)
    {
        List<CameraTargetInfo> activeTargets = new List<CameraTargetInfo>();

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].targetMovement.isActive)
            {
                activeTargets.Add(targets[i]);
            }
        }

        return activeTargets;
    }
}

[System.Serializable]
public struct CameraTargetInfo
{
    [Header("Camera Target Info")] public Transform targetTransform;

    [Space(10)] public EntityMovement targetMovement;

    public CameraTargetInfo(Transform newTargetTransform)
    {
        this.targetTransform = newTargetTransform;
        this.targetMovement = newTargetTransform.GetComponent<EntityMovement>();
    }
}

[System.Serializable]
public struct CameraViewPortInfo
{
    public Vector2 viewportWorldMin;
    public Vector2 viewportWorldMax;

    public CameraViewPortInfo(Camera targetCamera, Vector2 newViewportOffset)
    {            
        this.viewportWorldMin = targetCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        this.viewportWorldMax = targetCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        this.viewportWorldMin += newViewportOffset;
        this.viewportWorldMax -= newViewportOffset;
    }
}
