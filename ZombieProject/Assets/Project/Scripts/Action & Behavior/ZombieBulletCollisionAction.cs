using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBulletCollisionAction : CollisionAction
{
    protected override bool CollisionCondition(GameObject _defender)
    {
        if (_defender.tag != "Wall" && _defender.tag != "Player") return false;

        return true;
    }

    protected override void CollisionEvent(GameObject _defender)
    {
        (m_Character as Bullet).CollisionEvent(_defender);
    }
}
