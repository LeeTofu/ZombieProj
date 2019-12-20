using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleAction : ActionNode
{
    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;
    }
    public override NODE_STATE Tick()
    {
        //플레이 부분
        if (!m_Character.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle1"))
        {
            m_Character.m_Animator.Play("Idle1");
            Debug.Log("idleTest");
        }

        return NODE_STATE.SUCCESS;
    }
}