﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZombieRangeAttackCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        if (GetAttackObjectDistance() <= m_Character.m_Stat.Range && !m_Character.m_Stat.isStunned)
        {
            return NODE_STATE.SUCCESS;
        }

        return NODE_STATE.FAIL;
    }
}

public class ZombieRangeAttackAction : ActionNode
{
    bool m_isAttacked;

    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;

        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Zombie_Atk_Arms_4A_SHORT_Loop_IPC")
                m_totalActionTime = ac.animationClips[i].length;

        m_isAttacked = false;
    }

    public override NODE_STATE Tick()
    {
        //플레이 부분

        if (m_Character.m_zombieState != ZOMBIE_STATE.ATTACK)
        {
            m_Character.m_Animator.SetFloat("AttackSpeed", m_Character.m_Stat.AttackSpeed);
            m_Character.m_Animator.CrossFade("RangeAttack", 0.1f);
            m_Character.m_zombieState = ZOMBIE_STATE.ATTACK;
            return NODE_STATE.RUNNING;
        }
        else
        {
            MovingObject mobject = GetAttackObject();

            m_nowActionTime += Time.deltaTime;
            m_Character.gameObject.transform.LookAt(mobject.transform.position, Vector3.up);
            if (m_nowActionTime < m_totalActionTime)
            {
                if(m_nowActionTime / m_totalActionTime > 0.39)
                {
                    if(!m_isAttacked)
                    {
                        //발싸함수
                        m_isAttacked = true;
                    }

                }
                return NODE_STATE.RUNNING;
            }
        }

        m_isAttacked = false;
        m_Character.m_zombieState = ZOMBIE_STATE.NONE;
        m_nowActionTime = 0f;
        return NODE_STATE.FAIL;
    }
}