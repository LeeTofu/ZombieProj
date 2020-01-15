using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceBullet : Bullet
{
    public override void InGame_Initialize()
    {
        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<BulletCollisionAction>();
    }

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        base.Initialize(_model, _Controller);

        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<BulletCollisionAction>();
    }

    protected override void BulletMove()
    {
        transform.position += (m_CurDirection * Time.deltaTime * m_Stat.MoveSpeed);
    }

    public override void CollisionEvent(GameObject _object)
    {
        if (_object.tag == "Zombie")
        {
            MovingObject zombie = _object.GetComponent<MovingObject>();
            zombie.HitDamage(m_Stat.Attack, m_Stat.isKnockBack, 1.0f);
            EffectManager.Instance.PlayEffect(
                PARTICLE_TYPE.BLOOD, transform.position, Quaternion.LookRotation(-m_CurDirection),
                Vector3.one * 0.8f, true, 1.0f);
        }
        else
        {
            EffectManager.Instance.PlayEffect(
                PARTICLE_TYPE.BULLET_EXPLOSION,
                transform.position,
                Quaternion.LookRotation(-m_CurDirection),
                 Vector3.one * 1.0f,
                true, 1.0f);

            pushToMemory((int)m_BulletType);
        }
    }
}
