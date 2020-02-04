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

    public override void InGame_Initialize()
    {
        if (m_Time > 0.01f)
        {
            Invoke("TimePushToMemoty", m_Time);
        }
    }

    void TimePushToMemoty()
    {
        pushToMemory((int)m_Type);
    }

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
      //  m_audioSource = GetComponent<AudioSource>();

        if(m_CollisionAction == null)
            m_CollisionAction = gameObject.AddComponent<DeBuffAreaCollisionAction>();
    }

    public void ApplyBuff(MovingObject _object)
    {
        if (_object == null) return;
      //  Debug.Log("Buff 걸림" + m_BuffType);
        BuffManager.Instance.ApplyBuff(m_BuffType, _object, m_Level);

        if (m_isInstance)
        {
            pushToMemory((int)m_Type);
        }
    }
}
