using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;

public class InvenManager : Singleton<InvenManager>
{
    public InventorySceneMain m_Main { private set; get; }
    public InventoryUI m_UI { private set; get; }

    GameObject m_ItemSlotPrefab;
    public MovingObject m_Player { get; private set; }
    public MAIN_ITEM_SORT m_CurSelectedInventoryTab { get; private set; }

    // 인벤 슬롯에 있는 아이템 갯수
    public Dictionary<MAIN_ITEM_SORT, int> m_ItemSlotCount = new Dictionary<MAIN_ITEM_SORT, int>();
    public Dictionary<MAIN_ITEM_SORT, List<ItemSlot>> m_ItemInventorySlot = new Dictionary<MAIN_ITEM_SORT, List<ItemSlot>>();
    Dictionary<int, Item> m_ItemInventory = new Dictionary<int, Item>();

    // 게임내 장착된 아이템 
    public Dictionary<ITEM_SLOT_SORT, Item> m_EquipedItemSlots = new Dictionary<ITEM_SLOT_SORT, Item>();

    public const int MAX_INVEN_SLOT = 30;

    public const int MAX_INVEN_ROW = 10;
    public const int MAX_INVEN_COL = 3;

 
    public override bool Initialize()
    {
        m_ItemSlotPrefab = Resources.Load<GameObject>("Prefabs/ItemUI/ItemSlotUI");

        m_ItemSlotCount.Clear();

        m_ItemSlotCount.Add(MAIN_ITEM_SORT.EQUIPMENT, 0);
        m_ItemSlotCount.Add(MAIN_ITEM_SORT.QUICK, 0);
        m_ItemSlotCount.Add(MAIN_ITEM_SORT.ETC, 0);

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

        m_UI = UIManager.Instance.GetUIObject(GAME_SCENE.INVENTORY).GetComponent<InventoryUI>();
        m_Main = SceneMaster.Instance.GetGameMain(GAME_SCENE.INVENTORY) as InventorySceneMain;

        m_EquipedItemSlots.Clear();
        m_EquipedItemSlots.Add(ITEM_SLOT_SORT.MAIN, null);
        m_EquipedItemSlots.Add(ITEM_SLOT_SORT.SECOND, null);
        m_EquipedItemSlots.Add(ITEM_SLOT_SORT.THIRD, null);
        m_EquipedItemSlots.Add(ITEM_SLOT_SORT.FOURTH, null);
        m_EquipedItemSlots.Add(ITEM_SLOT_SORT.FIFTH, null);

        m_CurSelectedInventoryTab = MAIN_ITEM_SORT.END;
        InitializeInventoryTab(MAIN_ITEM_SORT.NONE);

        if (!m_UI)
        {
            Debug.LogError("Shop UI Object Error");
            return false;
        }

        for (
            MAIN_ITEM_SORT e = MAIN_ITEM_SORT.EQUIPMENT;
            e < MAIN_ITEM_SORT.END; 
            e++)
        {
            for (int r = 0; r < MAX_INVEN_ROW; r++)
            {
                for (int c = 0; c < MAX_INVEN_COL; c++)
                {
                    ItemSlot slot = m_ItemInventorySlot[e][r * MAX_INVEN_COL + c];
                    m_UI.InsertToScroll(slot,c,r);

                }
            }
        }

        LoadItemInvenFromXML();
        return true;
    }
    public override void DestroyManager()
    {
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
    public void LoadItemInvenFromXML()
    {
        TextAsset playerInvenList = (TextAsset)Resources.Load("Data/Inventory/PlayerInventoryList");
        XmlDocument xmlDoc = new XmlDocument();

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
                item = new Item(uniqueID, Lv, currentEXP, slotSort, stat);
              //  ActionTypeManager.Instance.SetItemActionType(item);

                bool result = InsertItemToInventory(item);

                if (result)
                {
                    if (isEquiped && slotSort != ITEM_SLOT_SORT.NONE)
                    {
                        EquipItem(uniqueID, slotSort, null);
                    }
                }
            }
        }
    }

    public void InitializeInventoryTab(MAIN_ITEM_SORT _sort)
    {
        if (_sort == m_CurSelectedInventoryTab) return;

        if (_sort != MAIN_ITEM_SORT.NONE)
        {
            for(MAIN_ITEM_SORT i = MAIN_ITEM_SORT.EQUIPMENT; i < MAIN_ITEM_SORT.END; i++)
            {
                if (i == _sort)
                {
                    foreach (ItemSlot slot in m_ItemInventorySlot[i])
                    {
                        slot.gameObject.SetActive(true);
                    }
                }
                else
                {
                    foreach (ItemSlot slot in m_ItemInventorySlot[i])
                    {
                        slot.gameObject.SetActive(false);
                    }
                }
            }
        }
        else if (_sort == MAIN_ITEM_SORT.NONE)
        {
            for (MAIN_ITEM_SORT i = MAIN_ITEM_SORT.EQUIPMENT; i < MAIN_ITEM_SORT.END; i++)
            {
                foreach (ItemSlot slot in m_ItemInventorySlot[i])
                {
                    slot.gameObject.SetActive(false);
                }
            }
        }

        m_CurSelectedInventoryTab = _sort;
    }


    public MAIN_ITEM_SORT ConvertSortToMainSort(ITEM_SORT _sort)
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
            case ITEM_SORT.INSTALL_BOMB:
            case ITEM_SORT.FIRE_GRENADE:
            case ITEM_SORT.GRENADE:
            case ITEM_SORT.HEALTH_PACK:
            case ITEM_SORT.AMMO:
            case ITEM_SORT.BUFF:
                sort = MAIN_ITEM_SORT.QUICK;
                break;
            default:
                sort = MAIN_ITEM_SORT.ETC;
                break;
        }

        return sort;
    }

    public MAIN_ITEM_SORT ConvertSortToMainSort(ITEM_SLOT_SORT _sort)
    {
        MAIN_ITEM_SORT sort = MAIN_ITEM_SORT.NONE;

        switch (_sort)
        {
            case ITEM_SLOT_SORT.MAIN:
            case ITEM_SLOT_SORT.SECOND:
                sort = MAIN_ITEM_SORT.EQUIPMENT;
                break;
            case ITEM_SLOT_SORT.THIRD:
                sort = MAIN_ITEM_SORT.QUICK;
                break;
            case ITEM_SLOT_SORT.FOURTH:
                sort = MAIN_ITEM_SORT.QUICK;
                break;
            case ITEM_SLOT_SORT.FIFTH:
                sort = MAIN_ITEM_SORT.QUICK;
                break;
        }

        return sort;
    }

    public ItemSlot GetItemSlot(MAIN_ITEM_SORT _slotType, int _uniqueItemID)
    {
       List<ItemSlot> listItem =  m_ItemInventorySlot[_slotType];

        if (m_ItemSlotCount[_slotType] == 0)
            return null;

        for(int i = 0; i < m_ItemInventorySlot[_slotType].Count; i++)
        {
            if (listItem[i] == null) continue;

            if(_uniqueItemID == listItem[i].m_Item.m_UniqueItemID)
            {
                return listItem[i];
            }
        }
        return null;
    }


    public bool DeleteItemFromInventory(Item _item)
    {
        if (!m_ItemInventory.ContainsKey(_item.m_UniqueItemID))
        {
            Debug.LogError("그런 아이템은 인벤에 없는데? _item ID  : " + _item.m_UniqueItemID);
            return false;
        }

        MAIN_ITEM_SORT mainItemSort = ConvertSortToMainSort(_item.m_ItemStat.m_Sort);

        if (m_ItemSlotCount[mainItemSort] <= 0)
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
                slot.SetItem(null);
                m_ItemInventorySlot[mainItemSort][i] = null;

                // 파괴 슬롯?

                break;
           }
        }

        m_ItemSlotCount[mainItemSort]--;

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

        m_ItemSlotCount[mainItemSort]++;

        return true;

    }

    public bool CheckCurrentInvenTabFull(MAIN_ITEM_SORT _itemSort)
    {
        return m_ItemSlotCount[_itemSort] >= MAX_INVEN_SLOT ? true : false;
    }
    public void CheckCurrentInvenTabFull()
    {
        CheckCurrentInvenTabFull(m_CurSelectedInventoryTab);
    }

    public ItemSlot FindEmptyInventorySlot(MAIN_ITEM_SORT _itemSort)
    {
        if (CheckCurrentInvenTabFull(_itemSort))
        {
            Debug.LogError("sws");
            return null;
        }
        for(int i = 0; i < m_ItemInventorySlot[_itemSort].Count; i++)
        {
           if( m_ItemInventorySlot[_itemSort][i].m_Item == null)
            {
                return m_ItemInventorySlot[_itemSort][i];
            }
        }
        Debug.LogError("sws");
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

    public bool EquipItem(int _itemUniqueID, ITEM_SLOT_SORT _slotSort, ItemSlot _EquipmentSlot)
    {
        // 장착 슬롯의 아이템 슬롯으로 장착은 못해요.
    //    if (_EquipmentSlot != null && _EquipmentSlot.m_isEquipmentItemSlot) return false;

        Item item = GetItemFromInven(_itemUniqueID);

        if (item == null)
        {
            Debug.LogError("그런 아이템은 인벤에 없어");
            return false;
        }

        if (isEquipedItemSlot(_slotSort))
        {
           DetachItem(_slotSort, _EquipmentSlot);
        }
        if(item.m_isEquiped)
        {
           DetachItem(item.m_ItemSlotType, m_UI.m_ItemEquipmentSlot[(int)item.m_ItemSlotType - 1]);
        }
     //   if(item.m_isEquiped == true)

        m_EquipedItemSlots[_slotSort] = item;
        item.m_isEquiped = true;
        item.m_ItemSlotType = _slotSort;

        if (m_Main != null && m_UI.m_SelectedSlot)
            m_UI.m_SelectedSlot.EquipItem();

        if(_EquipmentSlot)
        {
            _EquipmentSlot.SetItem(item);
        }

        return true;

    }

    public Item GetEquipedItemSlot(ITEM_SLOT_SORT _slot)
    {
        return m_EquipedItemSlots[_slot];
    }


    public Item DetachItem(ITEM_SLOT_SORT _slotSort, ItemSlot _EquipmentSlot)
    {
        Item preEquipItem = null;

        Debug.Log("여기는 와애ㅑ지");
        if (isEquipedItemSlot(_slotSort))
        {
            preEquipItem = m_EquipedItemSlots[_slotSort];
            preEquipItem.m_isEquiped = false;

            if (_EquipmentSlot)
            {
                _EquipmentSlot.SetItem(null);
            }

            if (m_ItemInventory.ContainsKey(preEquipItem.m_UniqueItemID))
            {
                Item item = GetEquipedItemSlot(_slotSort);

                if (item != null)
                {
                    Debug.Log("들오나");
                    //  m_ItemEquipmentSlot[(int)_slotType].SetItem(null);
                    MAIN_ITEM_SORT sort = ConvertSortToMainSort(_slotSort);
                    ItemSlot itemSlot = GetItemSlot(sort, item.m_UniqueItemID);

                    itemSlot.DetachItem();
                }

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

