using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZombieStunCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        if (m_Character.m_Stat.isStunned)
            return NODE_STATE.SUCCESS;

        return NODE_STATE.FAIL;
    }
}

public class ZombieStunAction : ActionNode
{
    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;

        m_totalActionTime = 5f; //totalactionTime을 좀비 스턴시간으로 사용함
    }

    public override NODE_STATE Tick()
    {
        //플레이 부분 -> 공격 시작 전 인식한 플레이어의 방향으로 대쉬어택(레포데 차져처럼)

        if (m_Character.m_zombieState != ZOMBIE_STATE.STUN)
        {
            if (m_Character.m_NavAgent != null)
                m_Character.m_NavAgent.isStopped = true;
            m_Character.m_Animator.CrossFade("Stun", 0.1f);
            m_Character.m_zombieState = ZOMBIE_STATE.STUN;
            return NODE_STATE.RUNNING;
        }
        else
        {
            if (m_nowActionTime < m_totalActionTime) //스턴시간중이냐
            {
                m_nowActionTime += Time.deltaTime;
                return NODE_STATE.RUNNING;
            }
            else //스턴끝났냐
            {
                m_nowActionTime = 0f;
                m_Character.m_zombieState = ZOMBIE_STATE.NONE;
                m_Character.m_Stat.isStunned = false;
                return NODE_STATE.FAIL;
            }
        }
    }
}