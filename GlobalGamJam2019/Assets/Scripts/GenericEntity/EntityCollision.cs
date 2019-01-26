using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EntityCollision : MonoBehaviour
{
    [Header("Entity Collision Base Attributes")]
    public EntityCollisionAttributes entityCollisionAttributes;

    [Header("Entity Collision Values")]
    public Collider entityCollider;

    [Space(10)]
    public EntityCollisionInfo entityCollisionInfo;

    [Space(10)]
    public ObjectGroundingBounds entityGroundingBounds;

    public virtual void Start()
    {
        InitializeCollisionValues();
    }

    public virtual void InitializeCollisionValues()
    {
        entityCollider = GetComponent<Collider>();

        entityGroundingBounds = new ObjectGroundingBounds();

        ReturnEntityColliderDimensions();
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(entityCollisionInfo.baseGroundData.groundPoint, 0.25f);
    }

    public virtual void ProcessCollisions()
    {
        UpdateObjectGroundingBounds();
        CollisionGroundCheck();
    }

    public virtual void UpdateObjectGroundingBounds()
    {
        entityGroundingBounds.UpdateGroundingBounds(entityCollisionAttributes.entityCollisionBounds, transform, entityCollisionAttributes.entityCollisionSkinWidth);      
    }

    public virtual void CollisionGroundCheck()
    {
        RaycastHit sphereHit = new RaycastHit();

        if (Physics.SphereCast(transform.position, entityCollisionAttributes.entitySpherePointRadius, -transform.up, out sphereHit, entityCollisionAttributes.entitySphereCollisionRayLength, entityCollisionAttributes.entityGroundCollisionMask))
        {
            entityCollisionInfo.groundInfo = new GroundInfo(entityGroundingBounds.objectGroundingBounds.Length);

            entityCollisionInfo.baseGroundData = new GroundData(sphereHit.point, true, sphereHit.distance, sphereHit.normal, ReturnSlopeAngle(sphereHit.normal, Vector3.up), ReturnSlopeCheck(sphereHit.normal));

            for (int i = 0; i < entityGroundingBounds.objectGroundingBounds.Length; i++)
            {
                RaycastHit rayHit = new RaycastHit();

                Debugger.DrawMultipleCustomDebugRayMultipleStarts(entityGroundingBounds.objectGroundingBounds, -transform.up * entityCollisionAttributes.entityCollisionRayLength, Color.yellow);

                if (Physics.Raycast(entityGroundingBounds.objectGroundingBounds[i], -transform.up, out rayHit, entityCollisionAttributes.entityCollisionRayLength, entityCollisionAttributes.entityGroundCollisionMask))
                {
                    Debugger.DrawCustomDebugRay(transform.position, -transform.up * entityCollisionAttributes.entityCollisionRayLength, Color.green);

                    entityCollisionInfo.groundInfo.groundData[i] = new GroundData(rayHit.point, true, rayHit.distance, rayHit.normal, ReturnSlopeAngle(rayHit.normal, Vector3.up), ReturnSlopeCheck(rayHit.normal));
                }
                else
                {
                    entityCollisionInfo.groundInfo.groundData[i] = new GroundData(sphereHit.point, true, sphereHit.distance, sphereHit.normal, ReturnSlopeAngle(sphereHit.normal, Vector3.up), false);
                }
            }          
        }
        else
        {
            entityCollisionInfo.groundInfo = new GroundInfo(0);

            entityCollisionInfo.baseGroundData = new GroundData(Vector3.zero, false, 0f, Vector3.up, 90f, false);
        }

        entityCollisionInfo.isGrounded = entityCollisionInfo.baseGroundData.isGrounded;
        entityCollisionInfo.isOnSlope = entityCollisionInfo.groundInfo.ReturnSloped();

        entityCollisionInfo.entityGroundForward = ReturnEntityForward();
        entityCollisionInfo.groundInfo.SetAverageGroundNormal();
    }

    public EntityCollisionInfo ReturnCollisionInfo()
    {
        ProcessCollisions();

        return entityCollisionInfo;
    }

    public virtual Vector3 ReturnEntityForward()
    {
        return Vector3.Cross(transform.right, entityCollisionInfo.groundInfo.ReturnAverageSlopeNormal());
    }

    public virtual void ReturnEntityColliderDimensions()
    {
        entityCollisionAttributes.entityCollisionBounds = entityCollider.bounds;        
    }

    public bool ReturnSlopeCheck(Vector3 slopeNormal)
    {
        if (slopeNormal != Vector3.up)
        {
            return true;
        }

        return false;
    }

    public float ReturnSlopeAngle(Vector3 slopeNormal, Vector3 forward)
    {
        return Vector3.Angle(slopeNormal, forward);
    }
}

