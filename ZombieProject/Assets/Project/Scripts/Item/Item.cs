using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MAIN_ITEM_SORT
{
    NONE = 0,
    EQUIPMENT, // 장비
    QUICK, // 일회용
    ETC, // 기타 아이템
    END

}

public enum ITEM_SORT
{
    NONE = 0,
    MELEE = 1,
    SNIPER,
    RIFLE,
    MACHINE_GUN,
    LAUNCHER,
    SHOT_GUN,

    HEALTH_PACK,
    ARMOR,
    AMMO,
    GRENADE,

    END
    
}

public enum ITEM_SLOT_SORT
{
    NONE,
    MAIN,
    SECOND,
    THIRD,
    FOURTH,
    END
}

public struct ItemStat
{
    public int m_ItemID;

    public string m_ItemName;
    public string m_IconTexrureID;
    public string m_ModelString;

    public ITEM_SORT m_Sort;

    public float m_HealthPoint;
    public float m_AttackPoint;
    public float m_ArmorPoint;
    public float m_MoveSpeed;
    public float m_HPGenerator;
    public float m_Range;
    public float m_AttackSpeed;

    public bool m_isAccumulate;
}

public class Item
{
    int MAX_LEVEL = 10;
    public ItemStat m_ItemStat { get; private set; }

    public int m_CurrentEXP { get; private set; }
    public int m_MaxEXP { get; private set; }
    public int m_Lv { get; private set; }
    public bool m_isEquiped { get; set; }
    public int m_UniqueItemID { get; private set; }

    public ITEM_SLOT_SORT m_ItemSlotType { get; private set; }

    public Item(int _ownedID, int _LV, int _curEXP, ItemStat _stat) 
    {
        m_UniqueItemID = _ownedID;
        m_Lv = _LV;
        m_CurrentEXP = _curEXP;
        m_ItemStat = _stat;
    }


    public void EquipItem()
    {
        m_isEquiped = true;
    }


    //// 인챈트는 서버에서 이루어져야 하는 작업...
    //public void ItemEnchant(int _exp)
    //{
    //    if (m_Lv == MAX_LEVEL) return;

    //    int PlusExp = _exp;

    //    while (PlusExp > 0)
    //    {
    //        if (m_CurrentEXP + PlusExp >= ItemManager.Instance.MaxEXP[m_Lv])
    //        {
    //            int curPlusExp =  (ItemManager.Instance.MaxEXP[m_Lv] - m_CurrentEXP);
    //            PlusExp -= curPlusExp;
    //            PlusItemLevel(1);
    //        }

    //        // Item의 최대 증가값 가져온다고 가정.
    //    }

    //    // exp 올라가는 연출 및 
    //    // DataBase 연동해서 아이템의 레벨을 올릴 것.
    //}

    //public void PlusItemLevel(int _Level)
    //{
    //    if (m_Lv == MAX_LEVEL) return;

    //    m_Lv += _Level;
    //    m_CurrentEXP = 0;
    //    m_MaxEXP = ItemManager.Instance.MaxEXP[m_Lv]; // DB에서 아이템의 최대값 연산 한 뒤에 가져와야
    //}

}
