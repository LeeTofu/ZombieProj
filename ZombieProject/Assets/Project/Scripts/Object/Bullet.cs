using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MovingObject
{
    [SerializeField]
    private ParticleSystem m_HitEffect;

    public Vector3 m_CurVelocity { get; private set; }
    public Vector3 m_CurDirection { get; private set; }

    public bool m_isFire { get; private set; }

    [SerializeField]
    TrailRenderer m_TrailRenderer;

    [SerializeField]
    GameObject m_PointLight;

    public AudioClip[] m_GunSound;

    BULLET_TYPE m_BulletType;


    public bool m_isArc { get; private set; }

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        AddCollisionCondtion(CollisionCondition);
        AddCollisionFunction(CollisionEvent);


    }

    void CollisionEvent(GameObject _object)
    {
        if (_object.tag == "Zombie")
        {
            MovingObject zombie = _object.GetComponent<MovingObject>();

            EffectManager.Instance.PlayEffect(PARTICLE_TYPE.BLOOD, transform.position, Quaternion.LookRotation(-m_CurDirection), true, 1.0f);

            zombie.HitDamage(100, true, 1.0f);

            pushToMemory((int)m_BulletType);
        }
        else if (_object.tag == "Wall")
        {
            EffectManager.Instance.PlayEffect(PARTICLE_TYPE.DUST, transform.position, Quaternion.LookRotation(-m_CurDirection), true, 1.0f);
            pushToMemory((int)m_BulletType);
        }
    }

    bool CollisionCondition(GameObject _defender)
    {
        if (_defender.tag == "Zombie" || _defender.tag == "Wall")  return true;
        
        return false;
    }

    public void FireBullet(Vector3 _pos, Vector3 _dir, ItemStat _itemStat)
    {
        transform.position = _pos;
         m_CurDirection = _dir;
        m_isFire = true;

        SetStat(new STAT
        {
            MaxHP = 100f,
            CurHP = 100f,
            Defend = 100f,
            MoveSpeed = _itemStat.m_BulletSpeed,
            Attack = _itemStat.m_AttackPoint,
            Range = _itemStat.m_Range,
        }) ;

        m_BulletType = BulletManager.Instance.GetBulletTypeFromItemStat(_itemStat);

        if (m_TrailRenderer == null)
            m_TrailRenderer = GetComponent<TrailRenderer>();

        m_TrailRenderer.enabled = true;
        m_TrailRenderer.startWidth = 0.18f;
        m_TrailRenderer.endWidth = 0.05f;
        m_TrailRenderer.time = 0.25f;

        m_PointLight.SetActive(true);

        m_isArc = false;

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(m_CurDirection * Time.deltaTime * m_Stat.MoveSpeed);
    }

    private void OnDisable()
    {
        
    }

}
