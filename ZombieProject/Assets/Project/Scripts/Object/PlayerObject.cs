using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class PlayerObject : MovingObject
{
    public StateController m_StateController;
    // MoveController m_Controller;
    private IEnumerator cor;

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

        return;
    }

    private void OnEnable()
    {
        if (SceneMaster.Instance.m_CurrentScene == GAME_SCENE.IN_GAME)
        {
            if (m_StateController == null)
            {
                m_StateController = gameObject.AddComponent<StateController>();
                m_StateController.Initialize(this);
            }

            m_StateController.InGame_Initialize();
        }
    }


}
