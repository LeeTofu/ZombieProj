using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MovingObject
{
    protected BehaviorNode m_zombieBehavior;

    // Start is called before the first frame update

     
    public override void Initialize(GameObject _Model, MoveController _Controller)
    {
        if(_Model != null) m_Model = _Model;

        if (m_Animator == null) m_Animator = gameObject.GetComponentInChildren<Animator>();


        if (BehaviorManger.Instance.GetNode("NormalZombieNode") != null)
        {
            m_zombieBehavior = BehaviorManger.Instance.GetNode("NormalZombieNode");
            Debug.Log("notNull");
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

            Debug.Log("Null");
        }


        m_zombieBehavior.SetChildCharacter(this);
        // Test //

    }

    private void Update()
    {
        if (m_zombieBehavior == null)
        {
            if (BehaviorManger.Instance.GetNode("NormalZombieNode") != null)
            {
                m_zombieBehavior = BehaviorManger.Instance.GetNode("NormalZombieNode");
            }
            else
            {
                SelectorNode sel = new SelectorNode();

                SequenceNode seqAtk = new SequenceNode();
                ZombieAttackCondition zombieAC = new ZombieAttackCondition();
                ZombieAttackAction zombieAA = new ZombieAttackAction();
                seqAtk.InsertAction(zombieAC);
                seqAtk.InsertAction(zombieAA);
                sel.InsertAction(seqAtk);

                SequenceNode seqWalk = new SequenceNode();
                ZombieWalkCondition zombieWC = new ZombieWalkCondition();
                ZombieWalkAction zombieWA = new ZombieWalkAction();
                seqWalk.InsertAction(zombieWC);
                seqWalk.InsertAction(zombieWA);
                sel.InsertAction(seqWalk);

                ZombieIdleAction zombieIA = new ZombieIdleAction();
                sel.InsertAction(zombieIA);

                BehaviorManger.Instance.AddBehaviorNode("NormalZombieNode", sel);

                m_zombieBehavior = sel;
            }

            m_zombieBehavior.SetChildCharacter(this);
        }

        m_zombieBehavior.Tick();
    }
}
