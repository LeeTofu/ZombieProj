using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MovingObject
{
    protected BehaviorNode m_zombieBehavior;

    public override void Initialize(GameObject _Model, MoveController _Controller)
    {
        if(_Model != null) m_Model = _Model;

        if (m_Animator == null) m_Animator = gameObject.GetComponentInChildren<Animator>();

        // Test //
        if (BehaviorManger.Instance.GetNode("NormalZombieNode") != null)
        {
            m_zombieBehavior = BehaviorManger.Instance.GetNode("NormalZombieNode");
        }
        else
        {
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

            BehaviorManger.Instance.AddBehaviorNode("NormalZombieNode", sel);

            m_zombieBehavior = sel;
        }
        m_zombieBehavior.Initialize(this);
   }

    private void Update()
    {
        m_zombieBehavior.Tick();
    }
}
