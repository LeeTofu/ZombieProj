using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleItemSlotButton : UIPressSubject
{
    public ITEM_SLOT_SORT m_slotType;
    public Item m_Item { private set; get; }

    [SerializeField]
    TMPro.TextMeshProUGUI m_StackCountText;

    [SerializeField]
    Image m_ItemIcon;

    [SerializeField]
    protected Image m_CoolDownImage;

    protected SlotController m_ItemButtonController;
    
    protected bool m_isInitialize = false;

    public virtual void Init(MovingObject _character ,Item _item)
    {
        m_CoolDownImage.fillAmount = 0.0f;
        m_StackCountText.text = " ";

        if (_item == null)
        {
            m_Item = null;
            m_ItemIcon.color = Color.clear;

            if (m_ItemButtonController != null)
                m_ItemButtonController.enabled = false;

            return;
        }

        m_Item = _item;
       
        m_ItemIcon.sprite = TextureManager.Instance.GetItemIcon(_item.m_ItemStat.m_IconTexrureID);
        m_ItemIcon.color = Color.white;

        m_StackCountText.text = _item.m_Count.ToString();

        if (m_ItemButtonController == null)
            m_ItemButtonController = GetComponent<SlotController>();

        if (m_ItemButtonController == null)
            m_ItemButtonController = gameObject.AddComponent<SlotController>();

        m_ItemButtonController.enabled = true;
       
        if (m_slotType != ITEM_SLOT_SORT.SECOND)
            m_ItemButtonController.Initialized(_item);
        else
        {
            m_ItemButtonController.Initialized(5.0f, true, 0.0f);
        }

        if (m_isInitialize == false)
        {
            RegisterEvent(BUTTON_ACTION.PRESS_ENTER, PressEnterItemAction);
            RegisterEvent(BUTTON_ACTION.PRESS_DOWN, PressItemAction);
        }

        m_isInitialize = true;
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
        if (m_ItemButtonController.enabled)
        {
            m_ItemButtonController.TickItemCoolTime();
            m_CoolDownImage.fillAmount = m_ItemButtonController.m_CoolTimePercentage;
        }
    }

    void UpdateAttackSpeed()
    {
        m_ItemButtonController.TickItemAttackSpeed();
    }

    bool CheckCanActive()
    {
        if (m_ItemButtonController.m_CoolTimePercentage > 0) return false;
        if (!InvenManager.Instance.isEquipedItemSlot(m_slotType)) return false;
        if (m_Item.m_Count <= 0) return false;

        return true;
    }

    public void SpendItemStackCount()
    {
        m_Item.spendItem();

        if (m_Item.m_Count > 0)
        {
            m_StackCountText.text = m_Item.m_Count.ToString();
        }
        else
        {
            m_StackCountText.text = "<color=#ff0000>" + m_Item.m_Count.ToString() + "</color>";
        }
    }
    public void plusItemStackCount(int _acc)
    {
        m_Item.plusItem(_acc);
        m_StackCountText.text = m_Item.m_Count.ToString();
    }


    void PressEnterItemAction()
    {
        if(CheckCanActive())
        {
            switch (m_slotType)
            {
                case ITEM_SLOT_SORT.MAIN:
                    break;
                case ITEM_SLOT_SORT.SECOND:
                    PlayerManager.Instance.ChangeWeapon();
                    break;
                default:
                    PlayerManager.Instance.PlayerUseItem(m_slotType);
                    SpendItemStackCount();
                    break;

            }

           
        }
    }

    void PressItemAction()
    {
        if (CheckCanActive())
        {
            switch (m_slotType)
            {
                case ITEM_SLOT_SORT.MAIN:
                    PlayerManager.Instance.PlayerAttack();
                    SpendItemStackCount();
                    break;
            }
        }
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        if(m_ItemButtonController != null && m_Item !=null )
        {
            if (m_ItemButtonController.OnPointerDownConditon())
                UpdateObserver(BUTTON_ACTION.PRESS_ENTER);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (m_ItemButtonController != null && m_Item != null )
        {
            if(m_ItemButtonController.OnPointerUpCondition())
                UpdateObserver(BUTTON_ACTION.PRESS_RELEASE);
        }
    }

    public override void OnPressed()
    {
        if (m_ItemButtonController != null && m_Item != null )
        {
            if (m_ItemButtonController.OnPointerPressCondition())
                UpdateObserver(BUTTON_ACTION.PRESS_DOWN);
        }
    }


}

