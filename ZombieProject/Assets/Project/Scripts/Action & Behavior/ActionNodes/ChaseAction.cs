using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChaseCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        float distance = GetAttackObjectDistance();

        if (distance < m_Character.m_Stat.alertRange && distance > m_Character.m_Stat.Range && m_Character.m_Stat.MoveSpeed >= 0.8f)
        {
            return NODE_STATE.SUCCESS;
        }

        return NODE_STATE.FAIL;
    }
}

public class ZombieChaseAction : ActionNode
{
    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;

        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Zombie_Chase_1_Full_Loop_IPC")
                m_totalActionTime = ac.animationClips[i].length;
    }

    public override NODE_STATE Tick()
    {
        //플레이 부분

        if (m_Character.m_zombieState != ZOMBIE_STATE.CHASE)
        {
            m_Character.m_Animator.CrossFade("Chase", 0.1f);
            m_Character.m_zombieState = ZOMBIE_STATE.CHASE;
            return NODE_STATE.RUNNING;
        }
        else
        {
            MovingObject mobject = GetAttackObject();

            m_nowActionTime += Time.deltaTime;
            m_Character.gameObject.transform.LookAt(mobject.transform.position, Vector3.up);
            m_Character.gameObject.transform.position += m_Character.transform.forward * m_Character.m_Stat.MoveSpeed * Time.deltaTime;
            if (m_nowActionTime < m_totalActionTime)
            {
                return NODE_STATE.RUNNING;
            }
        }

        m_Character.m_zombieState = ZOMBIE_STATE.NONE;
        m_nowActionTime = 0f;
        return NODE_STATE.FAIL;
    }
}