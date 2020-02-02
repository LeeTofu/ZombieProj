using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class PlayerObject : MovingObject
{
    public StateController m_StateController;
    // MoveController m_Controller;
    private Coroutine m_Coroutine;

    public Buff m_ReservedBuff;

    public override void InGame_Initialize()
    {
        SetStat(new STAT
        {
            MaxHP = 100f,
            CurHP = 100f,
            Defend = 100f,
            MoveSpeed = 3.0f
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
    }

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        if (m_Animator == null)
        {
            m_Animator = gameObject.GetComponentInChildren<Animator>();
            m_Animator.applyRootMotion = false;
        }

        // m_Controller = gameObject.AddComponent<MoveController>();
        // m_Controller.Initialize(this);

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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace) && !PlayerManager.Instance.m_Player.m_Stat.isKnockBack)
        {
           // EffectManager.Instance.AttachEffect(PARTICLE_TYPE.DROP_ITEM, this, Quaternion.Euler(-90.0f, 0, 0),
           // Vector3.one * 1.0f);

            (UIManager.Instance.m_CurrentUI as BattleUI).OnDamagedEffect();
            PlayerManager.Instance.m_Player.HitDamage(35f, true, 2f);
        }
    }
}
