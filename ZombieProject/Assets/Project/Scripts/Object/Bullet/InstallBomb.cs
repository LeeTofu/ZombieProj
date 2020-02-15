using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallBomb : Bullet
{
    AudioSource m_AuidoSource;
    float m_Time = 0.0f;

   

    public override void InGame_Initialize()
    {
        m_Time = 0.0f;

        if (m_AuidoSource == null)
            m_AuidoSource = GetComponent<AudioSource>();

    }

    public override void Initialize(GameObject _model, MoveController _Controller, int _typeKey )
    {
        m_TypeKey = _typeKey;
        base.Initialize(_model, _Controller, _typeKey);

        m_Time = 0.0f;

        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<BulletCollisionAction>();

        if (m_AuidoSource == null)
            m_AuidoSource = gameObject.AddComponent<AudioSource>();
    }

    protected override void BulletMove()
    {
        m_Time += Time.deltaTime;

        if (m_Time > 0.75f)
        {
            EnemyManager.Instance.SetTargetingRangeZombies(this, transform.position, m_Stat.Range);
            SoundManager.Instance.OneShotPlay(UI_SOUND.INSTALL_BOMB);

            m_PointLight.SetActive(m_PointLight.activeSelf == true ? false : true);

            m_Time = 0.0f;
        }
    }

    protected override void BulletOverRangefunction()
    {
        EffectManager.Instance.PlayEffect(
                    PARTICLE_TYPE.EXPLOSION_HUGE,
                    transform.position + Vector3.up * 0.2f,
                    Quaternion.LookRotation(-transform.forward),
                    Vector3.one * 1.2f,
                    true, 1.0f);

        EnemyManager.Instance.SplashAttackToZombie(transform.position, 4.0f, m_Stat.Attack, m_Stat.isKnockBack);

        m_Time = 0.0f;
        pushToMemory();
    }


    public override void CollisionEvent(GameObject _object)
    {
    }
}
