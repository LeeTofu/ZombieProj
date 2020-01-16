using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class PlayerObject : MovingObject
{
    public StateController m_StateController;
    // MoveController m_Controller;
    private Coroutine m_Coroutine;

    public override void InGame_Initialize()
    {
        SetStat(new STAT
        {
            MaxHP = 100f,
            CurHP = 100f,
            Defend = 100f,
            MoveSpeed = 3.0f
        });
        BuffManager.Instance.SetStat(m_Stat);
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
        BuffManager.Instance.SetStat(m_Stat);
        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<PlayerCollisionAction>();

        return;
    }


    public void ChangeState(E_PLAYABLE_STATE _state)
    {
        if(m_StateController != null)
        m_StateController.ChangeState(_state);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace) && !PlayerManager.Instance.m_Player.m_Stat.isKnockBack)
        {
            PlayerManager.Instance.m_Player.HitDamage(35f, true, 2f);
        }
    }
}
