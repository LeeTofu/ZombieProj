﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public ItemInfoUI m_ItemInfoUI { private set; get; }

  //  public ItemSlot m_SelectedItemSlot { private set; get; }

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
        _slot.transform.localPosition = 0.5f * new Vector3(ItemSlot.WIDTH + 30, -ItemSlot.HEIGHT, 0) + _c * new Vector3(ItemSlot.WIDTH, 0, 0) - _r * new Vector3(0, ItemSlot.HEIGHT, 0);
    }

    public override void InitializeUI()
    {
        SortItemslot(MAIN_ITEM_SORT.EQUIPMENT);

        SetEuqipmentItemSlot(ITEM_SLOT_SORT.MAIN, m_ItemEquipmentSlot[((int)ITEM_SLOT_SORT.MAIN - 1)]);
        SetEuqipmentItemSlot(ITEM_SLOT_SORT.SECOND, m_ItemEquipmentSlot[((int)ITEM_SLOT_SORT.SECOND - 1)]);
        SetEuqipmentItemSlot(ITEM_SLOT_SORT.THIRD, m_ItemEquipmentSlot[((int)ITEM_SLOT_SORT.THIRD - 1)]);
        SetEuqipmentItemSlot(ITEM_SLOT_SORT.FOURTH, m_ItemEquipmentSlot[((int)ITEM_SLOT_SORT.FOURTH - 1)]);
    }

    void SetEuqipmentItemSlot(ITEM_SLOT_SORT _slotType, ItemSlot _EquipmentSlot)
    {
        Item equipedItem = InvenManager.Instance.GetEquipedItemSlot(_slotType);
        if (equipedItem != null)
        {
            MAIN_ITEM_SORT slotType = InvenManager.Instance.ConvertSortToMainSort(_slotType);

            ItemSlot slot = InvenManager.Instance.GetItemSlot(slotType, equipedItem.m_UniqueItemID);
            slot.EquipItem();
            // _slot.EquipItem();
            _EquipmentSlot.SetItem(equipedItem);
        }
    }

    void SortItemslot(MAIN_ITEM_SORT _slot)
    {
        InvenManager.Instance.InitializeInventoryTab(_slot);
    }

    public void ClickEquipmentslot()
    {
        SortItemslot(MAIN_ITEM_SORT.EQUIPMENT);
    }

    public void ClickETCSlot()
    {
        SortItemslot(MAIN_ITEM_SORT.ETC);
    }

    public void ClickQuickSlot()
    {
        SortItemslot(MAIN_ITEM_SORT.QUICK);
    }

    public void ClickDetachButton()
    {
        if (InvenManager.Instance.m_Main.m_SelectedSlot == null) return;
        if (!InvenManager.Instance.m_Main.m_SelectedSlot.m_Item.m_isEquiped) return;

        DetachItem(InvenManager.Instance.m_Main.m_SelectedSlot.m_Item.m_ItemSlotType);
    }

    public void ClickEquipButton()
    {
        if (InvenManager.Instance.m_Main.m_SelectedSlot == null) return;

        MAIN_ITEM_SORT sort = InvenManager.Instance.ConvertSortToMainSort(InvenManager.Instance.m_Main.m_SelectedSlot.m_Item.m_ItemStat.m_Sort);

        switch(sort)
        {
            case MAIN_ITEM_SORT.EQUIPMENT:
                EquipItem(ITEM_SLOT_SORT.MAIN);
                break;
            case MAIN_ITEM_SORT.ETC:
                EquipItem(ITEM_SLOT_SORT.FOURTH);
                break;
            case MAIN_ITEM_SORT.QUICK:
                EquipItem(ITEM_SLOT_SORT.THIRD);
                break;
        }
    }

    public void ClickSecondaryEquipButton()
    {
        if (InvenManager.Instance.m_Main.m_SelectedSlot == null) return;

        MAIN_ITEM_SORT sort = InvenManager.Instance.ConvertSortToMainSort(InvenManager.Instance.m_Main.m_SelectedSlot.m_Item.m_ItemStat.m_Sort);

        if (sort != MAIN_ITEM_SORT.EQUIPMENT) return;

        EquipItem(ITEM_SLOT_SORT.SECOND);
    }


    private void EquipItem(ITEM_SLOT_SORT _slotType)
    {
        if (InvenManager.Instance.m_Main.m_SelectedSlot == null)
        {
            Debug.LogError("선택한 아이템이 없다.");
            return;
        }
        else if (InvenManager.Instance.m_Main.m_SelectedSlot.m_Item == null)
        {
            Debug.LogError("선택한 아이템 슬롯에 아이템이 없다.");
            return;
        }

        InvenManager.Instance.EquipItem(
          InvenManager.Instance.m_Main.m_SelectedSlot.m_Item.m_UniqueItemID,
          _slotType, m_ItemEquipmentSlot[(int)_slotType]);
    }

    public void DetachItem(ITEM_SLOT_SORT _slotType)
    {
        Debug.Log("누름");
        if (!InvenManager.Instance.m_Main.m_SelectedSlot)
        {
            return;
        }

         if (InvenManager.Instance.m_Main.m_SelectedSlot.m_Item == null)
        {
            return;
        }

        Debug.Log("들어가기 시작");
        // m_ItemEquipmentSlot[(int)_slotType].SetItem(null);
        InvenManager.Instance.DetachItem(_slotType, m_ItemEquipmentSlot[(int)_slotType]);
    }

    public override void DeleteUI()
    {
        SortItemslot(MAIN_ITEM_SORT.NONE);
    }

    public void OpenItemInfoUI(ItemSlot _slot)
    {
       MAIN_ITEM_SORT mainSort = InvenManager.Instance.ConvertSortToMainSort(_slot.m_Item.m_ItemStat.m_Sort);

        if (mainSort != MAIN_ITEM_SORT.EQUIPMENT)
        {
            m_SecondEquipButton.enabled = true;
            m_SecondEquipButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.gray;

        }
        else
        {
            m_SecondEquipButton.enabled = false;
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







