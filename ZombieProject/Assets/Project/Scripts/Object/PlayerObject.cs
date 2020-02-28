using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;

public class PlayerObject : MovingObject
{
    public StateController m_StateController;
    private Coroutine m_Coroutine;

    public Buff m_ReservedBuff;
    public System.Action m_ReservedAction;

    // Player 표시해주는 이펙트
    public EffectObject m_PlayerEffect;

    public DrawRange m_DrawRander { get; private set; }

    public override void InGame_Initialize()
    {
        m_CollisionAction.SetCollisionActive(true);
        if (m_BuffRimLight != null) m_BuffRimLight.SetStandard();
        SetStat(new STAT
        {
            MaxHP = 100f,
            CurHP = 100f,
            Defend = 100f,
            MoveSpeed = 3.0f
        });
        m_Stat.AddPropertyChangeAction(() =>
        {
            if (m_Stat.CheckIsDead())
                DeadAction();
        });

        m_Stat.isKnockBack = false;
        m_Stat.CurHP = m_Stat.MaxHP;

        if (SceneMaster.Instance.m_CurrentScene == GAME_SCENE.IN_GAME)
        {
            if (m_StateController == null)
            {
                m_StateController = gameObject.AddComponent<StateController>();
                m_StateController.Initialize(this);
            }

            m_StateController.InGame_Initialize();
        }

        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<PlayerCollisionAction>();

        if(m_PlayerEffect == null)
            m_PlayerEffect = EffectManager.Instance.AttachEffect(PARTICLE_TYPE.PLAYER, this, Vector3.up * 0.2f, Quaternion.Euler(90,0,0), Vector3.one);


        m_HpBarUI.InGame_Initialize();


    }

    public override void Initialize(GameObject _Model, MoveController _Controller, int _typeKey)
    {
        m_TypeKey = _typeKey;

        if (_Model != null) m_Model = _Model;
        if (m_BuffRimLight == null)
        {
            m_BuffRimLight = GetComponent<BuffRimLight>();
            m_BuffRimLight.Initialize(_Model);
        }
        if (m_HpBarUI == null) m_HpBarUI = GetComponent<HpBarUI>();

        if (m_Animator == null)
        {
            m_Animator = gameObject.GetComponentInChildren<Animator>();
            m_Animator.applyRootMotion = false;
        }

        if (m_StateController == null)
        {
            m_StateController = gameObject.AddComponent<StateController>();
            m_StateController.Initialize(this);
        }

        m_DrawRander = gameObject.AddComponent<DrawRange>();

        SetStat(new STAT
        {
            MaxHP = 100f,
            CurHP = 100f,
            Defend = 100f,
            MoveSpeed = 3.0f
        });
        m_Stat.AddPropertyChangeAction(() =>
        {
            if (m_Stat.CheckIsDead())
                DeadAction();
        });

        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<PlayerCollisionAction>();

        m_HpBarUI.Initialzie(this);

        return;
    }

    public void ChangeState(E_PLAYABLE_STATE _state)
    {
        if(m_StateController != null)
            m_StateController.ChangeState(_state);
    }

    public void ReserveBuff(Buff _buff)
    {
        

        _buff.SetStat(m_Stat);
        m_ReservedBuff = _buff;
    }

    public void ReserveAction (System.Action _action)
    {
        m_ReservedAction = _action;
    }

    protected new void DeadAction()
    {
        base.DeadAction();
        ChangeState(E_PLAYABLE_STATE.DEATH);

        if (m_PlayerEffect != null && m_PlayerEffect.gameObject.activeSelf)
        {
            m_PlayerEffect.pushToMemory();
            m_PlayerEffect = null;
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace) && !PlayerManager.Instance.m_Player.m_Stat.isKnockBack && !PlayerManager.Instance.m_Player.m_Stat.isDead)
        {
           // EffectManager.Instance.AttachEffect(PARTICLE_TYPE.DROP_ITEM, this, Quaternion.Euler(-90.0f, 0, 0),
           // Vector3.one * 1.0f);

            (UIManager.Instance.m_CurrentUI as BattleUI).OnDamagedEffect();
            PlayerManager.Instance.m_Player.HitDamage(35f, true, 2f);
        }
    }
}
