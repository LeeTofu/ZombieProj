using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWalkCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        float distance = GetAttackObjectDistance();

        if (distance < m_Character.m_Stat.alertRange && distance > m_Character.m_Stat.Range && m_Character.m_Stat.MoveSpeed < 1.01f)
        {
            return NODE_STATE.SUCCESS;
        }

        return NODE_STATE.FAIL;
    }
}

public class ZombieWalkAction : ActionNode
{
    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;

        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Zombie_Walk_F_1_Full_Loop_IPC")
                m_totalActionTime = ac.animationClips[i].length;


    }

    public override NODE_STATE Tick()
    {
        //플레이 부분
        if (m_Character.m_zombieState != ZOMBIE_STATE.WALK)
        {
            m_Character.m_Animator.CrossFade("Walk", 0.1f);
            m_Character.m_Animator.SetFloat("WalkSpeed", m_Character.m_Stat.MoveSpeed * 1.5f);
            m_Character.m_zombieState = ZOMBIE_STATE.WALK;
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