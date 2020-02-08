using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZombieDashAttackCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        if (GetAttackObjectDistance() <= m_Character.m_Stat.Range)
        {
            //   Debug.Log("AttackCondSuccess");
            return NODE_STATE.SUCCESS;
        }

        //  Debug.Log("AttackCondFail");
        return NODE_STATE.FAIL;
    }
}

public class ZombieDashAttackAction : ActionNode
{
    Vector3 m_targetPos;

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

        if (m_Character.m_zombieState != ZOMBIE_STATE.ATTACK) //대쉬 방향지정
        {
            m_targetPos = GetAttackObject().transform.position;
            m_Character.gameObject.transform.LookAt(m_targetPos, Vector3.up);

            m_Character.m_Animator.CrossFade("Attack", 0.1f); //->대쉬로수정해야함
            m_Character.m_zombieState = ZOMBIE_STATE.ATTACK;
            return NODE_STATE.RUNNING;
        }
        else //개돌
        {
            if (m_nowActionTime < m_totalActionTime) //조건을 벽에 박을때까지로 바꿔야함
            {
                return NODE_STATE.RUNNING;
            }
            else //벽에 박으면
            {
                m_Character.m_zombieState = ZOMBIE_STATE.NONE;
                return NODE_STATE.FAIL;
            }
        }
    }
}