using UnityEngine;
using System.Collections;

public class DrawRange : MonoBehaviour
{
    public float ThetaScale = 0.04f;
    private int Size;
    private LineRenderer m_LineRenderer;
    private float Theta = 0f;

    private float m_Range;

    void Awake()
    {
        m_LineRenderer = gameObject.GetComponent<LineRenderer>();

        if (m_LineRenderer == null)
        {
            m_LineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        Size = (int)((1f / ThetaScale) + 1f);
        m_LineRenderer.positionCount = Size;

        m_LineRenderer.startWidth = 0.1f;
        m_LineRenderer.endWidth = 0.1f;

    }

    public void SetRangeCircle(float _range)
    {
        m_Range = _range;
        Theta = 0f;

        for (int i = 0; i < Size; i++)
        {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = transform.position.x + m_Range * Mathf.Cos(Theta);
            float y = transform.position.z + m_Range * Mathf.Sin(Theta);
            m_LineRenderer.SetPosition(i, new Vector3(x, transform.position.y, y));
        }
    }

}