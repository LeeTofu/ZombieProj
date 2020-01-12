using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MovingObject
{
    [SerializeField]
    ParticleSystem m_TrailEffect;

    public Vector3 m_CurVelocity { get; private set; }
    public Vector3 m_CurDirection { get; private set; }

    public bool m_isFire { get; private set; }

    [SerializeField]
    TrailRenderer m_TrailRenderer;

    [SerializeField]
    GameObject m_PointLight;

    public AudioClip[] m_GunSound;
    public BULLET_TYPE m_BulletType;

    protected float m_currentMoveDistance = 0.0f;
    protected float m_currentSpeed = 0.0f;

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        AddCollisionCondtion(CollisionCondition);
        AddCollisionFunction(CollisionEvent);
    }

    protected void SplashAttack(Vector3 _pos)
    {
        var zombies = EnemyManager.Instance.GetRangeZombies(_pos, 2.0f);

        foreach (MovingObject zombie in zombies)
        {
            zombie.HitDamage(m_Stat.Attack, m_Stat.isKnockBack, 1.0f);
        }
    }

    protected abstract void BulletMove();

    protected abstract void CollisionEvent(GameObject _object);

    protected bool CollisionCondition(GameObject _defender)
    {
        if (_defender.tag == "Zombie" || _defender.tag == "Wall")  return true;
        
        return false;
    }

    public void FireBullet(Vector3 _pos, Vector3 _dir, STAT _stat )
    {
        transform.position = _pos;
        transform.forward = _dir;

        if(m_TrailEffect != null)
        {
            m_TrailEffect = transform.GetComponentInChildren<ParticleSystem>();
            m_TrailEffect.Play();
        }

        m_currentSpeed = 0.0f;
        m_currentMoveDistance = 0.0f;

        m_CurDirection = _dir;
        m_isFire = true;

        SetStat(_stat);

        if (m_TrailRenderer == null)
            m_TrailRenderer = GetComponent<TrailRenderer>();

        if (m_TrailRenderer)
        {
            m_TrailRenderer.enabled = true;
            m_TrailRenderer.startWidth = 0.18f;
            m_TrailRenderer.endWidth = 0.05f;
            m_TrailRenderer.time = 0.25f;
        }

        m_PointLight.SetActive(true);

    }


    public void FireBullet(Vector3 _pos, Vector3 _dir, ItemStat _itemStat)
    {
        FireBullet(_pos, _dir, new STAT
        {
            MaxHP = 100f,
            CurHP = 100f,
            Defend = 100f,
            MoveSpeed = _itemStat.m_BulletSpeed,
            Attack = _itemStat.m_AttackPoint,
            Range = _itemStat.m_Range,
            isKnockBack = _itemStat.m_isKnockBack
        }) ;
    }

    protected void Update()
    {
        BulletMove();
    }
}
