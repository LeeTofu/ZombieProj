using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class NormalZombieBT : BehaviorNode
{
    private BehaviorNode m_BTHead;

    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;

        SelectorNode sel = new SelectorNode();

        SequenceNode seqAtk = new SequenceNode();
        seqAtk.InsertAction(new ZombieAttackCondition());
        seqAtk.InsertAction(new ZombieAttackAction());
        sel.InsertAction(seqAtk);

        SequenceNode seqWalk = new SequenceNode();
        seqWalk.InsertAction(new ZombieWalkCondition());
        seqWalk.InsertAction(new ZombieWalkAction());
        sel.InsertAction(seqWalk);

        sel.InsertAction(new ZombieIdleAction());

        m_BTHead = sel;

        m_BTHead.Initialize(_character);
    }

    public override NODE_STATE Tick()
    {
        return m_BTHead.Tick();
    }
}