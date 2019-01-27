using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityObstacle : MonoBehaviour
{
   [Header("Entity Obstacle Attributes")]
   public ObstacleAttributes obstacleAttributes;
   
   [Space(10)]
   public List<Collider2D> hitEntities = new List<Collider2D>();

   private void Update()
   {
      CheckForCollisions();
   }

   private void CheckForCollisions()
   {
      Collider2D[] hitColliders = new Collider2D[0];
      
      if (obstacleAttributes.colliderType == COLLIDER_TYPE.BOX)
      {
         hitColliders = Physics2D.OverlapBoxAll(transform.position, obstacleAttributes.colliderSize,
            obstacleAttributes.colliderRotationAngle, obstacleAttributes.detectionMask);
      }
      else if (obstacleAttributes.colliderType == COLLIDER_TYPE.SPHERE)
      {
         hitColliders = Physics2D.OverlapCircleAll(transform.position, obstacleAttributes.colliderRadius,
            obstacleAttributes.detectionMask);
      }

      for (int i = 0; i < hitColliders.Length; i++)
      {         
         if (hitEntities.Contains(hitColliders[i]))
         {
            continue;
         }
         
         hitColliders[i].GetComponent<EntityMovement>().DisableMovement();
         
         hitEntities.Add(hitColliders[i]);
      }
   }

   public void Reset()
   {
      hitEntities.Clear();
   }

   private void OnDrawGizmos()
   {
      Gizmos.color = Color.red;
      
      if (obstacleAttributes.colliderType == COLLIDER_TYPE.BOX)
      {
         if (obstacleAttributes.colliderSize != Vector2.zero)
         {
            Gizmos.DrawWireCube(transform.position, obstacleAttributes.colliderSize);
         }
      }
      else if (obstacleAttributes.colliderType == COLLIDER_TYPE.SPHERE)
      {
         if (obstacleAttributes.colliderRadius != 0)
         {
            Gizmos.DrawWireSphere(transform.position, obstacleAttributes.colliderRadius);
         }
      }
   }
}

[System.Serializable]
public struct ObstacleAttributes
{
   [Header("Obstacle Attributes")] 
   public COLLIDER_TYPE colliderType;

   [Space(10)]
   public float colliderRadius;

   [Space(10)] public Vector2 colliderSize;

   [Space(10)] public float colliderRotationAngle;

   [Space(10)] public LayerMask detectionMask;
}

public enum COLLIDER_TYPE
{
   NONE,
   BOX,
   SPHERE
}
