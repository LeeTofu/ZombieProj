using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MovingObject
{
    ParticleSystem m_ParticleSystem;
    Coroutine m_Coroutine;

    public MovingObject m_MovingObject;

    public override void InGame_Initialize()
    {
        if (m_ParticleSystem != null)
        {
            m_ParticleSystem.Play();
        }
    }


    public override void Initialize(GameObject _model, MoveController _Controller, int _typeKey)
    {
        m_TypeKey = _typeKey;
        if (m_ParticleSystem == null)
        {
            m_ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }
    }

    public void SetDestroyTime(float _time)
    {
        if (!gameObject.activeSelf) return;
        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);
        m_Coroutine = StartCoroutine(DestoryTime_C(_time));
    }

    IEnumerator DestoryTime_C(float _time)
    {
        yield return new WaitForSeconds(_time);
        pushToMemory();
    }


}
