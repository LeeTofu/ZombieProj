using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_BuffText;
    public TMPro.TextMeshProUGUI m_TimeText;
    public Image m_BuffImage;
    public Buff m_Buff;

    public void Initialize()
    {
        m_BuffText = transform.Find("BuffText").GetComponent<TMPro.TextMeshProUGUI>();
        m_TimeText = transform.Find("TimeText").GetComponent<TMPro.TextMeshProUGUI>();
        m_BuffImage = transform.Find("BuffImage").GetComponent<Image>();
    }

    public void Update()
    {
        if(m_Buff!=null)
        {
            m_TimeText.text = m_Buff.GetTimeRemaining().ToString();
        }
    }
}