[System.Serializable]
public struct EntityCollisionAttributes
{
    [Header("Collision Bounds")]
    public Bounds entityCollisionBounds;

    [Header("Object Skin Width")]
    public float entityCollisionSkinWidth;

    [Header("Object Collision Sphere Check Radius")]
    public float entitySpherePointRadius;

    [Header("Object Collision Ray Length")]
    public float entitySphereCollisionRayLength;
    public float entityCollisionRayLength;

    [Header("Collision Layers")]
    public LayerMask entityCollisionMask;

    [Space(10)]
    public LayerMask entityGroundCollisionMask;
}

[System.Serializable]
public struct EntityCollisionInfo
{
    [Header("Entity Direction")]
    public Vector3 entityGroundForward;

    [Header("Grounded")]
    public bool isGrounded;

    [Space(10)]
    public bool isOnSlope;

    [Header("Basic Grounded Attributes")]
    public GroundData baseGroundData;

    [Header("Detailed Grounding Attributes")]
    public GroundInfo groundInfo;
}

[System.Serializable]
public struct ObjectGroundingBounds
{
    public Vector3[] objectGroundingBounds;

    public void UpdateGroundingBounds(Bounds objectBounds, Transform objectTransform, float objectSkinWidth)
    {
        Vector3 objectCenter = objectBounds.center;
        Vector3 objectSize = objectBounds.size;

        objectGroundingBounds = new Vector3[2];

        Vector3 xAxis = objectTransform.right * objectBounds.size.x / 2;
        Vector3 yAxis = -objectTransform.up * (objectSkinWidth + objectBounds.size.y / 2);
        Vector3 zAxis = objectTransform.forward * objectBounds.size.z / 2;

        objectGroundingBounds[0] = new Vector3(0, -objectBounds.min.y + objectSkinWidth, 0) + zAxis;
        objectGroundingBounds[1] = new Vector3(0, -objectBounds.min.y + objectSkinWidth, 0) - zAxis;

        for (int i = 0; i < objectGroundingBounds.Length; i++)
        {
            objectGroundingBounds[i] += objectTransform.position;
        }
    }
}

[System.Serializable]
public struct GroundInfo
{
    [Header("Base Ground Info")]
    public GroundData[] groundData;

    [Header("Average Ground Normal")]
    public Vector3 averageGroundNormal;

    public GroundInfo(int groundDataSetSize)
    {
        this.groundData = new GroundData[groundDataSetSize];
        this.averageGroundNormal = Vector3.up;
    }

    public void SetAverageGroundNormal()
    {
        this.averageGroundNormal = ReturnAverageSlopeNormal();
    }

    public bool ReturnGrounded()
    {
        for (int i = 0; i < groundData.Length; i++)
        {
            if (groundData[i].isGrounded)
            {
                return true;
            }
        }

        return false;
    }

    public bool ReturnSloped()
    {
        for (int i = 0; i < groundData.Length; i++)
        {
            if (groundData[i].isSloped)
            {
                return true;
            }
        }

        return false;
    }

