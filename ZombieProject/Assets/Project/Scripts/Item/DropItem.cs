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

    ParticleSystem m_DropItemParticle;

    AudioSource m_audioSource;


    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        AddCollisionCondtion(CollisionCondition);
        AddCollisionFunction(CollisionEvent);

        m_audioSource = GetComponent<AudioSource>();

        if (m_DropItemParticle == null)
        {
            GameObject go = Resources.Load<GameObject>("Prefabs/Effect&Particle/DropItemEffect");
            GameObject effect = Instantiate(go);
            m_DropItemParticle = effect.GetComponent<ParticleSystem>();

            m_DropItemParticle.Play();
            m_DropItemParticle.transform.SetParent(transform);
            m_DropItemParticle.transform.localPosition = Vector3.zero;
        }
    }

    void CollisionEvent(GameObject _object)
    {
        if (!GetComponentInChildren<MeshRenderer>().enabled) return;

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
        GameObject go = Resources.Load<GameObject>("Prefabs/Effect&Particle/BuffEffect");
        GameObject newEffect = Instantiate(go);
        newEffect.transform.position = transform.position;

        player.AddBuff(m_Buff);

        GetComponentInChildren<MeshRenderer>().enabled = false;
        m_DropItemParticle.gameObject.SetActive(false);
    }

    bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (_defender.tag != "Player") return false;

        return true;
    }
}
