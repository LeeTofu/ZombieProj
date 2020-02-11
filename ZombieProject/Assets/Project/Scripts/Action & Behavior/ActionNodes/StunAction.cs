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

        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Zombie_Atk_Arms_3_Loop_IPC")
                m_totalActionTime = ac.animationClips[i].length;
    }

    public override NODE_STATE Tick()
    {
        //플레이 부분 -> 공격 시작 전 인식한 플레이어의 방향으로 대쉬어택(레포데 차져처럼)

        if (m_Character.m_zombieState != ZOMBIE_STATE.STUN)
        {
            m_Character.m_Animator.CrossFade("Attack", 0.1f); //->스턴으로수정해야함
            m_Character.m_zombieState = ZOMBIE_STATE.STUN; //이것도
            return NODE_STATE.RUNNING;
        }
        else //개돌
        {
            if (m_nowActionTime < m_totalActionTime) //스턴시간중이냐
            {
                return NODE_STATE.RUNNING;
            }
            else //스턴끝났냐
            {
                m_Character.m_zombieState = ZOMBIE_STATE.NONE;
                m_Character.m_Stat.isStunned = false;
                return NODE_STATE.FAIL;
            }
        }
    }
}