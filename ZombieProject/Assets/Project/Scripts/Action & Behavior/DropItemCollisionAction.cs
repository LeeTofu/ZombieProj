using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemCollisionAction : CollisionAction
{
    protected override void CollisionEvent(GameObject _object)
    {
       (m_Character as DropItem).ApplyBuff(_object.GetComponent<MovingObject>());
    }

    protected override bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (_defender.tag != "Player") return false;

        return true;
    }
}
