using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericCamera : MonoBehaviour
{
    [Header("Base Camera Attributes")]
    [HideInInspector]
    public Camera targetCamera;

    [Space(10)]
    public List<Transform> cameraTargets = new List<Transform>();

    [Header("Camera Base Movement Attributes")]
    public CameraMovementAttributes cameraMovementAttributes;

    [Header("Camera Base Zooming Attributes")]
    public CameraZoomAttributes cameraZoomingAttributes;

    public virtual void LateUpdate()
    {
        ManageCamera();
    }

    public virtual void IntializeCamera()
    {
        targetCamera = GetComponent<Camera>();
    }

    public virtual void ManageCamera()
    {

    }

    public virtual List<Transform> ReturnPlayers()
    {
        List<Transform> newPlayers = new List<Transform>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            newPlayers.Add(players[i].transform);
        }

        return newPlayers;
    }

    public virtual void MoveCamera(Vector3 newCameraPosition)
    {
        transform.position = newCameraPosition;
    }

    public virtual void RotateCamera(Quaternion newCameraRotation)
    {
        transform.rotation = newCameraRotation;
    }

    public virtual void ZoomCamera(float newCameraFieldOfView)
    {
        targetCamera.fieldOfView = newCameraFieldOfView;
    }

    public virtual void ResetCamera()
    {

    }

    public virtual List<Transform> ConvertGameObjectListToTransforms(List<GameObject> gameObjectList)
    {
        List<Transform> newTransforms = new List<Transform>();

        for (int i = 0; i < gameObjectList.Count; i++)
        {
            newTransforms.Add(gameObjectList[i].transform);
        }

        return newTransforms;
    }

    public virtual void AddMultipleTargetsToList(List<Transform> targetsToAdd)
    {
        for (int i = 0; i < targetsToAdd.Count; i++)
        {
            if (cameraTargets.Contains(targetsToAdd[i]))
            {
                continue;
            }

            cameraTargets.Add(targetsToAdd[i]);
        }
    }

    public virtual void RemoveMultipleTargetsFromList(List<Transform> targetsToRemove)
    {
        for (int i = 0; i < targetsToRemove.Count; i++)
        {
            if (!cameraTargets.Contains(targetsToRemove[i]))
            {
                continue;
            }

            cameraTargets.Remove(targetsToRemove[i]);
        }
    }

    public virtual void AddTargetToList(Transform targetTransform)
    {
        if (cameraTargets.Contains(targetTransform))
        {
            return;
        }

        cameraTargets.Add(targetTransform);
    }

    public virtual void RemoveTargetFromList(Transform targetTransform)
    {
        if (cameraTargets.Contains(targetTransform))
        {
            cameraTargets.Remove(targetTransform);
        }

        return;
    }
}

[System.Serializable]
public struct CameraMovementAttributes
{
    [Header("Base Camera Movement Attributes")]
    [Range(0, 10)]
    public float cameraMoveSmoothing;

    [Space(10)]
    [Range(0, 10)]
    public float cameraRotationalSmoothing;

    [Space(10)]
    public Vector3 cameraOffset;
}

[System.Serializable]
public struct CameraZoomAttributes
{
    [Header("Base Camera Zoom Attributes")]
    public float cameraMinZoom;
    public float cameraMaxZoom;

    [Space(10)]
    [Range(0, 1)]
    public float cameraZoomSmoothing;
}
