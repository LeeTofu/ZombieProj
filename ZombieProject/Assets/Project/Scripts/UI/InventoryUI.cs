using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI: BaseUI
{
    [SerializeField]
    ScrollRect m_ScrollRect;

    [SerializeField]
    GameObject m_ScrollGrid;

    Vector2 m_RectSize;

    public ItemSlot m_SelectedItemSlot { private set; get; }

    private void Awake()
    {
        m_RectSize.x = m_ScrollRect.GetComponent<RectTransform>().rect.width;
        m_RectSize.y = m_ScrollRect.GetComponent<RectTransform>().rect.height;
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
