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
        m_zombieBehavior = new NormalZombieBT();
        m_zombieBehavior.Initialize(this);
   }

    private void Update()
    {
        m_zombieBehavior.Tick();
    }
}
