using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_AttackAction : ActionNode
{
    public override bool OnUpdate()
    {
        if (!m_isActive)
        {
            m_isActive = true;

            Debug.Log("Attack");
            m_Animation.Play("Attack");

            return true;
        }
        return true;
    }
}

public class Object_AttackCondition : ActionNode
{
    public float m_AttackTime;
    private float m_CurTime = 0.0f;

    public override bool OnUpdate()
    {
        if(!m_isActive)
        {
            if (CheckStartCondition())
            {
                m_isActive = true;
                return true;
            }
            else return false;
        }
        else
        {
            m_CurTime += Time.deltaTime;

            if (CheckUpdateCondition())
            {
                if (CheckStopCondition())
                {
                    m_isActive = false;
                    return false;
                }
                else return true;
            }
            else return false;
        }
    }

    public override bool CheckStartCondition()
    {
        float distance = (PlayerManager.m_Player.transform.position - this.m_Character.transform.position).magnitude;

        if (this.m_Character.m_Stat.Range >= distance) return true;

        return false;
    }

    public override bool CheckUpdateCondition()
    {
        /*
        float distance = (PlayerManager.m_Player.transform.position - this.m_Character.transform.position).magnitude;

        if (this.m_Character.m_Stat.Range >= distance) return true;

        return false;
        */

        return true;
    }

    public override bool CheckStopCondition()
    {
        float distance = (PlayerManager.m_Player.transform.position - this.m_Character.transform.position).magnitude;

        if (this.m_Character.m_Stat.Range >= distance) return false;
        if (this.m_CurTime > this.m_AttackTime) return false;

        return true;
    }
}