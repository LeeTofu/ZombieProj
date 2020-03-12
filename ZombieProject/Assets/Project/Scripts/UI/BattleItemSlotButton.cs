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
      //  m_ItemIcon.color = Color.white;
        UpdateStackCountText();

        if (m_ItemButtonController == null)
            m_ItemButtonController = GetComponent<SlotController>();

        if (m_ItemButtonController == null)
            m_ItemButtonController = gameObject.AddComponent<SlotController>();

        m_ItemButtonController.enabled = true;
       
        if (m_slotType != ITEM_SLOT_SORT.SECOND)
            m_ItemButtonController.Initialized(_item.m_ItemStat);
        else
        {
            m_ItemButtonController.Initialized(4.0f, true, 0.0f);
        }

        if (m_isInitialize == false)
        {
            RegisterEvent(BUTTON_ACTION.PRESS_ENTER, PressEnterItemAction);
            RegisterEvent(BUTTON_ACTION.PRESS_DOWN, PressItemAction);
        }

        m_isInitialize = true;
    }

    // 슬롯에 장착된 아이템 스탯 업데이트 해주는 함수.
    public void UpdateItemStat(ItemStat _itemStat)
    {
        if (m_ItemButtonController != null)
            m_ItemButtonController.Initialized(_itemStat);
        else Debug.LogError("아이템 버튼을 컨트롤 해줄 놈이 없는데...?");
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
        if (PlayerManager.Instance.m_Player == null) return false;
        if (PlayerManager.Instance.m_Player.m_Stat.isDead) return false;
        if (PlayerManager.Instance.GetPlayerState() == E_PLAYABLE_STATE.DRINK 
            || PlayerManager.Instance.GetPlayerState() == E_PLAYABLE_STATE.USE_QUICK
            || PlayerManager.Instance.GetPlayerState() == E_PLAYABLE_STATE.PICK_UP) return false;

        return true;
    }

    public void SpendItemStackCount()
    {
        m_Item.spendItem();

        UpdateStackCountText();
    }
    public void plusItemStackCount(short _acc)
    {
        m_Item.plusItem(_acc);
        m_StackCountText.text = m_Item.m_Count.ToString();

        if (m_Item.m_Count > 0)
        {
            m_ItemIcon.color = Color.white;
        }
    }

    public void ChargeMaxStackCount()
    {
        if (PlayerManager.Instance.m_CurrentEquipedItemObject == null || m_Item == null)
        {
            Debug.LogError("무기가 없는데? 뭘 충전하지");
            return;
        }

        if (m_slotType == ITEM_SLOT_SORT.MAIN)
        {
            m_Item.FullChargeItemCount(PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_Count);
        }
        else
        {
            m_Item.FullChargeItemCount(m_Item.m_ItemStat.m_Count);
        }

        UpdateStackCountText();
    }

    public void UpdateStackCountText()
    {
        if (m_Item == null) return;
        if(m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
        {
            m_StackCountText.text = "";
            m_ItemIcon.color = Color.white;
        }
        else if (m_Item.m_Count > 0)
        {
            m_StackCountText.text = m_Item.m_Count.ToString();
            m_ItemIcon.color = Color.white;
        }
        else
        {
            m_StackCountText.text = "<color=#ff0000>" + m_Item.m_Count.ToString() + "</color>";
            m_ItemIcon.color = Color.red;
        }
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
                    if (m_Item == null) return;
                    if (m_Item.m_Count <= 0) return;

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
                    if (m_Item == null) return;
                    if (m_Item.m_Count <= 0) return;

                    PlayerManager.Instance.PlayerAttack();
                    SpendItemStackCount();
                    break;
            }
        }
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        if (PlayerManager.Instance.m_Player == null) return;

        if (m_ItemButtonController != null && m_Item !=null )
        {
            if (m_ItemButtonController.OnPointerDownConditon())
                UpdateObserver(BUTTON_ACTION.PRESS_ENTER);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (PlayerManager.Instance.m_Player == null) return;

        if (m_ItemButtonController != null && m_Item != null )
        {
            if(m_ItemButtonController.OnPointerUpCondition())
                UpdateObserver(BUTTON_ACTION.PRESS_RELEASE);
        }
    }

    public override void OnPressed()
    {
        if (PlayerManager.Instance.m_Player == null) return;

        if (m_ItemButtonController != null && m_Item != null )
        {
            if (m_ItemButtonController.OnPointerPressCondition())
                UpdateObserver(BUTTON_ACTION.PRESS_DOWN);
        }
    }


}

