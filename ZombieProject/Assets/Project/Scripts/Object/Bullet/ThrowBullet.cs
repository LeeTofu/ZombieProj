using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBullet : Bullet
{
    Ray ray = new Ray();
    RaycastHit rayCast;
    Vector3 m_ReflectedVector;



    public override void InGame_Initialize()
    {
       
    }

    public override void Initialize(GameObject _model, MoveController _Controller, int _typeKey)
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
        EffectManager.Instance.PlayEffect(
                    PARTICLE_TYPE.EXPLOSION_MEDIUM,
                    transform.position,
                    Quaternion.LookRotation(-transform.forward),
                    Vector3.one * 1.4f,
                    true, 1.0f);

        EnemyManager.Instance.SplashAttackToZombie(transform.position, 4.0f, m_Stat.Attack, m_Stat.isKnockBack);

        pushToMemory();
    }


    public override void CollisionEvent(GameObject _object)
    {
        ray.origin = transform.position;
        ray.direction = m_CurDirection;

        if (Physics.Raycast(ray, out rayCast, 1.0f, 1 << LayerMask.NameToLayer("Wall")))
        {
            EffectManager.Instance.PlayEffect(PARTICLE_TYPE.HIT_EFFECT, rayCast.point, Quaternion.identity, Vector3.one * 1.2f, true, 0.5f);
            SoundManager.Instance.OneShotPlay(UI_SOUND.HIT_METAL);

            m_CurDirection = Vector3.Reflect(m_CurDirection, rayCast.normal);
        }

        if (_object.tag == "Zombie")
        {
            m_currentSpeed = 0.0f;

            EffectManager.Instance.PlayEffect(
                PARTICLE_TYPE.EXPLOSION_MEDIUM,
                transform.position,
                Quaternion.LookRotation(-transform.forward),
                Vector3.one * 1.4f,
                true, 1.0f);

            EnemyManager.Instance.SplashAttackToZombie(transform.position, 4.0f, m_Stat.Attack, m_Stat.isKnockBack);

            pushToMemory();
        }
    }
}
