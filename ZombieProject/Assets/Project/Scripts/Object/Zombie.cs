using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MovingObject
{
    // Start is called before the first frame update
    
    public override void Initialize(GameObject _Model, MoveController _Controller)
    {
        if(m_Animator == null)
        {
            m_Animator = gameObject.GetComponent<Animator>();
        }

        // Test //
    }
}
