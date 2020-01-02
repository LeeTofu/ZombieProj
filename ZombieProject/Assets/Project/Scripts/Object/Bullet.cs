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

    public bool m_isArc { get; private set; }

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        AddCollisionCondtion(CollisionCondition);
        AddCollisionFunction(CollisionEvent);
    }

    void CollisionEvent(GameObject _object)
    {
        Debug.Log("Zombie Check");
        MovingObject zombie = _object.GetComponent<MovingObject>();

        zombie.HitDamage(m_Stat.Attack);

        pushToMemory();
    }

    bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (_defender.tag != "Zombie") return false;

        return true;
    }

    public void FireBullet(Vector3 _pos, Vector3 _dir, Item _item)
    {
        transform.position = _pos;
         m_CurDirection = _dir;
        m_isFire = true;

        SetStat(new STAT
        {
            MaxHP = 100f,
            CurHP = 100f,
            Defend = 100f,
            MoveSpeed = _item.m_ItemStat.m_BulletSpeed,
            Attack = _item.m_ItemStat.m_AttackPoint,
            Range = _item.m_ItemStat.m_Range,
        }) ;
       
        if (m_TrailRenderer == null)
            m_TrailRenderer = GetComponent<TrailRenderer>();

        m_TrailRenderer.enabled = true;
        m_TrailRenderer.startWidth = 0.18f;
        m_TrailRenderer.endWidth = 0.05f;
        m_TrailRenderer.time = 0.25f;

        m_PointLight.SetActive(true);

        m_isArc = false;

        Invoke("pushToMemory", 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(m_CurDirection * Time.deltaTime * m_Stat.MoveSpeed);
    }
}
