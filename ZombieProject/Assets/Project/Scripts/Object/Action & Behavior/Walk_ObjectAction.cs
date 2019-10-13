﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk_ObjectAction : ObjectAction
{
    override public void PlayAction()
    {
        Debug.Log("zombie Walk");
        m_isFinish = false;
        m_Animation.Play("Walk");
        m_Character.transform.Translate(Vector3.forward * Time.deltaTime * 0.4f, Space.World);
    }

    override public void StopAction()
    {
        m_isFinish = true;
    }
}
