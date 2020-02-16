using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZombieAttackCondition : DecoratorNode
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

public class ZombieAttackAction : ActionNode
{
    bool m_CanMeleeAttack = false;


    void StartMeleeAttack()
    {
        (m_Character as Zombie).PlayAttackSound();

        if ((PlayerManager.Instance.m_Player.transform.position - m_Character.transform.position).sqrMagnitude < m_Character.m_Stat.Range * m_Character.m_Stat.Range)
        {
            PlayerManager.Instance.AttackToPlayer(m_Character.m_Stat.Attack, m_Character.m_Stat.isKnockBack);
        }
    }


    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;
        m_CanMeleeAttack = false;

        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Zombie_Atk_Arm_1_R_SHORT_Loop_IPC")
                m_totalActionTime = ac.animationClips[i].length;
    }

    public override NODE_STATE Tick()
    {
        //플레이 부분

        if (m_Character.m_zombieState != ZOMBIE_STATE.ATTACK)
        {

            
            m_CanMeleeAttack = false;
            m_Character.m_Animator.CrossFade("Attack" + Random.Range(0,2), 0.1f);

            m_Character.m_zombieState = ZOMBIE_STATE.ATTACK;
            m_nowActionTime = 0.0f;

            return NODE_STATE.RUNNING;
        }
        else
        {
            MovingObject mobject = GetAttackObject();

            if (m_nowActionTime >= m_totalActionTime * 0.25f && !m_CanMeleeAttack)
            {
                StartMeleeAttack();
                m_CanMeleeAttack = true;
            }

            m_nowActionTime += Time.deltaTime;
            m_Character.gameObject.transform.LookAt(mobject.transform.position, Vector3.up);

            if (m_nowActionTime < m_totalActionTime)
            {
                return NODE_STATE.RUNNING;
            }
        }
  
        m_CanMeleeAttack = false;
        m_Character.m_zombieState = ZOMBIE_STATE.NONE;
        m_nowActionTime = 0f;

        return NODE_STATE.FAIL;
    }
}