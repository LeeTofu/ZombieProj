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
        // Test //
        /*
        SequencerNode seqNode = gameObject.AddComponent<SequencerNode>();

        Walk_ObjectAction walk = gameObject.AddComponent<Walk_ObjectAction>();
        walk.Initialize(this, "Walk");
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
        */

        //Test2
        SelectorNode selNode = gameObject.AddComponent<SelectorNode>();

        SequencerNode walkSeqNode = gameObject.AddComponent<SequencerNode>();
        Object_WalkCondition walkCond = gameObject.AddComponent<Object_WalkCondition>();
        Object_WalkAction walkAct = gameObject.AddComponent<Object_WalkAction>();
        walkSeqNode.InsertAction(walkCond);
        walkSeqNode.InsertAction(walkAct);

        SequencerNode attackSeqNode = gameObject.AddComponent<SequencerNode>();
        Object_AttackCondition attackCond = gameObject.AddComponent<Object_AttackCondition>();
        Object_AttackAction attackAct = gameObject.AddComponent<Object_AttackAction>();
        walkSeqNode.InsertAction(attackCond);
        walkSeqNode.InsertAction(attackAct);

        selNode.InsertAction(walkSeqNode);
        selNode.InsertAction(attackSeqNode);
    }
}
