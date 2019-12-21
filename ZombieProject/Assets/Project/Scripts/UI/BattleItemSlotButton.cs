using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class BattleItemSlotButton : MonoBehaviour
{
    float m_CoolTime = 0.0f;

    public System.Action<Vector3, Vector3, MovingObject> m_UseMethod;
    public ITEM_SLOT_SORT m_slotType { private set; get; }
    public Item m_Item { private set; get; }
    [SerializeField]
    Image m_ItemIcon;

    [SerializeField]
    Image m_CoolDownImage;

    float m_MaxCoolDown;

    
    

    public void SetItem(Item _item)
    {
        m_CoolDownImage.fillAmount = 0.0f;

        if (_item == null)
        {
            m_CoolTime = 0.0f;
            m_ItemIcon.color = Color.clear;
            return;
        }

   
        m_Item = _item;
        m_CoolTime = 0.0f;
        m_MaxCoolDown = _item.m_ItemStat.m_AttackSpeed;
        m_UseMethod = _item.m_AttackMethod;
        m_slotType = _item.m_ItemSlotType;
        m_ItemIcon.sprite = TextureManager.Instance.GetItemIcon(_item.m_ItemStat.m_IconTexrureID);
        m_ItemIcon.color = Color.white;
    }

    private void Update()
    {
        if(m_Item != null)
            TickCoolDown();
    }

    public void ButtonClick()
    {
        if (m_CoolTime > 0.0f) return;
        if (m_UseMethod == null) return;
        if (PlayerManager.Instance.m_Player.m_CurrentEquipedItem == null) return;

        PlayerManager.Instance.m_Player.m_CurrentEquipedItem.PlaySound();

        m_UseMethod(
            PlayerManager.Instance.m_Player.m_CurrentEquipedItem.m_FireTransform.position,
             PlayerManager.Instance.m_Player.transform.forward,
            PlayerManager.Instance.m_Player);

        m_CoolTime = m_MaxCoolDown;
    }

    void TickCoolDown()
    {
        m_CoolTime -= Time.deltaTime;

        if (m_CoolTime < 0.0f) 
        {
            m_CoolTime = 0.0f;
        }

        m_CoolDownImage.fillAmount = m_CoolTime / m_MaxCoolDown;
    }


}

