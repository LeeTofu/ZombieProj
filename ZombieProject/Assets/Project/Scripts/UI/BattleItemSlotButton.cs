using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleItemSlotButton : MonoBehaviour
{
    public ITEM_SLOT_SORT m_slotType { private set; get; }

    [SerializeField]
    Image m_ItemIcon;

    System.Action<Vector3, Vector3, MovingObject> m_ButtonClickAction;

    public void SetItem(Item _item)
    {
        if (_item == null)
        {
            m_ItemIcon.color = Color.clear;
            return;
        }

        m_slotType = _item.m_ItemSlotType;
        m_ItemIcon.sprite = TextureManager.Instance.GetItemIcon(_item.m_ItemStat.m_IconTexrureID);
        m_ItemIcon.color = Color.white;
    }

    public void ButtonClick()
    {
        if (!InvenManager.Instance.m_EquipedItemSlots.ContainsKey(m_slotType)) return;

        Item currentEquipedItem = InvenManager.Instance.m_EquipedItemSlots[m_slotType];

        if (currentEquipedItem != null)
        {
            currentEquipedItem.Attack(
                PlayerManager.Instance.m_Player.transform.position + Vector3.up * 0.3f,
               PlayerManager.Instance.m_Player.transform.forward,
                PlayerManager.Instance.m_Player);
        }
    }


}

