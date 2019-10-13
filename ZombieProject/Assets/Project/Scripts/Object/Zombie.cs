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
            Debug.Log("Not have Move cotroller");
            m_Controller = gameObject.AddComponent<MoveController>();
        }

        _Model.transform.SetParent(transform);
        _Model.transform.localPosition = Vector3.zero;
        _Model.transform.localRotation = Quaternion.identity;

        m_Controller.Initialize(this, gameObject.AddComponent<Walk_ObjectAction>());

    }
}
