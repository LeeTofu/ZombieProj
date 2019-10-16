using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_ObjectAction : ActionNode
{
    public float m_AttackTime;
    private float m_CurTime = 0.0f;


    public override bool OnUpdate()
    {
        if (!m_isActive) return true;
        Debug.Log("Idle");
        m_CurTime += Time.deltaTime;

        m_Animation.Play("Idle1");

        if (m_CurTime > m_AttackTime)
        {
            m_isActive = false;
            return true;
        }

        return false;
    }

}
