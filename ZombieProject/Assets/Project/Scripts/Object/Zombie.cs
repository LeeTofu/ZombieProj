using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Zombie : MovingObject
{
    protected BehaviorNode m_zombieBehavior;

    private Coroutine m_KnockBackCoroutine;

    public override void InGame_Initialize()
    {
        if(m_CollisionAction != null)
            m_CollisionAction.SetCollisionActive(true);
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
            MoveSpeed = Random.Range(0.55f,0.75f),
            alertRange = 100.0f,
            isKnockBack = false,
        };

        m_zombieBehavior = new NormalZombieBT();
        m_zombieBehavior.Initialize(this);

        m_DeadActionCallBackFunc = DeadActionCallback;
        m_KnockBackAction = (time) => { KnockBackAction(time); };

        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<ZombieCollisionAction>();

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

    private void DeadActionCallback()
    {
        if (SceneMaster.Instance.m_CurrentScene == GAME_SCENE.IN_GAME)
        {
            if (m_KnockBackCoroutine != null)
            {
                StopCoroutine(m_KnockBackCoroutine);
            }

            if(m_Type == OBJECT_TYPE.ELITE_ZOMBIE)
                BattleSceneMain.CreateBuffItem(transform.position + Vector3.up * 0.1f, Quaternion.identity);

            PlayerManager.Instance.CurrentMoney += 10;

            if (m_CollisionAction != null)
                m_CollisionAction.SetCollisionActive(false);
        }
    }

    private void Update()
    {
        m_zombieBehavior.Tick();
    }
}
