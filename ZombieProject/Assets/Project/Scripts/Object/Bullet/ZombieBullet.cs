﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBullet : Bullet
{
    Ray ray = new Ray();
    RaycastHit rayCast;

    public override void InGame_Initialize()
    {
        if (m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<BulletCollisionAction>();
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
        PARTICLE_TYPE.DUST,
        transform.position,
        Quaternion.LookRotation(-m_CurDirection),
        Vector3.one * 1.0f,
        true, 1.0f);

        pushToMemory();
    }

    public override void CollisionEvent(GameObject _object)
    {

        if (_object.tag == "Player")
        {
            MovingObject player = _object.GetComponent<MovingObject>();
            player.HitDamage(m_Stat.Attack);
            EffectManager.Instance.PlayEffect(
                PARTICLE_TYPE.BLOOD, transform.position, Quaternion.LookRotation(-m_CurDirection),
                Vector3.one * 0.8f, true, 1.0f);
        }
        else
        {
            EffectManager.Instance.PlayEffect(
                PARTICLE_TYPE.BLOOD,
                transform.position,
                Quaternion.LookRotation(-m_CurDirection),
                 Vector3.one * 1.0f,
                true, 1.0f);
        }

        pushToMemory();
    }

}
