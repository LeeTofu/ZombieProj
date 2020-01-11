using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public const float WIDTH = 225;
    public const float HEIGHT = 100;

    [SerializeField]
    Image m_SelectedLineImage;

    //[SerializeField]
   // Image EquipItemItmage;
    public bool m_isEquipmentItemSlot;

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

    public Sprite GetItemTexture()
    {
        return m_IconImage.sprite;
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

            if(m_ItemName != null)
                m_ItemName.text = m_Item.m_ItemStat.m_ItemName;

            if(m_ItemLv != null)
                m_ItemLv.text = "Lv " + m_Item.m_Lv.ToString();
        }
        else
        {
            if (m_IconImage)
                m_IconImage.sprite = m_NullImage;

            if (m_ItemName != null)
                m_ItemName.text = "";

            if (m_ItemLv != null)
                m_ItemLv.text = "";

            m_Item = null;
        }

    }

    public void ClickSelectItem()
    {
        if (m_Item == null) return;
        if(InvenManager.Instance.m_Main.SelectItem(this))
            InvenManager.Instance.m_UI.OpenItemInfoUI(this);
        else
            InvenManager.Instance.m_UI.CloseItemInfoUI();
    }

    public void SelectItem()
    {
        m_SelectedLineImage.gameObject.SetActive(true);
    }

    public void UnSelectSlot()
    {
        m_SelectedLineImage.gameObject.SetActive(false);
    }

    public void EquipItem()
    {
        if (m_Item == null) return;

        m_IconImage.color = Color.gray;

        if(m_Equiped != null)
            m_Equiped.gameObject.SetActive(true);
    }

    public void DetachItem()
    {
        if (m_Item == null) return;

        m_IconImage.color = Color.white;

        if (m_Equiped != null)
            m_Equiped.gameObject.SetActive(false);
    }




}
