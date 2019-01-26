using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDetection : MonoBehaviour
{
    [Header("Entity Detection Attributes")]
    public EntityDetectionInfo detectionInfo = new EntityDetectionInfo();

    private List<DetectedEntity> cachedDetectionEntities = new List<DetectedEntity>();

    [Space(10)]
    public EntityDetectionAttributes entityDetectionAttributes;

    public virtual void DetectEntities()
    {
        Collider[] detectedColliders = Physics.OverlapSphere(transform.position, entityDetectionAttributes.entityDetectionRadius, entityDetectionAttributes.entityDetectionMask);

        cachedDetectionEntities = detectionInfo.detectedEntities;

        detectionInfo = new EntityDetectionInfo(ReturnDetectedEntityGameObjects(detectedColliders), cachedDetectionEntities, transform.position);
    }

    public virtual EntityDetectionInfo ReturnDetectionInfo()
    {
        DetectEntities();

        return detectionInfo;
    }

    public virtual List<DetectedEntity> ReturnDetectedEntityGameObjects(Collider[] hitColliders)
    {
        List<DetectedEntity> newDetectedEntities = new List<DetectedEntity>();

        for (int i = 0; i < hitColliders.Length; i++)
        {
            newDetectedEntities.Add(new DetectedEntity(hitColliders[i].gameObject, ReturnIsEntityInLineOfSight(hitColliders[i].transform)));
        }

        return newDetectedEntities;
    }

    public bool ReturnIsEntityInLineOfSight(Transform detectedEntityTransform)
    {
        Vector3 newInterceptVector = (detectedEntityTransform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, newInterceptVector, entityDetectionAttributes.entityDetectionRadius / 2, entityDetectionAttributes.entityTerrainMask))
        {
            return false;
        }

        return true;
    }
}

[System.Serializable]
public struct EntityDetectionInfo
{
    [Header("Detection Info Attributes")]
    public List<DetectedEntity> detectedEntities;
    public List<DetectedEntity> cachedEntities;

    [Space(10)]
    public Vector3 detectorEntityPosition;

    [Space(10)]
    public float closestEntityDistance;
    public float furthestEntityDistance;

    public EntityDetectionInfo (List<DetectedEntity> newDetectedEntities, List<DetectedEntity> newCachedEntities, Vector3 newDetectorPosition)
    {
        this.detectedEntities = newDetectedEntities;
        this.cachedEntities = newCachedEntities;
        this.detectorEntityPosition = newDetectorPosition;

        this.closestEntityDistance = 0f;
        this.furthestEntityDistance = 0f;

        this.closestEntityDistance = ReturnClosestDetectedEntityDistance();
        this.furthestEntityDistance = ReturnFurthestDetectedEntityDistance();
    }

    public float ReturnClosestDetectedEntityDistance(float minRange = 0, float maxRange = 0)
    {
        float closestEntityDistance = float.MaxValue;

        if (detectedEntities.Count == 0)
        {
            return 0;
        }

        for (int i = 0; i < detectedEntities.Count; i++)
        {
            float entityDistance = Vector3.Distance(detectorEntityPosition, detectedEntities[i].detectedEntityGameObject.transform.position);

            if (minRange != 0)
            {
                if (entityDistance < minRange)
                {
                    continue;
                }
            }

            if (maxRange != 0)
            {
                if (entityDistance > maxRange)
                {
                    continue;
                }
            }

            if (entityDistance < closestEntityDistance)
            {
                closestEntityDistance = entityDistance;
            }
        }

        return closestEntityDistance;
    }

