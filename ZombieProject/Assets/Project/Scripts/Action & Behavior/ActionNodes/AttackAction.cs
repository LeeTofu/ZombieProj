using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZombieAttackCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        float distance = Vector3.Distance(m_Character.gameObject.transform.position,
            PlayerManager.Instance.m_Player.gameObject.transform.position);

        if (distance <= 1.4f)
        {
            Debug.Log("AttackCondSuccess");
            return NODE_STATE.SUCCESS;
        }

        Debug.Log("AttackCondFail");
        return NODE_STATE.FAIL;
    }
}

public class ZombieAttackAction : ActionNode
{
    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;

        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Attack")
                m_totalActionTime = ac.animationClips[i].length;
    }

    public override NODE_STATE Tick()
    {
        //플레이 부분
        if (!m_Character.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            m_Character.m_Animator.Play("Attack");
            Debug.Log("AttackStart");
            return NODE_STATE.RUNNING;
        }
        else
        {
            m_nowActionTime += Time.deltaTime;
            if (m_nowActionTime < m_totalActionTime)
            {
                Debug.Log("Attacking");
                return NODE_STATE.RUNNING;
            }
        }

        m_nowActionTime = 0f;
        Debug.Log("Attack out");
        return NODE_STATE.FAIL;
    }
}