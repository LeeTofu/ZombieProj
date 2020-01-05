using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZombieDeadCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        if (m_Character.m_Stat.isDead)
        {
            return NODE_STATE.SUCCESS;
        }
        return NODE_STATE.FAIL;
    }

}
public class ZombieDeadAction : ActionNode
{
    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;
        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Zombie_Death_Forward_1_IPC")
                m_totalActionTime = ac.animationClips[i].length;
    }

    public override NODE_STATE Tick()
    {
        //플레이 부분
        if (m_Character.m_zombieState != ZOMBIE_STATE.DEAD)
        {
            m_Character.DeadAction();
            m_Character.m_Animator.CrossFade("Dead", 0.1f);
            m_Character.m_zombieState = ZOMBIE_STATE.DEAD;

            return NODE_STATE.SUCCESS;
        }
        else
        {
            m_nowActionTime += Time.deltaTime;
            if (m_nowActionTime < m_totalActionTime)
            {
                return NODE_STATE.SUCCESS;
            }
        }

        m_Character.m_zombieState = ZOMBIE_STATE.NONE;
        m_nowActionTime = 0f;

        m_Character.pushToMemory((int)m_Character.m_Type);

        return NODE_STATE.SUCCESS;
    }
}