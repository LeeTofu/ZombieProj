using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RangeZombieBT : BehaviorNode
{
    private BehaviorNode m_BTHead;

    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;

        SelectorNode sel = new SelectorNode();

        SequenceNode seqRangeAtk = new SequenceNode();
        seqRangeAtk.InsertAction(new ZombieRangeAttackCondition());
        seqRangeAtk.InsertAction(new ZombieRangeAttackAction());
        sel.InsertAction(seqRangeAtk);

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