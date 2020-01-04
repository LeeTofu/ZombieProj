using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieKnockBackCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        if (m_Character.m_Stat.isKnockBack)
        {
            Debug.Log("KnockBack Success");
            return NODE_STATE.SUCCESS;
        }

        return NODE_STATE.FAIL;
    }
}

public class ZombieKnockBackAction : ActionNode
{
    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;

        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Zombie_Walk_F_1_KnockBack_Walk_IPC")
                m_totalActionTime = ac.animationClips[i].length;
    }

    public override NODE_STATE Tick()
    {
        //플레이 부분

        if (m_Character.m_zombieState != ZOMBIE_STATE.KNOCK_BACK)
        {
            m_Character.m_Animator.CrossFade("KnockBack", 0.1f);
            m_Character.m_zombieState = ZOMBIE_STATE.KNOCK_BACK;

            return NODE_STATE.RUNNING;
        }
        else
        {
            m_nowActionTime += Time.deltaTime;

            if (m_nowActionTime < m_totalActionTime)
            {
                return NODE_STATE.RUNNING;
            }
            else
            {
                m_Character.m_Stat.isKnockBack = false;
                m_Character.m_zombieState = ZOMBIE_STATE.NONE;
                m_nowActionTime = 0f;

                return NODE_STATE.FAIL;
            }
        }
 
        m_Character.m_zombieState = ZOMBIE_STATE.NONE;
        m_nowActionTime = 0f;

        return NODE_STATE.FAIL;
    }
}