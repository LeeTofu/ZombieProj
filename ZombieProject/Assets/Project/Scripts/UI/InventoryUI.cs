using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI: BaseUI
{
    [SerializeField]
    Button m_SecondEquipButton;

    [SerializeField]
    ScrollRect m_ScrollRect;

    [SerializeField]
    GameObject m_ScrollGrid;

    Vector2 m_RectSize;

    public ItemSlot[] m_ItemEquipmentSlot;

    public ItemInfoUI m_ItemInfoUI;

    [SerializeField]
    TextMeshProUGUI PlayerLevel;

    [SerializeField]
    TextMeshProUGUI PlayerName;

    ITEM_SLOT_SORT m_CurrentQuickItemEquipSlot = ITEM_SLOT_SORT.THIRD;

    public ItemSlot m_SelectedSlot { private set; get; }

    private void Awake()
    {
        m_RectSize.x = m_ScrollRect.GetComponent<RectTransform>().rect.width;
        m_RectSize.y = m_ScrollRect.GetComponent<RectTransform>().rect.height;

        m_ItemInfoUI = GetComponentInChildren<ItemInfoUI>();
        m_ItemInfoUI.gameObject.SetActive(false);
        Debug.Log("Shop  UI 불러옴");
    }

    public void InsertToScroll(ItemSlot _slot, int _c, int _r)
    {
        _slot.transform.SetParent(m_ScrollGrid.transform);
        _slot.transform.localPosition = 0.5f * new Vector3(ItemSlot.WIDTH, -ItemSlot.HEIGHT, 0) + _c * new Vector3(ItemSlot.WIDTH, 0, 0) - _r * new Vector3(0, ItemSlot.HEIGHT, 0);
    }

    public override void InitializeUI()
    {
        PlayerName.text = PlayerManager.Instance.m_PlayerInfo.playerName;
        PlayerLevel.text = "Lv " +PlayerManager.Instance.m_PlayerInfo.Lv.ToString();

        SortItemslot(MAIN_ITEM_SORT.EQUIPMENT);

        SetEuqipmentItemSlot(ITEM_SLOT_SORT.MAIN, m_ItemEquipmentSlot[((int)ITEM_SLOT_SORT.MAIN - 1)]);
        SetEuqipmentItemSlot(ITEM_SLOT_SORT.SECOND, m_ItemEquipmentSlot[((int)ITEM_SLOT_SORT.SECOND - 1)]);
        SetEuqipmentItemSlot(ITEM_SLOT_SORT.THIRD, m_ItemEquipmentSlot[((int)ITEM_SLOT_SORT.THIRD - 1)]);
        SetEuqipmentItemSlot(ITEM_SLOT_SORT.FOURTH, m_ItemEquipmentSlot[((int)ITEM_SLOT_SORT.FOURTH - 1)]);
  //B      SetEuqipmentItemSlot(ITEM_SLOT_SORT.FIFTH, m_ItemEquipmentSlot[((int)ITEM_SLOT_SORT.FIFTH - 1)]);
    }

    public bool SelectItem(ItemSlot _slot)
    {
        if (m_SelectedSlot)
        {
            m_SelectedSlot.UnSelectSlot();

            if (_slot == m_SelectedSlot)
            {
                m_SelectedSlot = null;
                return false;
            }
        }

        m_SelectedSlot = _slot;

        if (m_SelectedSlot != null)
        {
            m_SelectedSlot.SelectItem();

            return true;
        }
        else return false;

    }

    void SetEuqipmentItemSlot(ITEM_SLOT_SORT _slotType, ItemSlot _EquipmentSlot)
    {
        if (_EquipmentSlot == null) return;
        
        Item equipedItem = InvenManager.Instance.GetEquipedItemSlot(_slotType);
        if (equipedItem != null)
        {
            MAIN_ITEM_SORT slotType = InvenManager.Instance.ConvertSortToMainSort(_slotType);

            if (slotType == MAIN_ITEM_SORT.END || slotType == MAIN_ITEM_SORT.NONE) return;

            ItemSlot slot = InvenManager.Instance.GetItemSlot(slotType, equipedItem.m_UniqueItemID);

            if (slot == null) return;
            if (!slot.m_CanUse) return;

            slot.EquipItem();
            _EquipmentSlot.SetItem(equipedItem);
        }
    }

    void SortItemslot(MAIN_ITEM_SORT _slot)
    {
        switch(_slot)
        {
            case MAIN_ITEM_SORT.EQUIPMENT:
                m_SecondEquipButton.gameObject.SetActive(true);
                break;
            default:
                m_SecondEquipButton.gameObject.SetActive(false);
                break;
        }

        InvenManager.Instance.InitializeInventoryTab(_slot);
    }

    public void ClickEquipmentslot()
    {
        SoundManager.Instance.OneShotPlay(UI_SOUND.INVEN_SLOT_CHANGE);
        SortItemslot(MAIN_ITEM_SORT.EQUIPMENT);
    }

    public void ClickETCSlot()
    {
        SoundManager.Instance.OneShotPlay(UI_SOUND.INVEN_SLOT_CHANGE);
        SortItemslot(MAIN_ITEM_SORT.ETC);
    }

    public void ClickQuickSlot()
    {
        SoundManager.Instance.OneShotPlay(UI_SOUND.INVEN_SLOT_CHANGE);
        SortItemslot(MAIN_ITEM_SORT.QUICK);
    }

    public void ClickDetachButton()
    {
        if (m_SelectedSlot == null) return;
        if (m_SelectedSlot.m_Item == null) return;
        if (!m_SelectedSlot.m_Item.m_isEquiped) return;
        if (!m_SelectedSlot.m_CanUse) return;

        DetachItem(m_SelectedSlot.m_Item.m_ItemSlotType);
    }

    public void ClickEquipButton()
    {
        if (m_SelectedSlot == null) return;
        if (m_SelectedSlot.m_Item == null) return;
        if (!m_SelectedSlot.m_CanUse) return;

        MAIN_ITEM_SORT sort = InvenManager.Instance.ConvertSortToMainSort(m_SelectedSlot.m_Item.m_ItemStat.m_Sort);

        switch (sort)
        {
            case MAIN_ITEM_SORT.EQUIPMENT:
                EquipItem(ITEM_SLOT_SORT.MAIN);
                break;
            case MAIN_ITEM_SORT.QUICK:
                EquipItem(m_CurrentQuickItemEquipSlot);
                m_CurrentQuickItemEquipSlot = (m_CurrentQuickItemEquipSlot == ITEM_SLOT_SORT.THIRD) ? ITEM_SLOT_SORT.FOURTH : ITEM_SLOT_SORT.THIRD;
                break;
        }
    }

    public void ClickSecondaryEquipButton()
    {
        if (m_SelectedSlot == null) return;
        if (m_SelectedSlot.m_Item == null) return;
        if (!m_SelectedSlot.m_CanUse) return;


        MAIN_ITEM_SORT sort = InvenManager.Instance.ConvertSortToMainSort(m_SelectedSlot.m_Item.m_ItemStat.m_Sort);

        if (sort != MAIN_ITEM_SORT.EQUIPMENT) return;

        EquipItem(ITEM_SLOT_SORT.SECOND);
    }

    private void EquipItem(ITEM_SLOT_SORT _slotType)
    {
        if (m_SelectedSlot == null)
        {
            Debug.LogError("선택한 아이템이 없다.");
            return;
        }
        else if (m_SelectedSlot.m_Item == null)
        {
            Debug.LogError("선택한 아이템 슬롯에 아이템이 없다.");
            return;
        }

        if (m_SelectedSlot.m_isEquipmentItemSlot) return;

        SoundManager.Instance.OneShotPlay(UI_SOUND.WEAPON_CHANGE);
        InvenManager.Instance.EquipItem(
          m_SelectedSlot.m_Item.m_UniqueItemID,
          _slotType, m_ItemEquipmentSlot[(int)_slotType - 1]);
    }

    public void DetachItem(ITEM_SLOT_SORT _slotType)
    {
        Debug.Log("누름");
        if (!m_SelectedSlot)
        {
            return;
        }

         if (m_SelectedSlot.m_Item == null)
        {
            return;
        }

       // Debug.Log("들어가기 시작");
        SoundManager.Instance.OneShotPlay(UI_SOUND.WEAPON_CHANGE);
        InvenManager.Instance.DetachItem(_slotType, m_ItemEquipmentSlot[(int)_slotType - 1]);
        SetEuqipmentItemSlot(_slotType, m_ItemEquipmentSlot[(int)(_slotType) - 1]);
    }

    public override void DeleteUI()
    {
        SelectItem(null);
        CloseItemInfoUI();
        SortItemslot(MAIN_ITEM_SORT.NONE);
    }

    public void OpenItemInfoUI(ItemSlot _slot)
    {
        if (_slot == null) return;
        if (_slot.m_Item == null) return;

       MAIN_ITEM_SORT mainSort = InvenManager.Instance.ConvertSortToMainSort(_slot.m_Item.m_ItemStat.m_Sort);

        if (mainSort != MAIN_ITEM_SORT.EQUIPMENT)
        {
            m_SecondEquipButton.gameObject.SetActive(false);
            m_SecondEquipButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.gray;

        }
        else
        {
            m_SecondEquipButton.gameObject.SetActive(true);
            m_SecondEquipButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.green;
        }

        m_ItemInfoUI.gameObject.SetActive(true);
        m_ItemInfoUI.Initialize(_slot);
    }

    public void CloseItemInfoUI()
    {
        m_ItemInfoUI.gameObject.SetActive(false);
       // m_ItemInfoUI.Initialize(null);
    }

    public void ClickBackIcon()
    {
        SceneMaster.Instance.LoadScene(GAME_SCENE.MAIN);

    }

}







