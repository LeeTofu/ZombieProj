using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DropItem : MovingObject
{
    [SerializeField]
    BUFF_TYPE m_BuffType;

    [SerializeField]
    int m_Level;

    // 이 아이템을 먹으면 주는 버프
    Buff m_Buff;
    EffectObject m_EffectObject;
    // ParticleSystem m_DropItemParticle;

    AudioSource m_audioSource;


    public override void InGame_Initialize()
    {
        if (m_EffectObject == null)
        {
            m_EffectObject = EffectManager.Instance.PlayEffect(PARTICLE_TYPE.DROP_ITEM, transform.position, Quaternion.Euler(-90.0f, 0, 0),
            Vector3.one * 1.0f);
        }
    }

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        m_audioSource = GetComponent<AudioSource>();

        if(m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<DropItemCollisionAction>();
    }

    public void ApplyBuff(MovingObject _object)
    {
        if (_object == null) return;

        Debug.Log("Buff 걸림" + m_BuffType);
        BuffManager.Instance.ApplyBuff(m_BuffType, _object, m_Level);

        m_audioSource.Play();
        EffectManager.Instance.PlayEffect(PARTICLE_TYPE.BUFF, transform.position, Quaternion.identity, Vector3.one * 1.2f, true, 1.0f);
      //  _object.AddBuff(m_Buff);

        pushToMemory((int)m_Type);
    }

    bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (_defender.tag != "Player") return false;

        return true;
    }


    private void OnDisable()
    {
        if (m_EffectObject != null)
            m_EffectObject.SetDestroyTime(0.0f, (int)PARTICLE_TYPE.DROP_ITEM);
    }

}
