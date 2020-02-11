﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Zombie : MovingObject
{
    protected BehaviorNode m_zombieBehavior;

    private Coroutine m_KnockBackCoroutine;

    public override void InGame_Initialize()
    {
        m_HpUi.enabled = false;
        m_Stat.AddPropertyChangeAction(() =>
        {
            if (m_Stat.CurHP == m_Stat.MaxHP)
                m_HpUi.enabled = false;
            else
                m_HpUi.enabled = true;
            HpChange();
        });
        if (m_CollisionAction != null)
            m_CollisionAction.SetCollisionActive(true);
    }

    public override void Initialize(GameObject _Model, MoveController _Controller, int _typeKey)
    {
        m_TypeKey = _typeKey;

        if (_Model != null) m_Model = _Model;
        m_Height = m_Model.transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Neck/Head").transform.position.y;


        m_HpUi = transform.Find("HPUI").GetComponent<Canvas>();
        m_HpBar = transform.Find("HPUI").GetChild(0).GetComponent<Image>();
        m_HpImage = transform.Find("HPUI").GetChild(0).GetChild(0).GetComponent<Image>();

        if (m_Animator == null) m_Animator = gameObject.GetComponentInChildren<Animator>();
        // Test // -> 태그별로 각자 다르게 만들것
        m_zombieState = ZOMBIE_STATE.IDLE;
        m_Stat = new STAT
        {
            MaxHP = 100,
            Range = 1.5f,
            MoveSpeed = Random.Range(0.55f,0.75f),
            alertRange = 100.0f,
            isKnockBack = false,
        };

        //이부분은 자기 TYPE받아서 바꾸게 만들어야함
        m_zombieBehavior = new NormalZombieBT();
        m_zombieBehavior.Initialize(this);

        m_DeadActionCallBackFunc = DeadActionCallback;
        m_KnockBackAction = (time) => { KnockBackAction(time); };

        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<ZombieCollisionAction>();

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
        if (!gameObject.activeSelf) return;
        if (m_zombieBehavior == null) return;

        if (CameraManager.Instance.m_Camera != null)
        {
            m_ScreenPos = CameraManager.Instance.m_Camera.WorldToScreenPoint(new Vector3(
                transform.position.x,
                transform.position.y + m_Height,
                transform.position.z
                ));
            m_HpBar.transform.position = new Vector3(m_ScreenPos.x, m_ScreenPos.y + 30f, m_HpBar.transform.position.z);
            //플레이어와의 거리가 일정거리가 될때까지 navagent이용해서 찾아감
        }

        //플레이어가 일정거리 이내 있을때 BT실행
        m_zombieBehavior.Tick();
    }
}
