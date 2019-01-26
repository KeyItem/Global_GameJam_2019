using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityTransition
{
    public EntityDecision decision;

    [Space(10)]
    public EntityState trueState;

    [Space(10)]
    public EntityState falseState;
}
