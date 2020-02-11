using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Zombie : MovingObject
{
    protected BehaviorNode m_zombieBehavior;

    private Coroutine m_KnockBackCoroutine;

    HUDHpBar m_HPBar;

    public override void InGame_Initialize()
    {
        if (m_HPBar == null)
        {
            //m_HPBar = Instantiate(Resources.Load<GameObject>("Prefabs/HUD/HPUI")).GetComponent<HUDHpBar>();
            m_HPBar = GetComponentInChildren<HUDHpBar>();
        }

        m_HPBar.Initialize(this);

        if (m_CollisionAction != null)
            m_CollisionAction.SetCollisionActive(true);

        if (m_Stat != null)
        {
            m_Stat.AddPropertyChangeAction(() =>
            {
                m_HPBar.HPChange();
            });
        }
    }

    public override void Initialize(GameObject _Model, MoveController _Controller)
    {
        if(_Model != null) m_Model = _Model;

        if (m_Animator == null) m_Animator = gameObject.GetComponentInChildren<Animator>();
        // Test //
        m_zombieState = ZOMBIE_STATE.IDLE;

        m_zombieBehavior = new NormalZombieBT();
        m_zombieBehavior.Initialize(this);

        m_DeadActionCallBackFunc = DeadActionCallback;
        m_KnockBackAction = (time) => { KnockBackAction(time); };

        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<ZombieCollisionAction>();



        m_Stat = new STAT();

      //  m_HPBar.Initialize(this);
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

    private void Update()
    {

        //플레이어와의 거리가 일정거리가 될때까지 navagent이용해서 찾아감


        //플레이어가 일정거리 이내 있을때 BT실행
        m_zombieBehavior.Tick();
    }
}
