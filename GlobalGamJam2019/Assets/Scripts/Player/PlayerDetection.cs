using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : EntityDetection
{
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, entityDetectionAttributes.entityDetectionRadius);
    }
}
