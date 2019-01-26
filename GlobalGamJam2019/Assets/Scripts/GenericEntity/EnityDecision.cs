using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityDecision : ScriptableObject
{
    public abstract bool Decide(EntityController controller, EntityStateController stateController, EntityStateInfo stateInfo);
}
