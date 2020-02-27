﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashZombieCollisionAction : CollisionAction
{
    protected override bool CollisionCondition(GameObject _defender)
    {
        if (_defender.tag != "Wall" && _defender.tag != "Bullet" && _defender.tag != "Player") return false;

        return true;
    }

    protected override void CollisionEvent(GameObject _object)
    {
        if ((_object.tag == "Wall" || _object.tag == "Bullet") && !m_Character.m_Stat.isStunned && m_Character.m_zombieState == ZOMBIE_STATE.ATTACK)
        {
            m_Character.m_Stat.isStunned = true;
            m_Character.m_NavAgent.isStopped = true;
        }

        if((_object.tag == "Player"))
        {
            PlayerManager.Instance.AttackToPlayer(m_Character.m_Stat.Attack, true);
        }

        m_Character.m_Stat.alertRange = 1000.0f;
        m_Character.m_Stat.MoveSpeed = 2.5f;
    }
}
