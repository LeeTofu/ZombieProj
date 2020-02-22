using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCollisionAction : CollisionAction
{
    protected override void CollisionEvent(GameObject _object)
    {
        m_Character.m_Stat.alertRange = 1000.0f;
        m_Character.m_Stat.MoveSpeed = 2.5f;
        m_Character.m_NavAgent.speed = m_Character.m_Stat.MoveSpeed;
    }

    protected override bool CollisionCondition(GameObject _defender)
    {
        if (_defender.gameObject.layer != LayerMask.NameToLayer("Bullet") ) return false;

        return true;
    }
}
