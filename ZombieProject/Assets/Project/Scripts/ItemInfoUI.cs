
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoUI : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemName;
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemLevel;
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemAttack;
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemArmor;
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemAttackSpeed;
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemRange;

    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemCoolTime;
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemStackCount;

    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemInfo;

    public void Initialize(ItemSlot _slot)
    {
        m_ItemName.text = _slot.m_Item.m_ItemStat.m_ItemName;
        m_ItemLevel.text = _slot.m_Item.m_Lv.ToString();

        string info;

        switch (_slot.m_Item.m_ItemStat.m_Sort)
        {
            case ITEM_SORT.SHOT_GUN:
            case ITEM_SORT.LAUNCHER:
            case ITEM_SORT.SNIPER:
            case ITEM_SORT.RIFLE:
            case ITEM_SORT.MACHINE_GUN:
                m_ItemAttack.text = _slot.m_Item.m_ItemStat.m_AttackPoint.ToString();
                m_ItemArmor.text = _slot.m_Item.m_ItemStat.m_ArmorPoint.ToString();
                m_ItemAttackSpeed.text = _slot.m_Item.m_ItemStat.m_AttackSpeed.ToString() + "초";
                m_ItemRange.text = _slot.m_Item.m_ItemStat.m_Range.ToString();

                m_ItemCoolTime.text = (0).ToString();
                m_ItemStackCount.text = _slot.m_Item.m_ItemStat.m_Count.ToString();

                m_ItemInfo.text = _slot.m_Item.m_ItemStat.m_Info.ToString(); 

                break;
            case ITEM_SORT.HEALTH_PACK:
            case ITEM_SORT.BUFF:
                m_ItemAttack.text = " ";
                m_ItemArmor.text = " ";
                m_ItemAttackSpeed.text = " ";
                m_ItemRange.text = " ";

                m_ItemCoolTime.text = _slot.m_Item.m_ItemStat.m_CoolTime.ToString();
                m_ItemStackCount.text = _slot.m_Item.m_ItemStat.m_Count.ToString();

                m_ItemInfo.text = _slot.m_Item.m_ItemStat.m_Info.ToString();
                break;
            case ITEM_SORT.GRENADE:
            case ITEM_SORT.FIRE_GRENADE:
            case ITEM_SORT.INSTALL_BOMB:
                m_ItemAttack.text = _slot.m_Item.m_ItemStat.m_AttackPoint.ToString();
                m_ItemArmor.text = " ";
                m_ItemAttackSpeed.text = " ";
                m_ItemRange.text = _slot.m_Item.m_ItemStat.m_Range.ToString();

                m_ItemCoolTime.text = _slot.m_Item.m_ItemStat.m_CoolTime.ToString();
                m_ItemStackCount.text = _slot.m_Item.m_ItemStat.m_Count.ToString();

                m_ItemInfo.text = _slot.m_Item.m_ItemStat.m_Info.ToString();

                break;
        }

       

    }



}
