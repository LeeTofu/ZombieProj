
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
    TMPro.TextMeshProUGUI m_ItemHp;
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemHpRegen;
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemMovePoint;
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemAttackSpeed;
    [SerializeField]
    TMPro.TextMeshProUGUI m_ItemRange;

    public void Initialize(ItemSlot _slot)
    {
        m_ItemName.text = _slot.m_Item.m_ItemStat.m_ItemName;
        m_ItemLevel.text = _slot.m_Item.m_Lv.ToString();
        m_ItemAttack.text = _slot.m_Item.m_ItemStat.m_AttackPoint.ToString();
        m_ItemArmor.text = _slot.m_Item.m_ItemStat.m_ArmorPoint.ToString();
        m_ItemHp.text = _slot.m_Item.m_ItemStat.m_HealthPoint.ToString();
        m_ItemHpRegen.text = _slot.m_Item.m_ItemStat.m_HPGenerator.ToString();
        m_ItemMovePoint.text = _slot.m_Item.m_ItemStat.m_MoveSpeed.ToString();
        m_ItemAttackSpeed.text = _slot.m_Item.m_ItemStat.m_AttackSpeed.ToString();
        m_ItemRange.text = _slot.m_Item.m_ItemStat.m_Range.ToString();
    }



}
