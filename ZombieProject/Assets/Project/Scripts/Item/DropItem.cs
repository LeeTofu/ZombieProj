using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eDROP_ITEM
{
    ADRENALIN, // 공속,이속 증가
    HEALTH, // 체력 회복
    POISON // 독
}

public class DropItem : MovingObject
{
    [SerializeField]
    eDROP_ITEM m_dropItem;

    // 이 아이템을 먹으면 주는 버프
    Buff m_Buff;
    EffectObject m_EffectObject;
    // ParticleSystem m_DropItemParticle;

    AudioSource m_audioSource;

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        AddCollisionCondtion(CollisionCondition);
        AddCollisionFunction(CollisionEvent);

        m_audioSource = GetComponent<AudioSource>();
    }

    void CollisionEvent(GameObject _object)
    {
        MovingObject player = _object.GetComponent<MovingObject>();

        switch (m_dropItem)
        {
            case eDROP_ITEM.ADRENALIN:
                {
                    m_Buff = new Adrenaline(player.m_Stat);
                }
                break;
            case eDROP_ITEM.HEALTH:
                {
                    m_Buff = new Blessing(player.m_Stat);

                }
                break;
            case eDROP_ITEM.POISON:
                {
                    m_Buff = new Poison(player.m_Stat);

                }
                break;
        }

        m_audioSource.Play();
        EffectManager.Instance.PlayEffect(PARTICLE_TYPE.BUFF, transform.position, Quaternion.identity, true, 1.0f);
        player.AddBuff(m_Buff);

        pushToMemory((int)m_Type);
    }

    bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (_defender.tag != "Player") return false;

        return true;
    }

    private void OnEnable()
    {
        m_EffectObject =  EffectManager.Instance.PlayEffect(PARTICLE_TYPE.DROP_ITEM, transform.position, Quaternion.Euler(0, 0, 0));
    }

    private void OnDisable()
    {
        if (m_EffectObject != null)
            m_EffectObject.SetDestroyTime(0.0f, (int)PARTICLE_TYPE.DROP_ITEM);
    }

}
