using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazukaBullet : Bullet
{
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

        m_currentSpeed = Mathf.Lerp(0, m_Stat.MoveSpeed, Time.deltaTime * m_Stat.MoveSpeed * 13.0f);
        transform.position += (transform.forward * Time.deltaTime * m_currentSpeed);

       // Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
    }

    protected override void BulletOverRangefunction()
    {
        EffectManager.Instance.PlayEffect(
                    PARTICLE_TYPE.EXPLOSION_HUGE,
                    transform.position + Vector3.up * 0.2f,
                    Quaternion.LookRotation(-transform.forward),
                    Vector3.one * 1.2f,
                    true, 1.0f);

        EnemyManager.Instance.SplashAttackToZombie(transform.position, 5.0f, m_Stat.Attack, m_Stat.isKnockBack);

        pushToMemory();
    }


    public override void CollisionEvent(GameObject _object)
    {
        m_currentSpeed = 0.0f;

        EffectManager.Instance.PlayEffect(
             PARTICLE_TYPE.EXPLOSION_HUGE, 
            transform.position, 
            Quaternion.LookRotation(-transform.forward), 
            Vector3.one * 1.4f,
            true, 1.0f);

        EnemyManager.Instance.SplashAttackToZombie(transform.position, 5.0f, m_Stat.Attack, m_Stat.isKnockBack);

        pushToMemory();
    }
}
