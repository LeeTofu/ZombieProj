using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DashZombieBT : BehaviorNode
{
    private BehaviorNode m_BTHead;

    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;

        SelectorNode sel = new SelectorNode();

        SequenceNode seqDashAtk = new SequenceNode();
        seqDashAtk.InsertAction(new ZombieDashAttackCondition());
        seqDashAtk.InsertAction(new ZombieDashAttackAction());
        sel.InsertAction(seqDashAtk);

        SequenceNode seqStun = new SequenceNode();
        seqStun.InsertAction(new ZombieStunCondition());
        seqStun.InsertAction(new ZombieStunAction());
        sel.InsertAction(seqStun);

        SequenceNode seqPF = new SequenceNode();
        seqPF.InsertAction(new ZombiePathFindCondition());
        seqPF.InsertAction(new ZombiePathFindAction());
        sel.InsertAction(seqPF);

        sel.InsertAction(new ZombieIdleAction());

        m_BTHead = sel;

        m_BTHead.Initialize(_character);
    }

    public override NODE_STATE Tick()
    {
        return m_BTHead.Tick();
    }
}