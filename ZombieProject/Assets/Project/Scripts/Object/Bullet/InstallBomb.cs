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

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        base.Initialize(_model, _Controller);

        m_Time = 0.0f;

        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<BulletCollisionAction>();

        if (m_AuidoSource == null)
            m_AuidoSource = gameObject.AddComponent<AudioSource>();
    }

    protected override void BulletMove()
    {
        m_Time += Time.deltaTime;

        if (m_Time > 0.5f)
        {
            EnemyManager.Instance.SetTargetingRangeZombies(this, transform.position, m_Stat.Range);
            SoundManager.Instance.OneShotPlay(UI_SOUND.INSTALL_BOMB);
        }
    }

    protected override void BulletOverRangefunction()
    {
        EffectManager.Instance.PlayEffect(
                    PARTICLE_TYPE.EXPLOSION_MEDIUM,
                    transform.position,
                    Quaternion.LookRotation(-transform.forward),
                    Vector3.one * 1.4f,
                    true, 1.0f);

        SplashAttack(transform.position, 4.0f);

        m_Time = 0.0f;
        pushToMemory((int)m_BulletType);
    }


    public override void CollisionEvent(GameObject _object)
    {
    }
}
