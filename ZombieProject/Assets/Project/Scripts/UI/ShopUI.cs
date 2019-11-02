using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : BaseUI
{
    [SerializeField]
    GameObject m_ItemSlotPivot;

    public ItemSlot m_SelectedItemSlot { private set; get; }

    private void Start()
    {
        Debug.Log("Shop  UI 불러옴");
    }

    public override void InitializeUI()
    {
        SortItemslot(MAIN_ITEM_SORT.EQUIPMENT);
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

    public override void DeleteUI()
    {
        SortItemslot(MAIN_ITEM_SORT.NONE);
    }
}
