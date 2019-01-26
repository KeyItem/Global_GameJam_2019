using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decision/DetectEnemyDecision")]
public class DetectEnemyDecision : EntityDecision
{
    public override bool Decide(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        return SearchForEnemyEntities(controller, stateController, stateInfo);
    }

    private bool SearchForEnemyEntities(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        if (stateInfo.detectionInfo.detectedEntities.Count > 0)
        {
            return true;
        }

        return false;
    }
}
