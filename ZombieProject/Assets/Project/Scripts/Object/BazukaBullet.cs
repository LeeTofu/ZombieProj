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
    }

    protected override void BulletMove()
    {
        m_currentMoveDistance += Time.deltaTime * 2.0f;
        m_currentSpeed = Mathf.Lerp(m_currentSpeed, m_Stat.MoveSpeed, Time.deltaTime * 2.0f);

        transform.position += (transform.forward * Time.deltaTime * m_currentSpeed);

        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
    }


    protected override void CollisionEvent(GameObject _object)
    {
        m_currentSpeed = 0.0f;

        EffectManager.Instance.PlayEffect(
            PARTICLE_TYPE.EXPLOSION_MEDIUM, 
            transform.position, 
            Quaternion.LookRotation(-transform.forward), 
            Vector3.one * 1.4f,
            true, 1.0f);

        SplashAttack(transform.position);
       
        pushToMemory((int)m_BulletType);
    }
}
