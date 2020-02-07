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

    // Player 표시해주는 이펙트
    public EffectObject m_PlayerEffect;

    public override void InGame_Initialize()
    {
        m_CollisionAction.SetCollisionActive(true);
        m_HpImage.fillAmount = 1f;
        m_HpUi.enabled = true;
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

            if (m_HpChangeCoroutine != null)
                StopCoroutine(m_HpChangeCoroutine);

            m_HpChangeCoroutine = StartCoroutine(HpChange());
        });
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
            m_PlayerEffect = EffectManager.Instance.AttachEffect(PARTICLE_TYPE.DROP_ITEM, this, Vector3.up * 0.2f, Quaternion.Euler(90,0,0), Vector3.one);
    }

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        m_HpUi = transform.Find("HPUI").GetComponent<Canvas>();
        m_HpImage = transform.Find("HPUI").GetChild(0).GetChild(0).GetComponent<Image>();
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

        SetStat(new STAT
        {
            MaxHP = 100f,
            CurHP = 100f,
            Defend = 100f,
            MoveSpeed = 3.0f
        });
        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<PlayerCollisionAction>();

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
    protected new void DeadAction()
    {
        base.DeadAction();
        ChangeState(E_PLAYABLE_STATE.DEATH);

        if (m_PlayerEffect != null && m_PlayerEffect.gameObject.activeSelf)
        {
            m_PlayerEffect.pushToMemory(m_PlayerEffect.m_EffectTypeID);
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
