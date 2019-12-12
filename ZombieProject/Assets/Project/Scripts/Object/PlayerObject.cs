using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MovingObject
{
    MoveController m_Controller;

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        m_Controller = gameObject.AddComponent<MoveController>();
        m_Controller.Initialize(this);

        if (m_Animator == null) m_Animator = gameObject.GetComponentInChildren<Animator>();

        return;
    }

    private void Update()
    {
        if(m_Controller.GetInputContoller() != null)
        {
            if (m_Controller.GetInputContoller().GetisHit()) m_Animator.SetBool("isWalking", true);
            else m_Animator.SetBool("isWalking", false);
        }
    }
}
