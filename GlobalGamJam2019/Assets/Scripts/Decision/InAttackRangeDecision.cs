using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decision/InAttackRangeDecision")]
public class InAttackRangeDecision : EntityDecision
{
    [Header("Custom Decision Attributes")]
    public float minAttackRange = 3f;

    public override bool Decide(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        return CheckIfInAttackRange(controller, stateController, stateInfo);
    }

    public bool CheckIfInAttackRange(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        if (stateInfo.detectionInfo.detectedEntities.Count > 0)
        {
            if (stateInfo.detectionInfo.closestEntityDistance <= minAttackRange)
            {
                return true;
            }
        }

        return false;
    }
}