    public float ReturnFurthestDetectedEntityDistance(float minRange = 0, float maxRange = 0)
    {
        float furthestEntityDistance = float.MinValue;

        if (detectedEntities.Count == 0)
        {
            return 0;
        }

        for (int i = 0; i < detectedEntities.Count; i++)
        {
            float entityDistance = Vector3.Distance(detectorEntityPosition, detectedEntities[i].detectedEntityGameObject.transform.position);

            if (minRange != 0)
            {
                if (entityDistance < minRange)
                {
                    continue;
                }
            }

            if (maxRange != 0)
            {
                if (entityDistance > maxRange)
                {
                    continue;
                }
            }

            if (entityDistance > furthestEntityDistance)
            {
                furthestEntityDistance = entityDistance;
            }
        }

        return furthestEntityDistance;
    }

    public GameObject ReturnFurthestDetectedEntity(float minRange = 0, float maxRange = 0)
    {
        GameObject furthestEntity = detectedEntities[0].detectedEntityGameObject;
        float furthestEntityDistance = float.MinValue;

        for (int i = 0; i < detectedEntities.Count; i++)
        {
            float entityDistance = Vector3.Distance(detectorEntityPosition, detectedEntities[i].detectedEntityGameObject.transform.position);

            if (minRange != 0)
            {
                if (entityDistance < minRange)
                {
                    continue;
                }
            }

            if (maxRange != 0)
            {
                if (entityDistance > maxRange)
                {
                    continue;
                }
            }

            if (entityDistance > furthestEntityDistance)
            {
                furthestEntity = detectedEntities[i].detectedEntityGameObject;
            }
        }

        return furthestEntity;
    }

    public GameObject ReturnClosestDetectedEntity(float minRange, float maxRange = 0)
    {
        GameObject closestEntity = detectedEntities[0].detectedEntityGameObject;
        float closestEntityDistance = float.MaxValue;

        for (int i = 0; i < detectedEntities.Count; i++)
        {
            float entityDistance = Vector3.Distance(detectorEntityPosition, detectedEntities[i].detectedEntityGameObject.transform.position);

            if (minRange != 0)
            {
                if (entityDistance < minRange)
                {
                    continue;
                }
            }

            if (maxRange != 0)
            {
                if (entityDistance > maxRange)
                {
                    continue;
                }
            }

            if (entityDistance < closestEntityDistance)
            {
                closestEntity = detectedEntities[i].detectedEntityGameObject;
            }
        }

        return closestEntity;
    }

    public List<GameObject> ReturnCompairisonAgainstOldObjects()
    {
        List<GameObject> newComparedEntities = new List<GameObject>();

        if (detectedEntities == cachedEntities)
        {
            return newComparedEntities;
        }

        for (int i = 0; i < cachedEntities.Count; i++)
        {
            if (detectedEntities.Contains(cachedEntities[i]))
            {
                continue;
            }

            newComparedEntities.Add(cachedEntities[i].detectedEntityGameObject);
        }

        return newComparedEntities;
    }

    public List<GameObject> ReturnCompairisonAgainstNewObjects()
    {
        List<GameObject> newComparedEntities = new List<GameObject>();

        if (detectedEntities == cachedEntities)
        {
            return newComparedEntities;
        }

        for (int i = 0; i < detectedEntities.Count; i++)
        {
            if (cachedEntities.Contains(detectedEntities[i]))
            {
                continue;
            }

            newComparedEntities.Add(detectedEntities[i].detectedEntityGameObject);
        }

        return newComparedEntities;
    }
}

[System.Serializable]
public struct EntityDetectionAttributes
{
    [Header("Detection Attributes")]
    public float entityDetectionRadius;

    [Space(10)]
    public LayerMask entityDetectionMask;
    public LayerMask entityTerrainMask;
}

[System.Serializable]
public struct DetectedEntity
{
    [Header("Detected Entity Attributes")]
    public GameObject detectedEntityGameObject;

    [Space(10)]
    public bool isEntityInLineOfSight;

    public DetectedEntity (GameObject newDetectedEntityGameObject, bool newIsEntityInLineOfSight)
    {
        this.detectedEntityGameObject = newDetectedEntityGameObject;
        this.isEntityInLineOfSight = newIsEntityInLineOfSight;
    }
}
 
