using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class PlayerObject : MovingObject
{
    public StateController m_StateController;
    // MoveController m_Controller;
    private Coroutine m_Coroutine;

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
        AddKnockBackAction(0.1f);
        AddKnockBackFunction((float time)=>
        {
            if (m_Coroutine != null)
                StopCoroutine(m_Coroutine);
            m_Coroutine = StartCoroutine(KnockBackChange(time));
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace) && !PlayerManager.Instance.m_Player.m_Stat.isKnockBack)
        {
            PlayerManager.Instance.m_Player.HitDamage(35f, true, 2f);
        }
    }

    private IEnumerator KnockBackChange(float _time)
    {
        m_StateController.ChangeState(E_PLAYABLE_STATE.KNOCKBACK);
        while(!m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        {
            yield return null;
        }
        while(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            yield return null;
        }
        if (m_Stat.CurHP <= m_InjuredHP)
            m_StateController.ChangeState(E_PLAYABLE_STATE.INJURED_IDLE);
        else
            m_StateController.ChangeState(E_PLAYABLE_STATE.IDLE);

    }
}
