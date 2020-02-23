using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DashZombieAttackCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        if ((GetAttackObjectDistance() <= m_Character.m_Stat.Range) &&
            !m_Character.m_Stat.isStunned ||
            (m_Character.m_zombieState == ZOMBIE_STATE.ATTACK))
        {
            return NODE_STATE.SUCCESS;
        }

        return NODE_STATE.FAIL;
    }
}

public class DashZombieAttackAction : ActionNode
{
    Vector3 m_originForward;
    Vector3 m_targetForward;
    float t;

    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;
        m_totalActionTime = 2f; //플레이어 방향으로까지의 준비시간(초)

       /*//혹시몰라 남겨두는 action info
        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Zombie_HyperChase_2_Loop_IPC")
                m_totalActionTime = ac.animationClips[i].length;
        */
    }

    public override NODE_STATE Tick()
    {
        if (m_Character.m_zombieState != ZOMBIE_STATE.ATTACK) //대쉬 방향지정
        {
            m_originForward = m_Character.transform.forward;
            m_targetForward = (GetAttackObject().transform.position - m_Character.transform.position).normalized;
            t = 0f;
            m_Character.m_Animator.CrossFade("Idle1", 0.1f);

            m_Character.m_zombieState = ZOMBIE_STATE.ATTACK;
            return NODE_STATE.RUNNING;
        }
        else 
        {
            if(t < 2f) //돌진 전 플레이어 방향으로까지의 회전
            {
                t += Time.deltaTime;
                if (t >= m_totalActionTime)
                {
                    t = m_totalActionTime;
                    m_Character.m_Animator.CrossFade("DashAttack", 0.1f);
                    m_Character.m_Animator.SetFloat("AttackSpeed", m_Character.m_Stat.AttackSpeed / 2f);
                }
                m_Character.transform.forward = Vector3.Slerp(m_originForward, m_targetForward, t / m_totalActionTime);
                return NODE_STATE.RUNNING;
            }
            else
            {
                m_Character.transform.position += 
                    m_Character.transform.forward * m_Character.m_Stat.AttackSpeed * Time.deltaTime * 2f;
                return NODE_STATE.RUNNING;
            }
        }
    }
}