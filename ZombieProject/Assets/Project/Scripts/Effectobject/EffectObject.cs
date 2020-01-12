using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MovingObject
{
    ParticleSystem m_ParticleSystem;
    Coroutine m_Coroutine;

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

        m_Coroutine = StartCoroutine(DestoryTime_C(_time, _type));
    }

    IEnumerator DestoryTime_C(float _time, int  _type)
    {
        yield return new WaitForSeconds(_time);

        pushToMemory(_type);
    }

}
