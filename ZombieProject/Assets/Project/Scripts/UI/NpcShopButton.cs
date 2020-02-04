using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SHOP_SORT
{
    NONE = 0,
    AMMO,
    RANGEUP,
    ATTACKUP,
    ATTACKSPEEDUP,
    END
}
public class NpcShopButton : UIPressSubject
{
    public SHOP_SORT m_slotType;
    [SerializeField]
    TMPro.TextMeshProUGUI m_StackCountText;
    public override void OnPointerDown(PointerEventData eventData)
    {
        switch(m_slotType)
        {
            case SHOP_SORT.AMMO:
                int current = PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_Count;
                int max = PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_Count;
                PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.plusItem(max - current);
                m_StackCountText.text = PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_Count.ToString();
                break;
            case SHOP_SORT.RANGEUP:
                float currentrange = PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_Range;
                PlayerManager.Instance.CurrentEquipedWeaponUpgrade(UPGRADE_TYPE.RANGE, currentrange*0.1f);
                break;
            case SHOP_SORT.ATTACKUP:
                float currentattack = PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_AttackPoint;
                PlayerManager.Instance.CurrentEquipedWeaponUpgrade(UPGRADE_TYPE.ATTACK, currentattack * 0.1f);
                break;
            case SHOP_SORT.ATTACKSPEEDUP:
                float currentattackspeed = PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_AttackSpeed;
                PlayerManager.Instance.CurrentEquipedWeaponUpgrade(UPGRADE_TYPE.ATTACK_SPEED, -currentattackspeed * 0.1f);
                break;
        }
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
    }
    public override void OnPressed()
    {
    }
}
