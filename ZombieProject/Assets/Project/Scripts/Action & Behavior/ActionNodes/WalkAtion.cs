using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWalkCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        float distance = Vector3.Distance(m_Character.gameObject.transform.position,
            PlayerManager.Instance.m_Player.gameObject.transform.position);

        if (distance < 10f && distance > m_Character.m_Stat.Range)
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
            //m_Character.m_Animator.CrossFadeInFixedTime("Walk", 0.1f);
            //m_Character.m_Animator.Play("Walk");
            m_Character.m_zombieState = ZOMBIE_STATE.WALK;
            Debug.Log("WalkStart");
            return NODE_STATE.RUNNING;
        }
        else
        {
            m_nowActionTime += Time.deltaTime;
            m_Character.gameObject.transform.LookAt(PlayerManager.Instance.m_Player.gameObject.transform.position, Vector3.up);
            m_Character.gameObject.transform.position += m_Character.transform.forward * 0.5f * Time.deltaTime;
            if (m_nowActionTime < m_totalActionTime)
            {
                Debug.Log("Walking");
                return NODE_STATE.RUNNING;
            }
        }
        /*
        if (!m_Character.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            m_Character.m_Animator.CrossFade("Walk", 0.3f);
            //m_Character.m_Animator.CrossFadeInFixedTime("Walk", 0.1f);
            //m_Character.m_Animator.Play("Walk");
            Debug.Log("WalkStart");
            return NODE_STATE.RUNNING;
        }
        else
        {
            m_nowActionTime += Time.deltaTime;
            m_Character.gameObject.transform.LookAt(PlayerManager.Instance.m_Player.gameObject.transform.position, Vector3.up);
            m_Character.gameObject.transform.position += m_Character.transform.forward * 0.5f * Time.deltaTime;
            if(m_nowActionTime < m_totalActionTime)
            {
                Debug.Log("Walking");
                return NODE_STATE.RUNNING;
            }
        }
        */

        m_Character.m_zombieState = ZOMBIE_STATE.NONE;
        m_nowActionTime = 0f;
        Debug.Log("Walk out");
        return NODE_STATE.FAIL;
    }
}