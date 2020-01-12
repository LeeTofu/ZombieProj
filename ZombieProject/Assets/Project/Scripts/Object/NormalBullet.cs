﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : Bullet
{


    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        base.Initialize(_model, _Controller);
    }

    protected override void BulletMove()
    {
        transform.position += (m_CurDirection * Time.deltaTime * m_Stat.MoveSpeed);
    }

    protected override void CollisionEvent(GameObject _object)
    {
        if (_object.tag == "Zombie")
        {
            MovingObject zombie = _object.GetComponent<MovingObject>();
            zombie.HitDamage(m_Stat.Attack, m_Stat.isKnockBack, 1.0f);
            EffectManager.Instance.PlayEffect(PARTICLE_TYPE.BLOOD, transform.position, Quaternion.LookRotation(-m_CurDirection), true, 1.0f);
        }
        else
            EffectManager.Instance.PlayEffect(PARTICLE_TYPE.DUST, transform.position, Quaternion.LookRotation(-m_CurDirection), true, 1.0f);

        pushToMemory((int)m_BulletType);
    }
}
