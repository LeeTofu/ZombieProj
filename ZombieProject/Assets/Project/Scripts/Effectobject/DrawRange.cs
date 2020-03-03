using UnityEngine;
using System.Collections;
using UnityEditor;

public class DrawRange : MonoBehaviour
{
    private int m_CircularSectorSize;

    private int m_CircleSize;
    private LineRenderer m_LineRenderer;
    private float m_DegInterval = 0f;

    private float m_Range;

    Coroutine m_RenderCoroutine;

    void Awake()
    {
        m_LineRenderer = gameObject.GetComponent<LineRenderer>();

        if (m_LineRenderer == null)
        {
            m_LineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        m_CircleSize = 14;
        m_CircularSectorSize = 14;
        m_LineRenderer.positionCount = m_CircularSectorSize;

        m_DegInterval = 360.0f / m_CircleSize;

        m_LineRenderer.startWidth = 0.1f;
        m_LineRenderer.endWidth = 0.1f;

    }

    public void StartRenderCircle(float _range, Vector3 _pos)
    {
        if (m_RenderCoroutine != null)
            StopCoroutine(m_RenderCoroutine);

        m_RenderCoroutine = StartCoroutine(RenderCirlce_C(_range, _pos));
    }

    public void StartRenderCircleSector(float _range, Vector3 _pos, float _startAngle, float _angle)
    {
        if (m_RenderCoroutine != null)
            StopCoroutine(m_RenderCoroutine);

       // m_RenderCoroutine = StartCoroutine(RenderCirlceSector_C(_range, _pos, _startAngle, _angle));
    }

    IEnumerator RenderCirlce_C(float _range, Vector3 _pos)
    {
        m_Range = _range;

        float Deg = 0;

        for (int i = 0; i < m_CircleSize; i++)
        {
            Deg += m_DegInterval;

            float x = _pos.x + m_Range * Mathf.Cos(Mathf.Deg2Rad * Deg);
            float y = _pos.z + m_Range * Mathf.Sin(Mathf.Deg2Rad * Deg);

            m_LineRenderer.SetPosition(i, new Vector3(x, _pos.y, y));

            yield return null;
        }
    }

 


    public void RenderCircle(float _range, Vector3 _pos)
    {
        m_Range = _range;

        float Deg = 0;

        for (int i = 0; i < m_CircleSize; i++)
        {
            Deg += m_DegInterval;

            float x = _pos.x + m_Range * Mathf.Cos(Mathf.Deg2Rad * Deg);
            float y = _pos.z + m_Range * Mathf.Sin(Mathf.Deg2Rad * Deg);

            m_LineRenderer.SetPosition(i, new Vector3(x, _pos.y, y));
        }
    }

    public void RenderCircleSector(float _range, Vector3 _pos, float _startAngle, float _angle)
    {
        m_Range = _range;

        float Deg = _startAngle + (_angle ) * 1.5f;

        m_LineRenderer.SetPosition(0, _pos);

        for (int i = 1; i < m_CircularSectorSize; i++)
        {
            float x = _pos.x + m_Range * Mathf.Cos( Mathf.Deg2Rad * Deg);
            float y = _pos.z + m_Range * Mathf.Sin(Mathf.Deg2Rad * Deg);
            
            m_LineRenderer.SetPosition(i, new Vector3(x, _pos.y, y));

            Deg += (_angle / m_CircularSectorSize);

        }

        m_LineRenderer.SetPosition(m_CircularSectorSize - 1, _pos);
    }


}