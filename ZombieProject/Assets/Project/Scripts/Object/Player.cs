using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject
{
    MoveController m_AIContoller;

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        m_AIContoller = gameObject.AddComponent<PlayerMoveController>();
        

        return;
    }

    

}
