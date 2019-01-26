using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EntityNavigation : MonoBehaviour
{
    [Header("Base Navigation Attributes")]
    public NavMeshPath entityNavMeshPath;

    [Header("Base Pathfinding Attributes")]
    public EntityPathingAttributes pathingAttributes;

    [Space(10)]
    public PATHING_TYPE pathType;

    [Space(10)]
    public bool canPathfind = true;

    private MovementInfo pathingMovement;

    [Space(10)]
    public PathingPoint[] pathingPoints;
    public PathingPoint[] cachedPathingPoints;

    private int pathfindPointIndex = 1;

    [Header("Base Pathing Movement Attributes")]
    public EntityPathingSpeedAttributes pathingSpeedAttributes;

    private float smoothMovementVelocity;

    [Header("Pathing Chase Attributes")]
    public Vector3 chaseStartPosition;

    [Header("Pathing Home Attributes")]
    public Vector3 homePoint = Vector3.zero;

    [Header("Pathing Patrol Attributes")]
    public Vector3[] localPatrolPoints;

    private Vector3[] patrolPoints = new Vector3[0];

    private int patrolPointIndex = 0;

    private void Start()
    {
        InitializeNavigation();
    }

    public virtual void InitializeNavigation()
    {
        InitializeHomePoint();
        InitializeWaypoints();
    }

    public virtual void InitializeHomePoint()
    {
        homePoint = ReturnHomePosition(transform.position);
    }

    public virtual void InitializeWaypoints()
    {
        patrolPoints = ReturnGlobalWaypoints(localPatrolPoints);
    }

    public virtual void SetPathingType(PATHING_TYPE newPathType)
    {
        pathType = newPathType;

        if (newPathType == PATHING_TYPE.CHASE)
        {
            chaseStartPosition = transform.position;
        }
    }

    public virtual void TogglePathfinding(bool pathfindToggle = true)
    {
        if (pathfindToggle)
        {
            canPathfind = true;
        }
        else
        {
            canPathfind = false;

            pathingMovement = new MovementInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f, Space.World, false, false);
        }
    }

    public virtual void NavigateEntity(EntityDetectionInfo detectionInfo, EntityStatusInfo statusInfo)
    {
        pathingMovement = new MovementInfo(MOVEMENT_SOURCE.NONE, Vector3.zero, 0f, Space.World, false, false);

        if (Time.frameCount % pathingAttributes.entityPathingFrequency == 0)
        {
            CalculatePathing(detectionInfo);
        }

        if (canPathfind)
        {
            PathfindEntity(detectionInfo);
        }
    }

    public virtual void PathfindEntity(EntityDetectionInfo detectionInfo)
    {
        if (canPathfind)
        {
            if (pathingPoints.Length > 0)
            {
                Vector3 nextCornerVelocity = ReturnNextPathingVelocity(pathingPoints, pathfindPointIndex);

                Debugger.DrawCustomDebugRay(transform.position, nextCornerVelocity, Color.cyan);
                Debugger.DrawNavMeshPath(entityNavMeshPath, Color.yellow);

                pathingMovement = new MovementInfo(MOVEMENT_SOURCE.NAVIGATION, nextCornerVelocity, 0f, Space.World, true, false);
            }
        }
    }

    public virtual void CalculatePathing(EntityDetectionInfo detectionInfo)
    {
        Vector3 pathEndPoint = ReturnTargetPathPoint(pathType, detectionInfo);

        if (pathEndPoint == Vector3.zero)
        {
            return;
        }

        entityNavMeshPath = ReturnNavMeshPath(pathEndPoint);

        cachedPathingPoints = pathingPoints;
        PathingPoint[] newPathingPoints = ReturnPathingPoints(ReturnHeightAdjustedPathingPoints(entityNavMeshPath.corners));

        if (CheckForPathMismatch(newPathingPoints, cachedPathingPoints))
        {
            pathingPoints = newPathingPoints;
        }
    }

    public virtual NavMeshPath ReturnNavMeshPath(Vector3 targetEndPoint)
    {
        NavMeshPath newPath = new NavMeshPath();

        NavMesh.CalculatePath(transform.position, targetEndPoint, NavMesh.AllAreas, newPath);

        return newPath;
    }

    public virtual MovementInfo ReturnPathfindVector()
    {
        return pathingMovement;
    }

    public virtual PathingPoint[] ReturnPathingPoints(Vector3[] pathPoints)
    {
        PathingPoint[] newPathingPoints = new PathingPoint[pathPoints.Length];

        for (int i = 0; i < newPathingPoints.Length; i++)
        {
            Vector3 pointStart = pathPoints[i];
            Vector3 pointEnd = i == pathPoints.Length - 1 ? pointStart : pathPoints[i + 1];
            Vector3 pointDirection = i == pathPoints.Length - 1 ? Vector3.zero : ReturnPathPointDirection(pointStart, pointEnd);

            newPathingPoints[i] = new PathingPoint(pathPoints[i], pointDirection, i);
        }

        return newPathingPoints;
    }

    public virtual Vector3 ReturnPathPointDirection(Vector3 startPoint, Vector3 targetPoint)
    {
        Vector3 newPathDirection = targetPoint - startPoint;

        return newPathDirection.normalized;
    }

    public virtual Vector3 ReturnNextPathingVelocity(PathingPoint[] pathingPoints, int pathingIndex)
    {
        Vector3 newPathingDirection = Vector3.zero;

        newPathingDirection = ReturnPathPointDirection(pathingPoints[0].pathingPoint, pathingPoints[1].pathingPoint);

        if (newPathingDirection.magnitude > 1)
        {
            newPathingDirection.Normalize();
        }

        newPathingDirection *= pathingSpeedAttributes.entityPathingChaseSpeed;

        return newPathingDirection;
    }

    public virtual Vector3[] ReturnHeightAdjustedPathingPoints(Vector3[] basePathingPoints)
    {
        Vector3[] newAdjustedPathingPoints = new Vector3[basePathingPoints.Length];

        for (int i = 0; i < newAdjustedPathingPoints.Length; i++)
        {
            newAdjustedPathingPoints[i] = basePathingPoints[i];

            newAdjustedPathingPoints[i].y = transform.position.y;
        }

        return newAdjustedPathingPoints;
    }

    public virtual bool CheckForPathMismatch(PathingPoint[] newPathingPoints, PathingPoint[] cachedPathfindingPoints)
    {
        if (newPathingPoints == null || cachedPathfindingPoints == null)
        {
            return true;
        }

        if (newPathingPoints.Length != cachedPathfindingPoints.Length)
        {
            return true;
        }

        for (int i = 0; i < newPathingPoints.Length; i++)
        {
            if (newPathingPoints[i].pathingPoint != cachedPathfindingPoints[i].pathingPoint)
            {
                return true;
            }
        }

        return false;
    }

    public virtual bool CheckForNextPathPoint(PathingPoint[] pathingPoints, int pathingIndex)
    {
        if (++pathingIndex > pathingPoints.Length)
        {
            return false;
        }

        return true;
    }

    public virtual bool CheckIfPathIsValid()
    {
        if (entityNavMeshPath.status == NavMeshPathStatus.PathInvalid)
        {
            Debug.Log("Path is invalid");

            return false;
        }

        return true;
    }

    public virtual bool CheckIfReachedTargetPoint(Vector3 targetPoint, float desiredDistance)
    {
        float remainingDistance = Vector3.Distance(transform.position, targetPoint);

        if (remainingDistance <= desiredDistance)
        {
            return true;
        }

        return false;
    }

    public virtual float ReturnTotalPathDistance(Vector3[] pathingPoints)
    {
        float totalDistanceOfPath = 0f;

        totalDistanceOfPath = Vector3.Distance(transform.position, pathingPoints[0]);

        for (int i = 1; i < pathingPoints.Length - 1; i++)
        {
            totalDistanceOfPath += Vector3.Distance(pathingPoints[i], pathingPoints[i + 1]);
        }

        return totalDistanceOfPath;
    }

    public virtual float ReturnDistanceToNextPoint(Vector3 nextPoint)
    {
        return Vector3.Distance(transform.position, nextPoint);
    }

    public virtual Vector3 ReturnHomePosition(Vector3 startPosition)
    {
        Vector3 newHomePosition = startPosition;

        RaycastHit rayHit = new RaycastHit();

        if (Physics.Raycast(startPosition, Vector3.down, out rayHit))
        {
            newHomePosition = rayHit.point;
            newHomePosition.y += 1;
        }

        return newHomePosition;
    }

    public virtual Vector3 ReturnTargetPathPoint (PATHING_TYPE pathType, EntityDetectionInfo detectionInfo)
    {
        switch(pathType)
        {
            case PATHING_TYPE.CHASE:
                if (detectionInfo.detectedEntities.Count > 0)
                {
                    return detectionInfo.detectedEntities[0].detectedEntityGameObject.transform.position;
                }
                break;

            case PATHING_TYPE.PATROL:
                return ReturnNextPatrolPoint(patrolPointIndex, patrolPoints);

            case PATHING_TYPE.RETURN:
                return homePoint;
        }

        return Vector3.zero;
    }

    public virtual Vector3 ReturnNextPatrolPoint(int pointIndex, Vector3[] patrolPoints)
    {
        Vector3 currentPatrolPoint = patrolPoints[pointIndex];

        if (CheckIfReachedTargetPoint(currentPatrolPoint, 1f))
        {
            if (++pointIndex > patrolPoints.Length - 1)
            {
                patrolPointIndex = 0;
            }
            else
            {
                patrolPointIndex++;

                return patrolPoints[patrolPointIndex];
            }
        }

        return currentPatrolPoint;
    }

    public virtual Vector3[] ReturnGlobalWaypoints (Vector3[] localWaypoints)
    {
        Vector3[] newGlobalWaypoints = new Vector3[localWaypoints.Length];

        for (int i = 0; i < localWaypoints.Length; i++)
        {
            newGlobalWaypoints[i] = transform.TransformPoint(localPatrolPoints[i]);
        }

        return newGlobalWaypoints;
    }

    public void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            for (int i = 0; i < localPatrolPoints.Length; i++)
            {
                Gizmos.color = Color.green;

                Vector3 localToWorldPatrolPoint = transform.TransformPoint(localPatrolPoints[i]);

                Gizmos.DrawWireSphere(localToWorldPatrolPoint, 0.25f);
            }
        }       
    }
}

[System.Serializable]
public struct EntityPathingAttributes
{
    [Header("Entity Pathing Attributes")]
    public float entityMinPathDistance;
    public float entityMaxPathDistance;

    [Space(10)]
    [Range(1, 100)]
    public int entityPathingFrequency;
}

[System.Serializable]
public struct EntityPathingSpeedAttributes
{
    [Header("Entity Pathing Speed Attributes")]
    public float entityPathingChaseSpeed;
    public float entityPathingPatrolSpeed;
    public float entityPathingReturnSpeed;
}

[System.Serializable]
public struct PathingPoint
{
    [Header("Pathing Point Attributes")]
    public Vector3 pathingPoint;

    [Space(10)]
    public Vector3 pathingDirection;

    [Space(10)]
    public int pathingIndex;

    public PathingPoint(Vector3 newPathingPoint, Vector3 newPathingDirection, int newPathingIndex)
    {
        this.pathingPoint = newPathingPoint;
        this.pathingDirection = newPathingDirection;
        this.pathingIndex = newPathingIndex;
    }
}

[System.Serializable]
public enum PATHING_TYPE
{
    NONE,
    CHASE,
    RETURN,
    PATROL
}
