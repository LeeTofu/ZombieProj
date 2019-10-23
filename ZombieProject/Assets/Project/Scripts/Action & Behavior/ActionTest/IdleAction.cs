using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_IdleAction : ActionNode
{
    public override bool OnUpdate()
    {
        if (!m_isActive)
        {
            m_isActive = true;

            Debug.Log("Idle");
            m_Animation.Play("Idle");

            return true;
        }
        else return true;
    }
}