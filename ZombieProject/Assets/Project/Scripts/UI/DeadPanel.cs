using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadPanel : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI m_WaveText;

   public void UpdateDead()
    {
        m_WaveText.text = RespawnManager.Instance.m_CurWave.ToString();
    }



}
