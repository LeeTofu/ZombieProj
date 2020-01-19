using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinkleTextUI : MonoBehaviour
{
    string m_StartString;

    [SerializeField]
    float m_Duration;

    [SerializeField]
    float m_tickTime;

    [SerializeField]
    TMPro.TextMeshProUGUI m_Text;

    Coroutine m_Couroutine;

    private void Awake()
    {
        m_Couroutine = null;
    }

    public void StartTextUITwinkle()
    { 
        if (m_Couroutine != null)
            StopCoroutine(m_Couroutine);

        m_Couroutine = StartCoroutine(Text_C(m_Duration));
    }

    private void OnDisable()
    {
        m_Text.gameObject.SetActive(false);
    }

    IEnumerator Text_C(float _durationTime)
    {
        yield return new WaitForSeconds(_durationTime);

        gameObject.SetActive(false);

    }

}
