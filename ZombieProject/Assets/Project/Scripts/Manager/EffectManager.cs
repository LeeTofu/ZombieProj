using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PARTICLE_TYPE
{
    DUST, // 작은 먼지

    EXPLOSION_SMALL, // 폭발 작은거
    EXPLOSION_MEDIUM,
    EXPLOSION_HUGE,
    BULLET_EXPLOSION, // 총알이 뭔가에 부딫혀서 폭발

    TOUCH_EFFECT, // 터치 이펙트

    BLOOD, // 피

    BUFF, //버프 이펙트
    DEBUFF, // 디버프 이펙트

    DROP_ITEM, // 드랍 아이템 표시

    MUZZLE, // 총알 발사
    HEAL,
    ADRENALIN,

    PLAYER, // 플레이어
    ENMETY_FOCUS, // 타겟팅된 적

    POISON, // 독 이펙트
    FIRE, // 불 이펙트

    NONE
}

public class EffectManager : Singleton<EffectManager>
{
    ObjectFactory m_EffectFactory;
    Dictionary<PARTICLE_TYPE, List<GameObject>> m_ParticleTable = new Dictionary<PARTICLE_TYPE, List<GameObject>>();
    const string m_PrefabPath = "Prefabs/Effect&Particle/EffectPrefab";
    const string m_UIPrefabPath = "Prefabs/Effect&Particle/UIEffectPrefab2";

    public override bool Initialize()
    {
        if(m_EffectFactory == null)
         m_EffectFactory = gameObject.AddComponent<ObjectFactory>();

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/Blood"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.BLOOD, 15);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/BuffEffect"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.BUFF, 5);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/DropItemEffect"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.DROP_ITEM, 15);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/Dust"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.DUST, 15);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/BulletMuzzle"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.MUZZLE, 15);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/BulletExplosion"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.BULLET_EXPLOSION, 10);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/ExplosionMedium"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.EXPLOSION_MEDIUM, 10);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/Heal"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.HEAL, 5);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/Poison"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.POISON, 10);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/Fire"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.FIRE, 10);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/Adrenalin"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.ADRENALIN, 5);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/Player"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.PLAYER, 3);

        m_EffectFactory.Initialize(m_PrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/Enemy_Focus"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.ENMETY_FOCUS, 3);

        m_EffectFactory.Initialize(m_UIPrefabPath, Resources.LoadAll<GameObject>("Prefabs/Effect&Particle/EffectModel/TouchEffect"));
        m_EffectFactory.CreateObjectPool((int)PARTICLE_TYPE.TOUCH_EFFECT, 5);

        return true;
    }
    public override void DestroyManager()
    {
        foreach (EffectObject obj in m_EffectFactory.m_ListAllMovingObject)
        {
            obj.pushToMemory((int)obj.m_EffectTypeID);
        }
    }
    public EffectObject PlayEffect(PARTICLE_TYPE _particleType, Vector3 _pos, Quaternion _quat, Vector3 _scale, bool _isDestroy = false, float _DestroyTime = 0.0f )
    {
        if (m_EffectFactory == null) return null;

        EffectObject effect = m_EffectFactory.PopObject(_pos, _quat, (int)_particleType) as EffectObject;

        if(!effect)
        {
            Debug.LogError(" 파티클 타입 " + _particleType);
            return null;
        }

        if(_isDestroy)
            effect.SetDestroyTime(_DestroyTime, (int)_particleType);

        effect.transform.localScale = _scale;
        return effect;
    }

    public EffectObject AttachEffect(PARTICLE_TYPE _particleType, MovingObject _object, Vector3 _offset, Quaternion _quat, Vector3 _scale, bool _isDestroy = false, float _DestroyTime = 0.0f)
    {
        if (_object == null) return null;
        if (m_EffectFactory == null) return null;

        EffectObject effect = m_EffectFactory.PopObject(_object.transform.position, _quat, (int)_particleType) as EffectObject;

        if (!effect)
        {
            Debug.LogError(" 파티클 타입 " + _particleType);
            return null;
        }

        if (_isDestroy)
            effect.SetDestroyTime(_DestroyTime, (int)_particleType);

        effect.transform.localScale = _scale;
        effect.transform.SetParent(_object.transform);
        effect.transform.localPosition = _offset;

        return effect;
    }
}
