using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Zombie : MovingObject
{
    protected BehaviorNode m_zombieBehavior;

    private Coroutine m_KnockBackCoroutine;

   // public MeleeAttackCollision m_MeleeAttackCollision;

    public override void InGame_Initialize()
    {
        m_HpBarUI.InGame_Initialize();
        if (m_CollisionAction != null)
            m_CollisionAction.SetCollisionActive(true);
    }


    public override void Initialize(GameObject _Model, MoveController _Controller, int _typeKey)
    {
        m_TypeKey = _typeKey;

        if (_Model != null) m_Model = _Model;
        if (m_HpBarUI == null) m_HpBarUI = GetComponent<HpBarUI>();
        if (m_Animator == null) m_Animator = gameObject.GetComponentInChildren<Animator>();
        // Test // -> 태그별로 각자 다르게 만들것

        m_zombieState = ZOMBIE_STATE.IDLE;
        InitByZombieType(this.m_Type);

        //if (m_NavAgent == null)
        //{
        //    m_NavAgent = gameObject.GetComponentInChildren<NavMeshAgent>();
        //    m_NavAgent.updateRotation = true;
        //    m_NavAgent.stoppingDistance = m_Stat.Range;
        //    m_NavAgent.speed = m_Stat.MoveSpeed;
        //    m_NavAgent.acceleration = 0.6f;
        //}
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

            //  if(m_Type == OBJECT_TYPE.ZOMBIE)
            //      BattleSceneMain.CreateBuffItem(transform.position + Vector3.up * 0.1f, Quaternion.identity);

            PlayerManager.Instance.CurrentMoney += 10;

            if (m_CollisionAction != null)
                m_CollisionAction.SetCollisionActive(false);
        }
    }

    private void InitByZombieType(OBJECT_TYPE _type)
    {
        switch(_type)
        {
            case OBJECT_TYPE.ZOMBIE:
                m_zombieBehavior = new NormalZombieBT();

                m_DeadActionCallBackFunc = DeadActionCallback;
                m_KnockBackAction = (time) => { KnockBackAction(time); };

                if (m_CollisionAction == null)
                    m_CollisionAction = gameObject.AddComponent<ZombieCollisionAction>();
                break;
            case OBJECT_TYPE.RANGE_ZOMBIE:
                m_zombieBehavior = new RangeZombieBT();

                m_DeadActionCallBackFunc = DeadActionCallback;
                m_KnockBackAction = (time) => { KnockBackAction(time); };

                if (m_CollisionAction == null)
                    m_CollisionAction = gameObject.AddComponent<ZombieCollisionAction>();
                break;
            case OBJECT_TYPE.DASH_ZOMBIE:
                m_zombieBehavior = new DashZombieBT();

                m_DeadActionCallBackFunc = DeadActionCallback;
                m_KnockBackAction = (time) => { KnockBackAction(time); };

                if (m_CollisionAction == null)
                    m_CollisionAction = gameObject.AddComponent<ZombieCollisionAction>();
                break;
            case OBJECT_TYPE.BOMB_ZOMBIE:
                m_zombieBehavior = new BombZombieBT();

                m_DeadActionCallBackFunc = DeadActionCallback;
                m_KnockBackAction = (time) => { KnockBackAction(time); };

                if (m_CollisionAction == null)
                    m_CollisionAction = gameObject.AddComponent<ZombieCollisionAction>();
                break;
            default:
                break;
        }

        m_zombieBehavior.Initialize(this);
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        if (m_zombieBehavior == null) return;

        m_zombieBehavior.Tick();
    }
}
