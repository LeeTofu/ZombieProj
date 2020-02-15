using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBomb : Bullet
{
    AudioSource m_AuidoSource;
    float m_Time = 0.0f;



    public override void InGame_Initialize()
    {

    }

    public override void Initialize(GameObject _model, MoveController _Controller,int _typeKey)
    {
        base.Initialize(_model, _Controller, _typeKey);

        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<BulletCollisionAction>();
    }

    protected override void BulletMove()
    {
        transform.position += (m_CurDirection * Time.deltaTime * m_Stat.MoveSpeed);
    }

    protected override void BulletOverRangefunction()
    {
        m_currentSpeed = 0.0f;

        EffectManager.Instance.PlayEffect(
            PARTICLE_TYPE.EXPLOSION_MEDIUM,
            transform.position,
            Quaternion.LookRotation(-transform.forward),
            Vector3.one * 1.4f,
            true, 1.0f);

        BattleSceneMain.CreateFireArea(transform.position, Quaternion.identity);

        EnemyManager.Instance.SplashAttackToZombie(transform.position, 4.0f, m_Stat.Attack, m_Stat.isKnockBack);

        pushToMemory();
    }


    public override void CollisionEvent(GameObject _object)
    {
        m_currentSpeed = 0.0f;

        EffectManager.Instance.PlayEffect(
            PARTICLE_TYPE.EXPLOSION_MEDIUM,
            transform.position,
            Quaternion.LookRotation(-transform.forward),
            Vector3.one * 1.4f,
            true, 1.0f);

        Debug.LogWarning("틀");

        BattleSceneMain.CreateFireArea(transform.position, Quaternion.identity);

        EnemyManager.Instance.SplashAttackToZombie(transform.position, 4.0f, m_Stat.Attack, m_Stat.isKnockBack);

        pushToMemory();
    }
}
