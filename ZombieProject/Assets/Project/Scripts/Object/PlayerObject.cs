using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class PlayerObject : MovingObject
{
    public StateController m_StateController { private set; get; }
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

        m_StateController = gameObject.AddComponent<StateController>();
        m_StateController.Initialize(this);

        SetStat(new STAT
        {
            MaxHP = 100f,
            CurHP = 100f,
            Defend = 100f,
            MoveSpeed = 1.0f
        });

        return;
    }

    private void OnEnable()
    {
        if(SceneMaster.Instance.m_CurrentScene == GAME_SCENE.IN_GAME)
            m_StateController.InGame_Initialize();
    }

    private new void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Wall"))
        {
            if (m_Stat.m_Coroutine != null) StopCoroutine(m_Stat.m_Coroutine);
            m_Stat = new Blessing(m_Stat);
            StartCoroutine(m_Stat.m_Coroutine);
        }
        else if (other.tag.Equals("Zombie"))
        {
            if (m_Stat.m_Coroutine != null) StopCoroutine(m_Stat.m_Coroutine);
            m_Stat = new Poison(m_Stat);
            StartCoroutine(m_Stat.m_Coroutine);
        }
        m_Stat.Action();
    }


}
