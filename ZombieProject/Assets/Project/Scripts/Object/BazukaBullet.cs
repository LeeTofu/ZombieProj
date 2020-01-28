using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazukaBullet : Bullet
{
    public override void InGame_Initialize()
    {
       
    }

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        base.Initialize(_model, _Controller);

        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<BulletCollisionAction>();
    }

    protected override void BulletMove()
    {
  
        transform.position += (transform.forward * Time.deltaTime * m_Stat.MoveSpeed);

       // Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
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

        pushToMemory((int)m_BulletType);
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

        SplashAttack(transform.position, 4.0f);
       
        pushToMemory((int)m_BulletType);
    }
}
