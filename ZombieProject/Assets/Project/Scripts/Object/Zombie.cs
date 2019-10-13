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

        _Model.transform.SetParent(transform);
        _Model.transform.localPosition = Vector3.zero;
        _Model.transform.localRotation = Quaternion.identity;

        m_Controller.Initialize(this);
        m_Controller.InsertActionToTable("Walk", gameObject.AddComponent<Walk_ObjectAction>(), false, true);
        m_Controller.InsertActionToTable("Attack", gameObject.AddComponent<Attack_ObjectAction>(), true, false);

        Invoke("WalkTest" ,5.0f);
    }

    void WalkTest()
    {
        m_Controller.PlayAction("Walk");
    }
}
