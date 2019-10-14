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


        // Test //
        SequencerNode seqNode = gameObject.AddComponent<SequencerNode>();

        Walk_ObjectAction walk = gameObject.AddComponent<Walk_ObjectAction>();
        walk.Initialize(this, "Attack");
        walk.m_DestinationPosition = GameObject.Find("Destination").transform.position;

        Idle_ObjectAction idle = gameObject.AddComponent<Idle_ObjectAction>();
        idle.Initialize(this, "Idle");
        idle.m_AttackTime = 3.0f;

        Attack_ObjectAction attack = gameObject.AddComponent<Attack_ObjectAction>();
        attack.Initialize(this, "Attack");
        attack.m_AttackTime = 3.0f;

        seqNode.InsertAction(walk);
        seqNode.InsertAction(idle);
        seqNode.InsertAction(attack);

        string newActionName = walk.m_ActionName + idle.m_ActionName + attack.m_ActionName;
        m_Controller.InsertActionToTable(walk.m_ActionName + idle.m_ActionName + attack.m_ActionName, seqNode);
        m_Controller.PlayAction(newActionName);

    }



}
