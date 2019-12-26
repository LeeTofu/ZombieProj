using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class PlayerObject : MovingObject
{
    public StateController m_StateController { private set; get; }
   // MoveController m_Controller;

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        if (m_Animator == null)
        {
            m_Animator = gameObject.GetComponentInChildren<Animator>();
            m_Animator.applyRootMotion = false;
        }

       // m_Controller = gameObject.AddComponent<MoveController>();
       // m_Controller.Initialize(this);

        m_StateController = gameObject.AddComponent<StateController>();
        m_StateController.Initialize(this);

        return;
    }

  
}
