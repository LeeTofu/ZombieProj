using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class ItemManager : Singleton<ItemManager>
{
    Dictionary<int, ItemStat> m_ItemTable = new Dictionary<int, ItemStat>();

    const string m_xmlFileName = "itemData";
    public int[] MaxEXP =
    {
        100, 200, 300, 400, 500, 600, 700, 800, 900, 1000
    };

    public override bool Initialize()
    {
        // Item data를 파싱 받아서 저장해야함.
        // 서버도 
       // LoadXML(m_xmlFileName);
        return true;
    }

    public void LoadItemData()
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("Data/Item/ItemData");
        XmlDocument xmlDoc = new XmlDocument();
        Debug.Log(txtAsset.text);
        xmlDoc.LoadXml(txtAsset.text);

        XmlNodeList all_nodes = xmlDoc.SelectNodes("root/Item");
        foreach (XmlNode node in all_nodes)
        {
            ItemStat itemStat;
            itemStat.m_ItemID = int.Parse(node.SelectSingleNode("ItemID").InnerText );

            itemStat.m_ItemName = node.SelectSingleNode("ItemName").InnerText;
            itemStat.m_IconTexrureID =node.SelectSingleNode("IconName").InnerText;
            itemStat.m_ModelString = node.SelectSingleNode("ModelName").InnerText;

            itemStat.m_Sort = (ITEM_SORT)int.Parse(node.SelectSingleNode("ItemSort").InnerText) ;

            itemStat.m_MoveSpeed = float.Parse(node.SelectSingleNode("MovePoint").InnerText);
            itemStat.m_Range = float.Parse(node.SelectSingleNode("Range").InnerText);
            itemStat.m_HPGenerator = float.Parse(node.SelectSingleNode("HPRegenerate").InnerText);
            itemStat.m_AttackPoint = float.Parse(node.SelectSingleNode("Attack").InnerText);
            itemStat.m_ArmorPoint = float.Parse(node.SelectSingleNode("Armor").InnerText);
            itemStat.m_AttackSpeed = float.Parse(node.SelectSingleNode("AttackSpeed").InnerText);
            itemStat.m_HealthPoint = float.Parse(node.SelectSingleNode("HP").InnerText);

            itemStat.m_isAccumulate = bool.Parse(node.SelectSingleNode("isAccumulate").InnerText);
            
            m_ItemTable.Add(itemStat.m_ItemID, itemStat);
        }
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
