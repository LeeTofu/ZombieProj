using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Zombie : MovingObject
{
    protected BehaviorNode m_zombieBehavior;

    private Coroutine m_KnockBackCoroutine;

    public override void InGame_Initialize()
    {
        if(m_CapsuleCollider != null)
            m_CapsuleCollider.enabled = true;
    }


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

        m_DeadActionCallBackFunc = DeadAction;
        m_KnockBackAction = (time) => { KnockBackAction(time); };
   }

    private void KnockBackAction(float _time)
    {
        m_Stat.isKnockBack = true;

        if(m_KnockBackCoroutine != null)
        {
            StopCoroutine(m_KnockBackCoroutine);
        }

        m_KnockBackCoroutine = StartCoroutine(ExitKnockBack(_time));
    }

    private IEnumerator ExitKnockBack(float _time)
    {
        yield return new WaitForSeconds(_time);
        m_Stat.isKnockBack = false;
    }

    private void DeadAction()
    {
        if (SceneMaster.Instance.m_CurrentScene == GAME_SCENE.IN_GAME)
        {
            if (m_KnockBackCoroutine != null)
            {
                StopCoroutine(m_KnockBackCoroutine);
            }

            BattleSceneMain.CreateBuffItem(transform.position + Vector3.up * 0.1f, Quaternion.identity);

            if (m_CapsuleCollider != null)
                m_CapsuleCollider.enabled = false;
        }
    }

    private void Update()
    {
        m_zombieBehavior.Tick();
    }
}
