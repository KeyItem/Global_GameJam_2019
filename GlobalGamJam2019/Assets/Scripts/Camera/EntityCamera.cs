using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCamera : GenericCamera
{
    [Header("Camera Movement Attributes")]
    private Vector3 cameraMovementSmoothingVelocity;

    [Header("Camera Zoom Attributes")]
    private float cameraZoomSmoothingVelocity;

    [Header("Camera Size Attributes")] public CameraViewPortInfo viewPortInfo;

    [Space(10)] public Vector2 viewportOffset;

    [Header("Player Camera Attributes")]
    private List<Transform> targetTransforms = new List<Transform>();
    
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
        AddMultipleTargetsToList(targetTransforms);
        
        viewPortInfo = new CameraViewPortInfo(Camera.main, viewportOffset);

        base.InitializeCamera();
    }

    public override void ManageCamera()
    {
        ManageCameraMovement();
        ManageCameraZooming();
    }

    private void ManageCameraMovement()
    {
        Vector3 newCameraCenterPoint = ReturnTargetsCenter(cameraTargets);
        Vector3 newCameraTargetPosition = newCameraCenterPoint + cameraMovementAttributes.cameraOffset;
        Vector3 newCameraPosition = Vector3.SmoothDamp(transform.position, newCameraTargetPosition, ref cameraMovementSmoothingVelocity, cameraMovementAttributes.cameraMoveSmoothing);

        MoveCamera(newCameraPosition);
    }

    private void ManageCameraZooming()
    {
        float maxDistanceBetweenTargets = ReturnLargestDistanceBetweenTargets(cameraTargets);
        float newCameraFieldOfView = Mathf.SmoothDamp(targetCamera.fieldOfView, maxDistanceBetweenTargets, ref cameraZoomSmoothingVelocity, cameraZoomingAttributes.cameraZoomSmoothing);

        ZoomCamera(newCameraFieldOfView);
    }

    public CameraViewPortInfo ReturnViewportInfo()
    {
        return viewPortInfo;
    }

    private Vector3 ReturnTargetsCenter(List<Transform> targets)
    {
        if (targets.Count == 0)
        {
            return Vector3.zero;
        }

        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        Bounds centerBounds = new Bounds(targets[0].position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            centerBounds.Encapsulate(targets[i].position);
        }

        return centerBounds.center;
    }

    private float ReturnLargestDistanceBetweenTargets(List<Transform> targets)
    {
        Bounds maxDistanceBounds = new Bounds(targets[0].position, Vector3.zero);
        float maxDistance = 0;

        for (int i = 0; i < targets.Count; i++)
        {
            maxDistanceBounds.Encapsulate(targets[i].position);
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
