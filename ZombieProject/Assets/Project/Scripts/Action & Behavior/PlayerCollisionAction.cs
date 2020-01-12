using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionAction : CollisionAction
{
    protected override bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (_defender.tag != "Bullet" && _defender.tag != "Zombie") return false;

        return true;
    }

    protected override void CollisionEvent(GameObject _object)
    {
        return;
    }
}
