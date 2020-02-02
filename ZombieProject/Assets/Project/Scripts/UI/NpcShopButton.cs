using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NpcShopButton : UIPressSubject
{
    [SerializeField]
    TMPro.TextMeshProUGUI m_StackCountText;
    public override void OnPointerDown(PointerEventData eventData)
    {
        int current = PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_Count;
        int max = PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_Count;

        PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.plusItem(max - current);
        m_StackCountText.text = PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_Count.ToString();
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
    }
    public override void OnPressed()
    {
    }
}
