using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MovingObject
{
    // Start is called before the first frame update
    
    public override void Initialize(GameObject _Model, MoveController _Controller)
    {
        m_Controller = _Controller;

        if (m_Controller == null)
        {
            m_Controller = gameObject.AddComponent<MoveController>();
        }

        m_Controller.Initialize(this);

        if(m_Animator == null)
        {
            m_Animator = gameObject.GetComponent<Animator>();
        }

        // Test //
    }
}
