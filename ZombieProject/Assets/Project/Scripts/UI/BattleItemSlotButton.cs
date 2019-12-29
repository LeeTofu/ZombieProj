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

    ItemAction m_ItemAction;

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

        GetActionFromManager(m_Item);

    }

    // ============================================================
    // 버튼 누르면 액션이 발동 된다.
    // 그 액션은 아이템 타입에 따라 다르다.
    // 그러므로 아이템 타입에 따른 액션을 가져오기 위해 ItemManager, ActionTypeManager에서 액션을 가져오도록 한다.
    // 끝.
    private void  GetActionFromManager(Item _item)
    {
        if (_item == null)
        {
            Debug.LogError("item 없는데~ 액션을 넣는다고? ");
            return;
        }

        m_ItemAction = GetComponent<ItemAction>();

        if (m_ItemAction == null)
            m_ItemAction = gameObject.AddComponent<ItemAction>();

        m_ItemAction.Initialized(_item, this);

        for (BUTTON_ACTION actionType = BUTTON_ACTION.PRESS_ENTER; actionType != BUTTON_ACTION.END; actionType++)
        {
            ITEM_EVENT_TYPE eventType =  ItemManager.Instance.GetItemActionType(_item);
            var action = ActionTypeManager.Instance.GetActionType(actionType, eventType);

            if (eventType == ITEM_EVENT_TYPE.END)
            {
               // Debug.LogError("END라서 액션 못넣음");
                continue;
            }
            if (action == null)
            {
               // Debug.LogError("action null이라 액션 못넣음" + actionType + " , " + eventType);
                continue;
            }

            m_ItemAction.SetPlayAction(actionType, action);
        }
    }

    private void Update()
    {
        

        if (m_Item == null) return;
        if (m_ItemAction == null) return;

        UpdateCoolDown();
        UpdateAttackSpeed();

        OnPressed();
    }

    void UpdateCoolDown()
    {
        m_ItemAction.TickItemCoolTime();
        m_CoolDownImage.fillAmount = m_ItemAction.m_CoolTimePercentage;
    }

    void UpdateAttackSpeed()
    {
        m_ItemAction.TickItemAttackSpeed();
    }

    bool CheckCanActive()
    {
        if (InvenManager.Instance.isEquipedItemSlot(m_slotType)) return false;

        return true;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(m_ItemAction != null)
        {
            if (m_ItemAction.OnPointerDown())
                UpdateObserver(BUTTON_ACTION.PRESS_ENTER);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (m_ItemAction != null)
        {
            if(m_ItemAction.OnPointerUp())
                UpdateObserver(BUTTON_ACTION.PRESS_RELEASE);
        }
    }

    public override void OnPressed()
    {
        if (m_ItemAction != null)
        {
            if (m_ItemAction.OnPointerPress())
                UpdateObserver(BUTTON_ACTION.PRESS_DOWN);
        }
    }


}

