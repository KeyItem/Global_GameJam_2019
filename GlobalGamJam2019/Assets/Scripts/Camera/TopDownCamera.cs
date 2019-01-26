using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : GenericCamera
{
    [Header("Camera Movement Attributes")]
    private Vector3 cameraMovementSmoothingVelocity;

    [Header("Camera Zoom Attributes")]
    private float cameraZoomSmoothingVelocity;

    [Header("Player Camera Attributes")]
    private List<Transform> playerTransforms = new List<Transform>();

    private List<EntityDetection> playerDetections = new List<EntityDetection>();

    private void Start()
    {
        IntializeCamera();
    }

    public override void LateUpdate()
    {
        ManageCamera();
    }

    public override void IntializeCamera()
    {
        playerTransforms = ReturnPlayers();
        playerDetections = ReturnPlayerDetections(playerTransforms);

        AddMultipleTargetsToList(playerTransforms);

        base.IntializeCamera();
    }

    public override void ManageCamera()
    {
        ManageCameraTargets();
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

    private void ManageCameraTargets()
    {
        for (int i = 0; i < playerDetections.Count; i++)
        {
            EntityDetection playerDetection = playerDetections[i];

            List<GameObject> newObjects = playerDetection.detectionInfo.ReturnCompairisonAgainstNewObjects();
            List<GameObject> oldObjects = playerDetection.detectionInfo.ReturnCompairisonAgainstOldObjects();

            if (newObjects.Count > 0)
            {
                AddMultipleTargetsToList(ConvertGameObjectListToTransforms(newObjects));
            }

            if (oldObjects.Count > 0)
            {
                RemoveMultipleTargetsFromList(ConvertGameObjectListToTransforms(oldObjects));
            }
        }
    }

    private void ManageCameraZooming()
    {
        float maxDistanceBetweenTargets = ReturnLargetsDistanceBetweenTargets(cameraTargets);
        float newCameraFieldOfView = Mathf.SmoothDamp(targetCamera.fieldOfView, maxDistanceBetweenTargets, ref cameraZoomSmoothingVelocity, cameraZoomingAttributes.cameraZoomSmoothing);

        ZoomCamera(newCameraFieldOfView);
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

    private float ReturnLargetsDistanceBetweenTargets(List<Transform> targets)
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

    private List<EntityDetection> ReturnPlayerDetections (List<Transform> playerTransforms)
    {
        List<EntityDetection> newPlayerDetectors = new List<EntityDetection>();

        for (int i = 0; i < playerTransforms.Count; i++)
        {
            EntityDetection newPlayerDetection = playerTransforms[i].GetComponent<EntityDetection>();

            if (newPlayerDetection != null)
            {
                newPlayerDetectors.Add(newPlayerDetection);
            }
        }

        return newPlayerDetectors;
    }
}
