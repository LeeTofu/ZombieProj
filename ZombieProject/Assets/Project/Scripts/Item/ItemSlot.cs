﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public const float WIDTH = 225;
    public const float HEIGHT = 100;

    [SerializeField]
    Image m_SelectedLine;

    [SerializeField]
    TextMeshProUGUI m_Equiped;

    Sprite m_NullImage;

    [SerializeField]
    Image m_IconImage;

    [SerializeField]
    TextMeshProUGUI m_ItemName;

    [SerializeField]
    TextMeshProUGUI m_ItemLv;

    public Item m_Item { private set; get; }

    public void Awake()
    {
        m_NullImage = m_IconImage.sprite;
    }


    public void SetItem(Item _item)
    {
        m_Item = _item;

        if (m_Item != null)
        {
            Sprite sprite = TextureManager.Instance.GetItemIcon(m_Item.m_ItemStat.m_IconTexrureID);
            if (sprite)
            {
                m_IconImage.sprite = sprite;
            }

            m_ItemName.text = m_Item.m_ItemStat.m_ItemName;
            m_ItemLv.text = "Lv " + m_Item.m_Lv.ToString();
        }
        else
        {
            if (m_IconImage)
                m_IconImage.sprite = m_NullImage;

            m_ItemName.text = "";
            m_ItemLv.text = "";

            m_Item = null;
        }

    }

    public void ClickSelectItem()
    {
        if (m_Item == null) return;

        InvenManager.Instance.m_Main.SelectItem(this);
    }

    public void SelectItem()
    {
        m_SelectedLine.gameObject.SetActive(true);
    }

    public void UnSelectSlot()
    {
        m_SelectedLine.gameObject.SetActive(false);
    }

    public void EquipItem()
    {
        if (m_Item == null) return;
        if (m_Item.m_isEquiped) return;

        m_IconImage.color = Color.gray;
        m_Equiped.gameObject.SetActive(true);
        InvenManager.Instance.EquipItem(m_Item.m_UniqueItemID, ITEM_SLOT_SORT.MAIN);
    }

    public void DetachItem()
    {
        if (m_Item == null) return;
        if (!m_Item.m_isEquiped) return;

        m_IconImage.color = Color.white;
        m_Equiped.gameObject.SetActive(false);
        InvenManager.Instance.DetachItem( m_Item.m_ItemSlotType);
    }




}
