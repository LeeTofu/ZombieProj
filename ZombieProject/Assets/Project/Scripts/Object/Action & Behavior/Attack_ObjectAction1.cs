using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_ObjectAction : ObjectAction
{
    override public void PlayAction()
    {
        Debug.Log("Attack");
        m_isFinish = false;
        m_Animation.Play("Attack");
    }

    override public void StopAction()
    {
        m_isFinish = true;
    }

    public override bool CheckFinishCondition()
    {
        if (!m_Animation.GetCurrentAnimatorStateInfo(0).IsName("Attack") )
        {
            return true;
        }

        return false;
    }
}
