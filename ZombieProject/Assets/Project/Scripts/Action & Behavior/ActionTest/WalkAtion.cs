using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_WalkAction : ActionNode
{
    public override bool OnUpdate()
    {
        if (!m_isActive)
        {
            m_isActive = true;

            Debug.Log("Walk");
            m_Animation.Play("Walk");

            return true;
        }
        else
        {
            this.m_Character.transform.LookAt(PlayerManager.m_Player.transform.position, Vector3.up);

            this.m_Character.transform.position += this.m_Character.transform.forward * 0.2f;

            return true;
        }
        
    }
}

public class Object_WalkCondition : ActionNode
{
    public float m_WalkTime;
    private float m_CurTime = 0.0f;

    public override bool OnUpdate()
    {
        if (!m_isActive)
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

        if (this.m_Character.m_Stat.Range <= distance) return true;

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

        if (this.m_Character.m_Stat.Range <= distance) return false;
        if (this.m_CurTime > this.m_WalkTime) return false;

        return true;
    }
}