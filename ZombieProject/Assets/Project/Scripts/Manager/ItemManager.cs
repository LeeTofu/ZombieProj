using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class ItemManager : Singleton<ItemManager>
{
    Dictionary<int, ItemStat> m_ItemTable = new Dictionary<int, ItemStat>();

    public bool m_isParsingCompelete = false;

    const string m_xmlFileName = "itemData";
    public int[] MaxEXP =
    {
        100, 200, 300, 400, 500, 600, 700, 800, 900, 1000
    };

    public override bool Initialize()
    {
        // Item data를 파싱 받아서 저장해야함.
        // 서버도 
        LoadItemData();
        return true;
    }

    public void LoadItemData()
    {
        if (m_isParsingCompelete) return;

        TextAsset txtAsset = (TextAsset)Resources.Load("Data/Item/ItemData");
        XmlDocument xmlDoc = new XmlDocument();
        Debug.Log(txtAsset.text);
        xmlDoc.LoadXml(txtAsset.text);

        XmlNodeList all_nodes = xmlDoc.SelectNodes("Root/text");
        foreach (XmlNode node in all_nodes)
        {
            ItemStat itemStat;
            itemStat.m_ItemID = int.Parse(node.SelectSingleNode("ItemID").InnerText );

            itemStat.m_ItemName = node.SelectSingleNode("ItemName").InnerText;
            itemStat.m_IconTexrureID =node.SelectSingleNode("IconName").InnerText;
            itemStat.m_ModelString = node.SelectSingleNode("ModelName").InnerText;

            bool success = System.Enum.TryParse<ITEM_SORT>(node.SelectSingleNode("ItemSort").InnerText, out itemStat.m_Sort);
            if (!success)
            {
                Debug.LogError("Item Parse Fail ItemType 확인할 것 : " + itemStat.m_Sort);
                continue;
            }

            itemStat.m_MoveSpeed = float.Parse(node.SelectSingleNode("MovePoint").InnerText);
            itemStat.m_Range = float.Parse(node.SelectSingleNode("Range").InnerText);
            itemStat.m_HPGenerator = float.Parse(node.SelectSingleNode("HPRegenerate").InnerText);
            itemStat.m_AttackPoint = float.Parse(node.SelectSingleNode("Attack").InnerText);
            itemStat.m_ArmorPoint = float.Parse(node.SelectSingleNode("Armor").InnerText);
            itemStat.m_AttackSpeed = float.Parse(node.SelectSingleNode("AttackSpeed").InnerText);
            itemStat.m_HealthPoint = float.Parse(node.SelectSingleNode("HP").InnerText);

            itemStat.m_isAccumulate = bool.Parse(node.SelectSingleNode("isAccumulate").InnerText);

            itemStat.m_isRayAttack = true;
            itemStat.m_BulletSpeed = 20.0f;
            itemStat.m_BulletString = "TestBullet";
            
            m_ItemTable.Add(itemStat.m_ItemID, itemStat);
            Debug.Log(itemStat.m_ItemID);

           
        }
        m_isParsingCompelete = true;
    }

    public void DebuggingLogItem(int _itemID)
    {
        if (m_ItemTable.ContainsKey(_itemID) == false)
        {
            Debug.LogError("그런 아이템 없습니다.");
            return;
        }

        ItemStat stat = m_ItemTable[_itemID];
        Debug.Log("Armor : " + stat.m_ArmorPoint);
        Debug.Log("Attack : " + stat.m_AttackPoint);
        Debug.Log("AttackSpeed : " + stat.m_AttackSpeed);
        Debug.Log("HP : " + stat.m_HealthPoint);
        Debug.Log("HPRegenrate : " + stat.m_HPGenerator);
        Debug.Log("IconTexture : " + stat.m_IconTexrureID);
        Debug.Log("Acc : " + stat.m_isAccumulate);
        Debug.Log("ItemID : " + stat.m_ItemID);
        Debug.Log("ItemName: " + stat.m_ItemName);
        Debug.Log("ModelString : " + stat.m_ModelString);
        Debug.Log("MoveSpeed : " + stat.m_MoveSpeed);
        Debug.Log("Range : " + stat.m_Range);
        Debug.Log("Sort : " + stat.m_Sort);
    }

    public ItemStat GetItemStat(int _ItemID)
    {
        ItemStat stat;
        if(!m_ItemTable.TryGetValue(_ItemID, out stat))
        {
            Debug.LogError("그런 아이템 없다... 아이템 아디 : " + _ItemID);
            stat.m_ItemID = -1;
        }

        return stat;
    }


    public void ItemEnchant(Item _item, int _exp)
    {
      // _item.ItemEnchant(_exp);
    }

    public void PlusItemLevelUp(Item _item, int _Level)
    {
        
    }
}
