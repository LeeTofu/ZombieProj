using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 m_CurVelocity { get; private set; }
    public Vector3 m_CurDirection { get; private set; }

    public float m_Speed { get; private set; }

    public bool m_isFire { get; private set; }

    [SerializeField]
    TrailRenderer m_TrailRenderer;

    public bool m_isArc { get; private set; }

    public void FireBullet(Vector3 _pos, Vector3 _dir, float _speed)
    {
        transform.position = _pos;
         m_CurDirection = _dir;
        m_isFire = true;
        m_Speed = _speed;

        if (m_TrailRenderer == null)
            m_TrailRenderer = GetComponent<TrailRenderer>();

        m_TrailRenderer.enabled = true;
        m_TrailRenderer.startWidth = 0.18f;
        m_TrailRenderer.endWidth = 0.05f;

        m_TrailRenderer.time = 0.25f;

        m_isArc = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(m_CurDirection * Time.deltaTime * m_Speed);
    }
}
