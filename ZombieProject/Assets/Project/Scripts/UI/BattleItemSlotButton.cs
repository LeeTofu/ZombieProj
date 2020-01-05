using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleItemSlotButton : UIPressSubject
{
    public ITEM_SLOT_SORT m_slotType;
    public Item m_Item { private set; get; }

    MovingObject m_Character;

    [SerializeField]
    Image m_ItemIcon;

    [SerializeField]
    Image m_CoolDownImage;

    ItemActionController m_ItemButtonController;

    public void Init(MovingObject _character ,Item _item)
    {
        m_CoolDownImage.fillAmount = 0.0f;

        if (_item == null)
        {
            m_ItemIcon.color = Color.clear;
            return;
        }

        m_Character = _character;
        m_Item = _item;
        m_slotType = _item.m_ItemSlotType;
        m_ItemIcon.sprite = TextureManager.Instance.GetItemIcon(_item.m_ItemStat.m_IconTexrureID);
        m_ItemIcon.color = Color.white;

        m_ItemButtonController = GetComponent<ItemActionController>();

        if (m_ItemButtonController == null)
            m_ItemButtonController = gameObject.AddComponent<ItemActionController>();

        m_ItemButtonController.Initialized(_item, this);
    }


    private void Update()
    {
        if (m_Item == null) return;
        if (m_ItemButtonController == null) return;

        UpdateCoolDown();
        UpdateAttackSpeed();

        OnPressed();
    }

    void UpdateCoolDown()
    {
        m_ItemButtonController.TickItemCoolTime();
        m_CoolDownImage.fillAmount = m_ItemButtonController.m_CoolTimePercentage;
    }

    void UpdateAttackSpeed()
    {
        m_ItemButtonController.TickItemAttackSpeed();
    }

    bool CheckCanActive()
    {
        if (InvenManager.Instance.isEquipedItemSlot(m_slotType)) return false;

        return true;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(m_ItemButtonController != null && m_Item != null)
        {
            if (m_ItemButtonController.OnPointerDownConditon())
                UpdateObserver(BUTTON_ACTION.PRESS_ENTER);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (m_ItemButtonController != null && m_Item != null)
        {
            if(m_ItemButtonController.OnPointerUpCondition())
                UpdateObserver(BUTTON_ACTION.PRESS_RELEASE);
        }
    }

    public override void OnPressed()
    {
        if (m_ItemButtonController != null && m_Item != null)
        {
            if (m_ItemButtonController.OnPointerPressCondition())
                UpdateObserver(BUTTON_ACTION.PRESS_DOWN);
        }
    }


}

