using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Decision/CheckIfNavigationPathIsValidDecision")]
public class CheckIfNavigationPathIsValidDecision : EntityDecision
{
    public override bool Decide(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        return CheckIfPathIsValid(controller, stateController, stateInfo);
    }

    private bool CheckIfPathIsValid(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo)
    {
        return controller.navigation.CheckIfPathIsValid();
    }
}
