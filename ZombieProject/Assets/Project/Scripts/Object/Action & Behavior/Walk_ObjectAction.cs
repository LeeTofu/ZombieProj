using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk_ObjectAction : ObjectAction
{
    override public void PlayAction()
    {
        base.PlayAction();
        m_Animation.Play("Walk");
        m_Character.transform.Translate(Vector3.forward * Time.deltaTime * 0.4f, Space.World);
    }

    override public void StopAction()
    {
        base.PlayAction();
        m_Animation.Play("Idle");
        // 오버라이드해서 행동 구현 //
    }
}
