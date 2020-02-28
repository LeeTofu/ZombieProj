using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeBuffArea : MovingObject
{
    [SerializeField]
    BUFF_TYPE m_BuffType;

    [SerializeField]
    int m_Level;

    //일회성인가?
    [SerializeField]
    bool m_isInstance;

    [SerializeField]
    float m_Time;

    [SerializeField]
    string[] m_TagArray;

    float m_DebuffDamage;


    public override void InGame_Initialize()
    {
        if (m_Time > 0.01f)
        {
            Invoke("TimePushToMemoty", m_Time);
        }

    }

    void TimePushToMemoty()
    {
        pushToMemory();
    }

    public override void Initialize(GameObject _model, MoveController _Controller, int _typeKey)
    {
        m_TypeKey = _typeKey;
        //  m_audioSource = GetComponent<AudioSource>();

        if (m_CollisionAction == null)
             m_CollisionAction = gameObject.AddComponent<DeBuffAreaCollisionAction>();

       for (int i = 0; i < m_TagArray.Length; i++)
       {
           m_CollisionAction.InsertCollisionTag(m_TagArray[i]);
       }
        
    }

    public void ApplyBuff(MovingObject _object)
    {
        if (_object == null) return;

        BuffManager.Instance.ApplyBuff(m_BuffType, _object, 1);

        if (m_isInstance)
        {
            pushToMemory();
        }
    }
}
