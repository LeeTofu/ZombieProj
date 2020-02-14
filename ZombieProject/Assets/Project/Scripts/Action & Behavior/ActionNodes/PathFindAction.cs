using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePathFindCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        //생성 이후부터 플레이어와의 거리가 주어진 값 이내가 될 때까지 쫓아감
        float distance = GetAttackObjectDistance();

        if (distance < m_Character.m_Stat.alertRange && distance > m_Character.m_Stat.Range || m_Character.m_zombieState != ZOMBIE_STATE.PATHFIND && !m_Character.m_Stat.isStunned)
        {
            MovingObject mobject = GetAttackObject();

            m_Character.m_NavAgent.destination = mobject.transform.position;
            m_Character.m_NavAgent.isStopped = false;

            return NODE_STATE.SUCCESS;
        }

        return NODE_STATE.FAIL;
    }
}

public class ZombiePathFindAction : ActionNode
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
        if(GetAttackObjectDistance() <= m_Character.m_Stat.Range) //길찾기 탈출조건
        {
            m_Character.m_NavAgent.isStopped = true;

            return NODE_STATE.FAIL;
        }
        else
        {
            MovingObject mobject = GetAttackObject();

            m_Character.m_NavAgent.destination = mobject.transform.position;

            if (m_Character.m_zombieState != ZOMBIE_STATE.PATHFIND)
            {
                m_Character.m_Animator.CrossFade("Walk", 0.1f);
                m_Character.m_zombieState = ZOMBIE_STATE.PATHFIND;
            }

            return NODE_STATE.RUNNING;
        }
    }
}