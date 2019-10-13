using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_ObjectAction : ObjectAction
{
    override public void PlayAction()
    {
        m_isFinish = false;
        m_Animation.Play("Idle1");
    }

    override public void StopAction()
    {
        m_isFinish = true;
    }

}
