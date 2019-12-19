using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWalkCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        float distance = Vector3.Distance(m_Character.gameObject.transform.position,
            PlayerManager.Instance.m_Player.gameObject.transform.position);

        if (distance < 10f && distance > 1.5f)
        {
            Debug.Log("WalkCondSuccess");
            return NODE_STATE.SUCCESS;
        }

        Debug.Log("WalkCondFail");
        return NODE_STATE.FAIL;
    }
}

public class ZombieWalkAction : ActionNode
{
    public override NODE_STATE Tick()
    {
        if (m_totalActionTime == 0f)
        {
            AnimatorStateInfo animationState = m_Character.m_Animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] myAnimatorClip = m_Character.m_Animator.GetCurrentAnimatorClipInfo(0);
            m_totalActionTime = myAnimatorClip[0].clip.length;
        }

        //플레이 부분
        if (!m_Character.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            m_Character.m_Animator.Play("Walk");
            Debug.Log("WalkStart");
            return NODE_STATE.RUNNING;
        }
        else
        {
            m_nowActionTime += Time.deltaTime;
            if(m_nowActionTime < m_totalActionTime)
            {
                Debug.Log("Walking");
                return NODE_STATE.RUNNING;
            }
        }

        m_nowActionTime = 0f;
        Debug.Log("Walk out");
        return NODE_STATE.FAIL;
    }
}