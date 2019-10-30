using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class InvenManager : Singleton<InvenManager>
{
    GameObject m_ItemSlotPrefab;
    public MovingObject m_Player { get; private set; }
    public MAIN_ITEM_SORT m_CurSelectedInventoryTab { get; private set; }

    Dictionary<MAIN_ITEM_SORT, int> m_ItenSlotCount = new Dictionary<MAIN_ITEM_SORT, int>();
    Dictionary<MAIN_ITEM_SORT, List<ItemSlot>> m_ItemInventorySlot = new Dictionary<MAIN_ITEM_SORT, List<ItemSlot>>();
    Dictionary<int, Item> m_ItemInventory = new Dictionary<int, Item>();

    // 게임내 장착된 아이템 
    Dictionary<ITEM_SLOT_SORT, Item> m_EquipedItemSlots = new Dictionary<ITEM_SLOT_SORT, Item>();

    public const int MAX_INVEN_SLOT = 50;

    public override bool Initialize()
    {
        
        m_ItemSlotPrefab = Resources.Load<GameObject>("Prefabs/ItemUI/ItemSlotUI");
             
        m_ItenSlotCount.Clear();

        m_ItenSlotCount.Add(MAIN_ITEM_SORT.EQUIPMENT, 0);
        m_ItenSlotCount.Add(MAIN_ITEM_SORT.QUICK, 0);
        m_ItenSlotCount.Add(MAIN_ITEM_SORT.ETC, 0);

        m_ItemInventorySlot.Clear();

        m_ItemInventorySlot.Add(MAIN_ITEM_SORT.EQUIPMENT, new List<ItemSlot>());
        m_ItemInventorySlot.Add(MAIN_ITEM_SORT.ETC, new List<ItemSlot>());
        m_ItemInventorySlot.Add(MAIN_ITEM_SORT.QUICK, new List<ItemSlot>());

        for (int i = 0; i < MAX_INVEN_SLOT; i++)
        {
            ItemSlot slot = CreateInventoryItemSlot(null);

            if(slot != null)
                 m_ItemInventorySlot[MAIN_ITEM_SORT.EQUIPMENT].Add(slot);

            slot = CreateInventoryItemSlot(null);

            if (slot != null)
                m_ItemInventorySlot[MAIN_ITEM_SORT.ETC].Add(slot);

            slot = CreateInventoryItemSlot(null);

            if (slot != null)
                m_ItemInventorySlot[MAIN_ITEM_SORT.QUICK].Add(slot);
        }

        m_EquipedItemSlots.Clear();
        m_EquipedItemSlots.Add(ITEM_SLOT_SORT.MAIN, null);
        m_EquipedItemSlots.Add(ITEM_SLOT_SORT.SECOND, null);
        m_EquipedItemSlots.Add(ITEM_SLOT_SORT.THIRD, null);
        m_EquipedItemSlots.Add(ITEM_SLOT_SORT.FOURTH, null);

        m_CurSelectedInventoryTab = MAIN_ITEM_SORT.NONE;
        SetInventoryTab(MAIN_ITEM_SORT.EQUIPMENT);

        return true;
    }

    public ItemSlot CreateInventoryItemSlot(Item _item)
    {
        GameObject newObj = Instantiate(m_ItemSlotPrefab);
        ItemSlot slot = newObj.GetComponent<ItemSlot>();

        if (slot == null)
        {
            Debug.LogError("Slot 오브젝트에 ItemSlot이 없다");
            return null;
        }

        slot.SetItem(_item);

        newObj.transform.SetParent(transform);
        newObj.transform.localPosition = Vector3.zero;
        newObj.transform.localRotation = Quaternion.identity;

        newObj.SetActive(false);
       
        return slot;
    }

    // 서버에 대응
    public void GetItemInvenFromXML()
    {
        TextAsset playerInvenList = (TextAsset)Resources.Load("Data/Inventory/PlayerInventoryList");
        //TextAsset ownedItem = (TextAsset)Resources.Load("Data/Inventory/OwnedItemData");
        XmlDocument xmlDoc = new XmlDocument();
        //Debug.Log(txtAsset.text);
        xmlDoc.LoadXml(playerInvenList.text);

        XmlNodeList all_nodes = xmlDoc.SelectNodes("Root/text");

        foreach (XmlNode node in all_nodes)
        {
            ITEM_SLOT_SORT slotSort;
            bool success = System.Enum.TryParse<ITEM_SLOT_SORT>(node.SelectSingleNode("SlotSort").InnerText, out slotSort);
            if (!success)
            {
                Debug.LogError("Item Parse Fail ItemSlotSort 확인할 것 : " + slotSort);
                continue;
            }

            int currentEXP = int.Parse(node.SelectSingleNode("CurrentEXP").InnerText);
            int Lv = int.Parse(node.SelectSingleNode("Level").InnerText);
            int uniqueID = int.Parse(node.SelectSingleNode("uniqueID").InnerText);
            int itemID = int.Parse(node.SelectSingleNode("ItemID").InnerText);

            bool isEquiped = bool.Parse(node.SelectSingleNode("isEquiped").InnerText);
           
            ItemStat stat = ItemManager.Instance.GetItemStat(itemID);

            Item item = null;

            if (stat.m_ItemID != -1)
            {
                item = new Item(uniqueID, Lv, currentEXP, stat);
                bool result = InsertItemToInventory(item);

                if (result)
                {
                    if (isEquiped && slotSort != ITEM_SLOT_SORT.NONE) 
                        EquipItem(uniqueID, slotSort);
                }
            }
        }
    }

    public void SetInventoryTab(MAIN_ITEM_SORT _tab)
    { 
        if (_tab == m_CurSelectedInventoryTab) return;

        if (m_CurSelectedInventoryTab != MAIN_ITEM_SORT.NONE)
        {
            foreach (ItemSlot slot in m_ItemInventorySlot[m_CurSelectedInventoryTab])
            {
                slot.gameObject.SetActive(false);
            }
        }

        m_CurSelectedInventoryTab = _tab;

        foreach (ItemSlot slot in m_ItemInventorySlot[m_CurSelectedInventoryTab])
        {
            slot.gameObject.SetActive(true);
        }
    }

    private MAIN_ITEM_SORT ConvertSortToMainSort(ITEM_SORT _sort)
    {
        MAIN_ITEM_SORT sort = MAIN_ITEM_SORT.NONE;

        switch (_sort)
        {
            case ITEM_SORT.MELEE:
            case ITEM_SORT.MACHINE_GUN:
            case ITEM_SORT.SHOT_GUN:
            case ITEM_SORT.ARMOR:
            case ITEM_SORT.RIFLE:
            case ITEM_SORT.SNIPER:
            case ITEM_SORT.LAUNCHER:
                sort = MAIN_ITEM_SORT.EQUIPMENT;
                break;
            case ITEM_SORT.GRENADE:
            case ITEM_SORT.HEALTH_PACK:
            case ITEM_SORT.AMMO:
                sort = MAIN_ITEM_SORT.QUICK;
                break;
            default:
                sort = MAIN_ITEM_SORT.ETC;
                break;
        }

        return sort;
    }


    public bool DeleteItemFromInventory(Item _item)
    {
       
        if (!m_ItemInventory.ContainsKey(_item.m_UniqueItemID))
        {
            Debug.LogError("그런 아이템은 인벤에 없는데? _item ID  : " + _item.m_UniqueItemID);
            return false;
        }

        MAIN_ITEM_SORT mainItemSort = ConvertSortToMainSort(_item.m_ItemStat.m_Sort);

        if (m_ItenSlotCount[mainItemSort] <= 0)
        {
            Debug.LogError("지울게 없는데 지운다?");
            return false;
        }

        for (int i = 0; i < m_ItemInventorySlot[mainItemSort].Count; i++)
        {
           ItemSlot slot = m_ItemInventorySlot[mainItemSort][i];

           if ( slot.m_Item.m_UniqueItemID == _item.m_UniqueItemID)
            {
               Item deleteItem = null;
               
                m_ItemInventory.Remove(slot.m_Item.m_UniqueItemID);
                slot.DetachItem();
                m_ItemInventorySlot[mainItemSort][i] = null;

                // 파괴 슬롯?

                break;
            }
        }

        m_ItenSlotCount[mainItemSort]--;

        return true;
    }

    public bool InsertItemToInventory(Item _item)
    {
        if(m_ItemInventory.ContainsKey(_item.m_UniqueItemID))
        {
            Debug.LogError("이미 존재하는 item 입니다 _item ID  : " + _item.m_UniqueItemID);
            return false;
        }


        MAIN_ITEM_SORT mainItemSort = ConvertSortToMainSort(_item.m_ItemStat.m_Sort);

        ItemSlot slot = FindEmptyInventorySlot(mainItemSort);

        if(slot == null)
        {
            Debug.Log("Inventory is full");
            return false;
        }

        m_ItemInventory.Add(_item.m_UniqueItemID, _item);
        slot.SetItem(_item);

        m_ItenSlotCount[mainItemSort]++;

        return true;

    }

    public bool CheckCurrentInvenTabFull(MAIN_ITEM_SORT _itemSort)
    {
        return m_ItenSlotCount[_itemSort] >= MAX_INVEN_SLOT ? true : false;
    }
    public void CheckCurrentInvenTabFull()
    {
        CheckCurrentInvenTabFull(m_CurSelectedInventoryTab);
    }

    public ItemSlot FindEmptyInventorySlot(MAIN_ITEM_SORT _itemSort)
    {
        if (CheckCurrentInvenTabFull(_itemSort)) return null;

        for(int i = 0; i < m_ItemInventorySlot.Count; i++)
        {
           if( m_ItemInventorySlot[_itemSort][i] == null)
            {
                return m_ItemInventorySlot[_itemSort][i];
            }
        }

        return null;
    }

    // 해당 슬롯에 아이템이 장착되어 있는가 함수
    public bool isEquipedItemSlot(ITEM_SLOT_SORT _slotSort)
    {
        return m_EquipedItemSlots[_slotSort] != null ? true : false;
    }

    public Item GetItemFromInven(int _itemUniqueID)
    {
        Item item = null;
        m_ItemInventory.TryGetValue(_itemUniqueID, out item);
        return item;
    }

    public void EquipItem(Item _item, ITEM_SLOT_SORT _slotSort)
    {
        DetachItem(_slotSort);
       
        m_EquipedItemSlots[_slotSort] = _item;
        _item.m_isEquiped = true;
    }

    public void EquipItem(int _itemUniqueID, ITEM_SLOT_SORT _slotSort)
    {
        Item item = GetItemFromInven(_itemUniqueID);

        if (item == null)
        {
            Debug.Log("그런 아이템은 인벤에 없어");
            return;
        }

        EquipItem(item, _slotSort);
    }


    public Item DetachItem(ITEM_SLOT_SORT _slotSort)
    {
        Item preEquipItem = null;

        if (isEquipedItemSlot(_slotSort))
        {
            preEquipItem = m_EquipedItemSlots[_slotSort];
            preEquipItem.m_isEquiped = false;

            if (m_ItemInventory.ContainsKey(preEquipItem.m_UniqueItemID))
            {
                m_ItemInventory[preEquipItem.m_UniqueItemID].m_isEquiped = false;
            }
            else
            {
                Debug.LogError("지금 끼고 아이템을 해제하려 하니깐 인벤에 없는 유니크 아이디다..? unique ID: " + preEquipItem.m_UniqueItemID);
            }

            m_EquipedItemSlots[_slotSort] = null;
        }
        else
        {
            Debug.LogError("그 슬롯에는 아이템이 없는데 아이템을 해제하려고 한다?? 슬롯 위치 : " + _slotSort);
            return null;
        }

        return preEquipItem;
    }






}

