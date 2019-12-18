using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleItemSlotButton : MonoBehaviour
{
    float m_CoolTime = 0.0f;

    public System.Action<Vector3, Vector3, MovingObject> m_UseMethod;
    public ITEM_SLOT_SORT m_slotType { private set; get; }

    [SerializeField]
    Image m_ItemIcon;

    public void SetItem(Item _item)
    {
        if (_item == null)
        {
            m_ItemIcon.color = Color.clear;
            return;
        }

        m_UseMethod = _item.m_AttackMethod;
        m_slotType = _item.m_ItemSlotType;
        m_ItemIcon.sprite = TextureManager.Instance.GetItemIcon(_item.m_ItemStat.m_IconTexrureID);
        m_ItemIcon.color = Color.white;
    }

    public void ButtonClick()
    {
        if (m_UseMethod == null) return;

        m_UseMethod(
            PlayerManager.Instance.m_Player.m_CurrentItemObject.m_FireTransform.position,
            PlayerManager.Instance.m_Player.m_CurrentItemObject.m_FireTransform.forward,
            PlayerManager.Instance.m_Player);
    }


}

