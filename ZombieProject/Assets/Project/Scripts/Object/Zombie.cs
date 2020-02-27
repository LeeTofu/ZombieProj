using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Zombie : MovingObject
{
    protected BehaviorNode m_zombieBehavior;

    protected Coroutine m_KnockBackCoroutine;

    private AudioSource m_AudioSource;

    [SerializeField]
    protected int m_StartMoney;

    [SerializeField]
    protected int m_StepMoney;

    [SerializeField]
    AudioClip[] m_AttackAudioSource;

    [SerializeField]
    AudioClip[] m_IdleAudioSource;

    [SerializeField]
    AudioClip[] m_HurtAudioSource;

    [SerializeField]
    [Range(0, 999)]
    int m_ItemDropPercentage;

    int m_ModelIndex;

    // public MeleeAttackCollision m_MeleeAttackCollision;

    public override void InGame_Initialize()
    {
       
        if (m_CollisionAction != null)
            m_CollisionAction.SetCollisionActive(true);
    }

    public void PlayAttackSound()
    {
        if (m_AudioSource == null) return;

        int attackSoundIdx = m_AttackAudioSource.Length;
        m_AudioSource.PlayOneShot(m_AttackAudioSource[Random.Range(0, attackSoundIdx)]);
    }

    public void PlayHurtSound()
    {
        if (m_AudioSource == null) return;

        int hurtSoundIdx = m_HurtAudioSource.Length;
        m_AudioSource.PlayOneShot(m_HurtAudioSource[Random.Range(0, hurtSoundIdx)]);
    }

    public void PlayIdleSound()
    {
        if (m_AudioSource == null) return;

        int idleSoundIdx = m_IdleAudioSource.Length;
        m_AudioSource.PlayOneShot(m_IdleAudioSource[Random.Range(0, idleSoundIdx)]);
    }


    public override void Initialize(GameObject _Model, MoveController _Controller, int _typeKey)
    {
        m_TypeKey = _typeKey;
      //  m_Type = (OBJECT_TYPE)_typeKey;
        if (_Model != null) m_Model = _Model;

        SkinnedMeshRenderer[] skin = m_Model.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        skin[Random.Range(0, skin.Length)].gameObject.SetActive(true);

        for(int i = 0; i < skin.Length; i++)
        {
            if (skin[i].gameObject.activeSelf) continue;

            skin[i].transform.SetParent(null);
            Destroy(skin[i].gameObject);
        }

        if (m_BuffRimLight == null)
        {
            m_BuffRimLight = GetComponent<BuffRimLight>();
            if (_Model != null && m_BuffRimLight != null) m_BuffRimLight.Initialize(m_Model);
        }
        if (m_HpBarUI == null) m_HpBarUI = GetComponent<HpBarUI>();
        if (m_Animator == null) m_Animator = gameObject.GetComponentInChildren<Animator>();
        // Test // -> 태그별로 각자 다르게 만들것

        m_zombieState = ZOMBIE_STATE.IDLE;
        InitByZombieType(this.m_Type);

        m_AudioSource = GetComponent<AudioSource>();
        m_HpBarUI.Initialzie(this);

        if (m_NavAgent == null)
        {
            m_NavAgent = gameObject.GetComponentInChildren<NavMeshAgent>();
            m_NavAgent.updateRotation = true;
            m_NavAgent.acceleration = 10f;
        }
    }

    protected void OnDestroy()
    {
        m_TargetingObject = null;
    }

    protected void KnockBackAction(float _time)
    {
        m_Stat.isKnockBack = true;

        if(m_KnockBackCoroutine != null)
        {
            StopCoroutine(m_KnockBackCoroutine);
        }

        PlayHurtSound();

        m_KnockBackCoroutine = StartCoroutine(ExitKnockBack(_time));
    }

    private IEnumerator ExitKnockBack(float _time)
    {
        yield return new WaitForSeconds(_time);
        m_Stat.isKnockBack = false;
    }

    protected virtual void DeadActionCallback()
    {
        if (SceneMaster.Instance.m_CurrentScene == GAME_SCENE.IN_GAME)
        {
            if (m_KnockBackCoroutine != null)
            {
                StopCoroutine(m_KnockBackCoroutine);
            }

            m_TargetingObject = null;

            PlayerManager.Instance.CurrentMoney += (m_StartMoney + RespawnManager.Instance.m_CurWave * m_StepMoney);

            if (m_ItemDropPercentage > Random.Range(0, 1000))
            {
                BattleSceneMain.CreateBuffItem(transform.position, Quaternion.identity);
            }

            if (m_CollisionAction != null)
                m_CollisionAction.SetCollisionActive(false);

            if (m_NavAgent != null)
            {
                m_NavAgent.enabled = true;

            }
        }
    }

    protected void InitByZombieType(OBJECT_TYPE _type)
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
                    m_CollisionAction = gameObject.AddComponent<DashZombieCollisionAction>();
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


        if (m_zombieBehavior != null)
            m_zombieBehavior.Initialize(this);
        else
            Debug.LogError(_type + "없" );
    }

    protected void Update()
    {
        if (!gameObject.activeSelf) return;
        if (m_zombieBehavior == null) return;

        m_zombieBehavior.Tick();
    }
}
