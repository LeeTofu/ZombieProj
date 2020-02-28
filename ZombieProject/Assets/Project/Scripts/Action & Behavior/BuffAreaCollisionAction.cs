using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAreaCollisionAction : CollisionAction
{
    protected override void CollisionEvent(GameObject _object)
    {
       (m_Character as BuffArea).ApplyBuff(_object.GetComponent<MovingObject>());
    }

    protected override bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (!CheckTagCollision(_defender.tag)) return false;

        return true;
    }
}

public class DeBuffAreaCollisionAction : CollisionAction
{

    protected override void CollisionEvent(GameObject _object)
    {
        (m_Character as DeBuffArea).ApplyBuff(_object.GetComponent<MovingObject>());
    }

    protected override bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (!CheckTagCollision(_defender.tag)) return false;


        return true;
    }
}
