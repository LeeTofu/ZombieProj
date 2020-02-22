using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarUI : MonoBehaviour
{
    private Canvas m_HpUi;
    private Image m_HpImage;
    private Image m_HpBar;
    private Vector3 m_ScreenPos;
    private float m_Height;
    private Vector3 m_TargetPos;
    private MovingObject m_MovingObject;

    private void Awake()
    {
        m_MovingObject = GetComponent<MovingObject>();
        m_HpUi = transform.Find("HPUI").GetComponent<Canvas>();
        m_HpBar = transform.Find("HPUI/HPBar").GetComponent<Image>();
        m_HpImage = transform.Find("HPUI/HPBar/HP").GetComponent<Image>();
    }
    public void Initialzie(MovingObject _object)
    {
        if (m_MovingObject == null) m_MovingObject = _object;
        if (m_MovingObject.GetModel() != null) 
            m_Height = m_MovingObject.GetModel().transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Neck/Head").transform.position.y;
    }

    private void Update()
    {
        if (CameraManager.Instance.m_Camera != null)
        {
            m_TargetPos = m_MovingObject.transform.position;
            m_TargetPos.y += m_Height;
            m_ScreenPos = CameraManager.Instance.m_Camera.WorldToScreenPoint(m_TargetPos);
            m_HpBar.transform.position = new Vector3(m_ScreenPos.x, m_ScreenPos.y + 30f, 0);
        }

        if (PlayerManager.Instance.m_Player == null) return;
        if (PlayerManager.Instance.m_Player.m_Stat == null) return;
        if (PlayerManager.Instance.m_Player.m_Stat.CheckIsDead())
            m_HpUi.enabled = false;
    }
    public void HpChange()
    {
        m_HpImage.fillAmount = m_MovingObject.m_Stat.CurHP / m_MovingObject.m_Stat.MaxHP;
    }

    public void InGame_Initialize()
    {
        if (m_MovingObject == null) return;
        if (m_MovingObject.m_Stat == null) return;

        switch (m_MovingObject.tag)
        {
            case "Player":
                m_HpImage.fillAmount = 1f;
                m_HpUi.enabled = true;
                break;
            case "Zombie":
                m_HpImage.fillAmount = 1f;
                m_HpUi.enabled = false;
                m_MovingObject.m_Stat.AddPropertyChangeAction(() =>
                {
                    if (m_MovingObject.m_Stat.CurHP >= m_MovingObject.m_Stat.MaxHP)
                        m_HpUi.enabled = false;
                    else
                        m_HpUi.enabled = true;
                });
                break;
        }
        m_MovingObject.m_Stat.AddPropertyChangeAction(() =>
        {
            HpChange();
        });
    }
}
