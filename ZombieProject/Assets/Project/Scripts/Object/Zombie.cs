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
        m_zombieState = ZOMBIE_STATE.IDLE;

        m_Stat = new STAT
        {
            MaxHP = 100,
            Range = 1.5f,
            MoveSpeed = 1.0f,
            isKnockBack = false,
        };

        m_zombieBehavior = new NormalZombieBT();
        m_zombieBehavior.Initialize(this);


   }

    private void DeadAction()
    {
        SceneMain main = SceneMaster.Instance.GetCurrentMain();

        if (main != null)
        {
            BattleSceneMain.CreateBuffItem(transform.position + Vector3.up * 0.1f, Quaternion.identity);
        }
    }

    private void Update()
    {
        m_zombieBehavior.Tick();
    }
}
