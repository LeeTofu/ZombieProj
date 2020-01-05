using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PARTICLE_TYPE
{
    DUST,

    EXPLOSION_SMALL,
    EXPLOSION_MEDIUM,
    EXPLOSION_HUGE,

    BLOOD,

    BUFF,
    DEBUFF,

    DROP_ITEM,

    NONE
}

public class EffectManager : Singleton<EffectManager>
{
    ObjectFactory m_EffectFactory;
    Dictionary<PARTICLE_TYPE, List<GameObject>> m_ParticleTable = new Dictionary<PARTICLE_TYPE, List<GameObject>>();

    public override bool Initialize()
    {
        if(m_EffectFactory == null)
         m_EffectFactory = gameObject.AddComponent<ObjectFactory>();

        m_EffectFactory.Initialize("Prefabs/Effect&Particle/EffectPrefab", Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/Blood"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.BLOOD, 15);

        m_EffectFactory.Initialize("Prefabs/Effect&Particle/EffectPrefab", Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/BuffEffect"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.BUFF, 5);

        m_EffectFactory.Initialize("Prefabs/Effect&Particle/EffectPrefab", Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/DropItemEffect"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.DROP_ITEM, 15);

        return true;
    }

    public EffectObject PlayEffect(PARTICLE_TYPE _particleType, Vector3 _pos, Quaternion _quat, bool _isDestroy = false, float _DestroyTime = 0.0f )
    {
        if (m_EffectFactory == null) return null;

        EffectObject effect = m_EffectFactory.GetObjectFromFactory(_pos, _quat, (int)_particleType) as EffectObject;

        if(!effect)
        {
            Debug.LogError(" 파티클 타입 " + _particleType);
            return null;
        }

        if(_isDestroy)
            effect.SetDestroyTime(_DestroyTime, (int)_particleType);

        return effect;
    }
}
