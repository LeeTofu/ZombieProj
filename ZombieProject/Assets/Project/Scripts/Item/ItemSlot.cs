using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public const float WIDTH = 225;
    public const float HEIGHT = 100;

    InventoryUI m_UI;

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
    Image m_NotUsedImage;

    [SerializeField]
    TextMeshProUGUI m_ItemLv;

    [SerializeField]
    TextMeshProUGUI m_WaveInfo;

    public bool m_CanUse { private set; get; }

    public ItemSlot m_SelectedSlot { get; private set; }

    public Item m_Item { private set; get; }

    public void Awake()
    {
        m_NullImage = m_IconImage.sprite;
    }

    private void Start()
    {
        m_UI = UIManager.Instance.GetUIObject(GAME_SCENE.INVENTORY).GetComponent<InventoryUI>();
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

            if( InvenManager.Instance.ConvertSortToMainSort(_item.m_ItemStat.m_Sort) == MAIN_ITEM_SORT.QUICK)
            {
                m_IconImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 120 * m_IconImage.rectTransform.localScale.x);
                m_IconImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80 * m_IconImage.rectTransform.localScale.x);
            }
            else 
            {
                m_IconImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 210 * m_IconImage.rectTransform.localScale.x);
                m_IconImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80 * m_IconImage.rectTransform.localScale.x);
            }

            if (m_ItemName != null)
                m_ItemName.text = m_Item.m_ItemStat.m_ItemName;

           
            if (m_Item.m_ItemStat.m_Lv >= PlayerManager.Instance.m_MaxClearWave)
            {
                m_CanUse = false;

                if (m_NotUsedImage != null)
                    m_NotUsedImage.gameObject.SetActive(true);
                if (m_WaveInfo != null)
                {
                    m_WaveInfo.gameObject.SetActive(true);
                    m_WaveInfo.text = "Wave '" + m_Item.m_ItemStat.m_Lv.ToString() + "' 이상 \n 클리어";
                }
            }
            else
            {
                m_CanUse = true;

                if(m_NotUsedImage != null)
                    m_NotUsedImage.gameObject.SetActive(false);

                if (m_WaveInfo != null)
                    m_WaveInfo.gameObject.SetActive(false);
            }


        }
        else
        {
            if (m_IconImage)
                m_IconImage.sprite = m_NullImage;

            if (m_isEquipmentItemSlot)
            {
                m_IconImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 238);
                m_IconImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 96);
            }

            if (m_ItemName != null)
                m_ItemName.text = "";

            if (m_ItemLv != null)
                m_ItemLv.text = "";

            m_Item = null;

            if (m_NotUsedImage != null)
                m_NotUsedImage.gameObject.SetActive(false);
        }

    }

   

    public void ClickSelectItem()
    {
        if (m_Item == null) return;
        if(m_UI.SelectItem(this))
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
