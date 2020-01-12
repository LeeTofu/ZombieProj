using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCollisionAction : CollisionAction
{
    protected override void CollisionEvent(GameObject _object)
    {
        return;
    }

    protected override bool CollisionCondition(GameObject _defender)
    {
        return true;
    }
}
