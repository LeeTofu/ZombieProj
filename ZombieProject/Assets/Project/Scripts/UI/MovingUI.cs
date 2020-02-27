using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingUI : MonoBehaviour
{
    [SerializeField]
    GameObject m_Destination;

    [SerializeField]
    GameObject m_Start;

    float m_Time = 0.0f;

    bool m_isShowing;

    private void Awake()
    {
        m_isShowing = false;
        transform.localPosition = m_Start.transform.localPosition;
        m_Time = 0.0f;
    }

    private void OnEnable()
    {
        transform.localPosition = m_Start.transform.localPosition;
        m_Time = 0.0f;
        m_isShowing = true;
    }

    private void OnDisable()
    {
        transform.localPosition = m_Start.transform.localPosition;
        m_Time = 0.0f;
        m_isShowing = false;
    }

    private void Update()
    {
        if (m_isShowing)
        {
            transform.localPosition = Vector3.Lerp(m_Start.transform.localPosition, m_Destination.transform.localPosition, m_Time += Time.deltaTime * 0.2f);
            if(m_Time >= 1.0f)
            {
                m_isShowing = false;
            }
        }
    }



}
