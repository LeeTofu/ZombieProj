using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DashZombieAttackCondition : DecoratorNode
{
    Ray checkingRay;

    private bool CheckObstacle()    //좀비와 플레이어 사이에 장애물이 없는지 체크 없으면 True, 잇으면 false
    {
        RaycastHit hit;

        checkingRay.origin = m_Character.gameObject.transform.position;
        checkingRay.direction = (GetAttackObject().transform.position -
            m_Character.gameObject.transform.position).normalized;

        if (Physics.Raycast(checkingRay, out hit, m_Character.m_Stat.Range))
        {
            if (hit.transform.tag.Equals("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public override NODE_STATE Tick()
    {
        if (GetAttackObjectDistance() <= m_Character.m_Stat.Range && !m_Character.m_Stat.isStunned)
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

        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Zombie_HyperChase_2_Loop_IPC")
                m_totalActionTime = ac.animationClips[i].length;
    }

    public override NODE_STATE Tick()
    {
        if (m_Character.m_zombieState != ZOMBIE_STATE.ATTACK) //대쉬 방향지정
        {
            m_originForward = m_Character.transform.forward;
            m_targetForward = (GetAttackObject().transform.position - m_Character.transform.position).normalized;
            t = 0f;

            m_Character.m_zombieState = ZOMBIE_STATE.ATTACK;
            return NODE_STATE.RUNNING;
        }
        else 
        {
            if(t < 1f)
            {
                //돌진 전 플레이어 방향으로까지의 회전
                t += Time.deltaTime;
                if (t > 1f)
                {
                    t = 1f;
                    m_Character.m_Animator.SetFloat("AttackSpeed", m_Character.m_Stat.AttackSpeed);
                    m_Character.m_Animator.CrossFade("DashAttack", 0.1f);
                }
                m_Character.transform.forward = Vector3.Slerp(m_originForward, m_targetForward, t);
                return NODE_STATE.RUNNING;
            }
            else
            {
                if (!m_Character.m_Stat.isStunned) //개돌
                {
                    m_Character.transform.position +=
                        m_Character.transform.forward * m_Character.m_Stat.AttackSpeed * Time.deltaTime * 2f; // -> 수치조정 이것도
                    return NODE_STATE.RUNNING;
                }
                else //stun상태일 때
                {
                    m_Character.m_zombieState = ZOMBIE_STATE.NONE;
                    return NODE_STATE.FAIL;
                }
            }
        }
    }
}