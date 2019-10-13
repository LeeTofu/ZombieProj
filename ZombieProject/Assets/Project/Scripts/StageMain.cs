using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STAGE
{
    REST_TIME = 1,

    STAGE_1,
    STAGE_2,
    STAGE_3,
    STAGE_4,
    STAGE_5,
    STAGE_BOSS,
    STAGE_END
};

public class StageMain : MonoBehaviour
{
    public STAGE m_CurrentStage;

    [Range(0.0f, 60.0f)]
    float m_Stage_Time;

    [HideInInspector]
    public float m_CurrentTime;

    private void Awake()
    {
        m_CurrentTime = 0.0f;

        StartCoroutine(C_TimeUpdate());
    }

    IEnumerator C_TimeUpdate()
    {
        while(m_CurrentTime < m_Stage_Time)
        {
            m_CurrentTime += Time.deltaTime;

            yield return null;

        }

        Debug.Log("시간 끝");
    }



}
