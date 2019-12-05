using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MovingObject
{
    protected BehaviorNode m_zombieBehavior;

    // Start is called before the first frame update

     
    public override void Initialize(GameObject _Model, MoveController _Controller)
    {
        if(_Model != null)
        m_Model = _Model;

        if(m_Animator == null)
        {
            m_Animator = gameObject.GetComponent<Animator>();
        }


        // Test //
        m_zombieBehavior = BehaviorManger.Instance.GetNode("NormalZombieNode");
        if(m_zombieBehavior == null)
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
    }

    private void Update()
    {
        if (m_zombieBehavior == null) return;

        m_zombieBehavior.Tick();
    }
}
