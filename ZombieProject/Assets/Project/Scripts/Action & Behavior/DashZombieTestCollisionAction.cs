using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashZombieTestCollisionAction : CollisionAction
{
    protected override void CollisionEvent(GameObject _object)
    {
        m_Character.m_Stat.alertRange = 1000.0f;
        m_Character.m_Stat.MoveSpeed = 2.5f;


        if(_object.gameObject.layer == LayerMask.NameToLayer("Wall") && !m_Character.m_Stat.isStunned)
        {
            m_Character.m_Stat.isStunned = true;
        }
    }

    protected override bool CollisionCondition(GameObject _defender)
    {
        if (_defender.gameObject.layer != LayerMask.NameToLayer("Bullet") ) return false;

        return true;
    }
}
