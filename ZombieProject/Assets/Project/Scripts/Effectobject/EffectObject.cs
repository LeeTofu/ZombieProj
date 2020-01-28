using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MovingObject
{
    ParticleSystem m_ParticleSystem;
    Coroutine m_Coroutine;

    int m_EffectTypeID;

    public override void InGame_Initialize()
    {
        if (m_ParticleSystem != null)
        {
            m_ParticleSystem.Play();
        }
    }

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        if(m_ParticleSystem == null)
        {
            m_ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }
    }

    public void SetDestroyTime(float _time, int _type)
    {
        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);

        m_EffectTypeID = _type;

        m_Coroutine = StartCoroutine(DestoryTime_C(_time, m_EffectTypeID));
    }

    IEnumerator DestoryTime_C(float _time, int  _type)
    {
        yield return new WaitForSeconds(_time);
        pushToMemory(m_EffectTypeID);
    }


}
