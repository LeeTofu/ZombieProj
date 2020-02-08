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

       /*
        SequenceNode seqDead = new SequenceNode();
        seqDead.InsertAction(new ZombieDeadCondition());
        seqDead.InsertAction(new ZombieDeadAction());
        sel.InsertAction(seqDead);

        SequenceNode seqKnockBack = new SequenceNode();
        seqKnockBack.InsertAction(new ZombieKnockBackCondition());
        seqKnockBack.InsertAction(new ZombieKnockBackAction());
        sel.InsertAction(seqKnockBack);
        */

        SequenceNode seqAtk = new SequenceNode();
        seqAtk.InsertAction(new ZombieAttackCondition());
        seqAtk.InsertAction(new ZombieAttackAction());
        sel.InsertAction(seqAtk);

       /*
        SequenceNode seqWalk = new SequenceNode();
        seqWalk.InsertAction(new ZombieWalkCondition());
        seqWalk.InsertAction(new ZombieWalkAction());
        sel.InsertAction(seqWalk);
        */

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