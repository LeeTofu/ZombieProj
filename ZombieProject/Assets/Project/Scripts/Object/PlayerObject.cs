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


        return;
    }

    

}