    public void SetSlopeValues(Vector3 slopeNormal, float slopeAngle)
    {
        for (int i = 0; i < groundData.Length; i++)
        {
            groundData[i].groundNormal = slopeNormal;
            groundData[i].groundAngle = slopeAngle;
        }
    }

    public Vector3 ReturnSharpestSlope()
    {
        Vector3 sharpestSlope = Vector3.zero;

        for (int i = 0; i < groundData.Length; i++)
        {
            if (sharpestSlope.y < groundData[i].groundNormal.y)
            {
                sharpestSlope = groundData[i].groundNormal;
            }
        }

        return sharpestSlope;
    }

    public Vector3 ReturnSmallestSlope()
    {
        Vector3 smallestSlope = Vector3.one;

        for (int i = 0; i < groundData.Length; i++)
        {
            if (smallestSlope.y > groundData[i].groundNormal.y)
            {
                smallestSlope = groundData[i].groundNormal;
            }
        }

        return smallestSlope;
    }

    public Vector3 ReturnAverageSlopeNormal()
    {
        Vector3 averageSlopeNormal = Vector3.zero;
        int dataCount = 0;

        for (int i = 0; i < groundData.Length; i++)
        {
            if (groundData[i].isSloped)
            {
                averageSlopeNormal += groundData[i].groundNormal;
                dataCount++;
            }
        }

        if (dataCount > 0)
        {
            averageSlopeNormal /= dataCount;

            return averageSlopeNormal;
        }

        return Vector3.up;
    }

    public float ReturnSmallestSlopeAngle()
    {
        float smallestSlopeAngle = Mathf.Infinity;

        for (int i = 0; i < groundData.Length; i++)
        {
            if (smallestSlopeAngle > groundData[i].groundAngle)
            {
                smallestSlopeAngle = groundData[i].groundAngle;
            }
        }

        return smallestSlopeAngle;
    }

    public float ReturnSharpestSlopeAngle()
    {
        float sharpestSlopeAngle = 0f;

        for (int i = 0; i < groundData.Length; i++)
        {
            if (sharpestSlopeAngle < groundData[i].groundAngle)
            {
                sharpestSlopeAngle = groundData[i].groundAngle;
            }
        }

        return sharpestSlopeAngle;
    }

    public float ReturnAverageSlopeAngle()
    {
        float averageSlopeAngle = 0f;
        int dataCount = 0;

        for (int i = 0; i < groundData.Length; i++)
        {
            if (groundData[i].isSloped)
            {
                averageSlopeAngle += groundData[i].groundAngle;
                dataCount++;
            }
        }

        if (dataCount > 0)
        {
            averageSlopeAngle /= dataCount;

            return averageSlopeAngle;
        }

        return 90f;
    }

    public float ReturnAverageGroundHeight()
    {
        float averageGroundHeight = 0;
        int dataCount = 0;

        if (groundData.Length == 0)
        {
            return 0;
        }

        for (int i = 0; i < groundData.Length; i++)
        {
            if (groundData[i].isSloped)
            {
                averageGroundHeight += groundData[i].groundPoint.y;
                dataCount++;
            }
        }

        if (dataCount > 0)
        {
            averageGroundHeight /= dataCount;

            return averageGroundHeight;
        }

        return 0;
    }
}

[System.Serializable]
public struct GroundData
{
    public Vector3 groundPoint;
    public bool isGrounded;

    public float groundHitDistance;

    public Vector3 groundNormal;
    public float groundAngle;
    public bool isSloped;

    public GroundData(Vector3 newGroundPoint, bool newIsGrounded, float newGroundHitDistance, Vector3 newSlopeNormal, float newSlopeAngle, bool newIsSloped)
    {
        this.groundPoint = newGroundPoint;
        this.isGrounded = newIsGrounded;
        this.groundHitDistance = newGroundHitDistance;
        this.groundNormal = newSlopeNormal;
        this.groundAngle = newSlopeAngle;
        this.isSloped = newIsSloped;
    }
}
