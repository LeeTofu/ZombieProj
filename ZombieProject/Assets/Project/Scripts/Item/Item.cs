﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 아이템 큰 종류 목록
public enum MAIN_ITEM_SORT
{
    NONE = 0,
    EQUIPMENT, // 장비
    QUICK, // 일회용
    ETC, // 기타 아이템
    END

}

// 아이템 세부 종류 목록
public enum ITEM_SORT
{
    NONE = 0,
    MELEE = 1, // Equipment
    SNIPER,// Equipment
    RIFLE,// Equipment
    MACHINE_GUN,// Equipment
    LAUNCHER,// Equipment
    SHOT_GUN,// Equipment
    ARMOR, // Equipment

    HEALTH_PACK, // Quick
    AMMO,// Quick
    GRENADE,// Quick

    END
    
}

// 장착할 아이템 위치 슬롯 위치 
// 인게임에는 4개의 아이템 슬롯이 있음. 그 슬롯의 목록임.
public enum ITEM_SLOT_SORT
{
    NONE,
    MAIN, // 메인 무기
    SECOND, // 세컨더리 무기
    THIRD, // 헬스팩이나 1회용 아이템
    FOURTH, // 헬스팩이나 1회용 아이템
    END
}

public struct ItemStat
{
    public int m_ItemID; // 아이템 아이디

    public string m_ItemName; // 아이템 이름
    public string m_IconTexrureID; // 아이콘 텍스처 이름
    public string m_ModelString; // 모델 이름

    public ITEM_SORT m_Sort;

    public float m_HealthPoint; // 체력
    public float m_AttackPoint; // 공격력
    public float m_ArmorPoint; // 방어력
    public float m_MoveSpeed; // 이동속도를 증가시키나
    public float m_HPGenerator; // 체력을 초당 치유하는가
    public float m_Range; // 거리
    public float m_AttackSpeed; //공속

    public bool m_isAccumulate; // 중첩이 되나 1,2,3...
}

public class Item
{
    int MAX_LEVEL = 10;
    public ItemStat m_ItemStat { get; private set; } // 아이템 능력치

    public int m_CurrentEXP { get; private set; } // 아이템의 경험치
    public int m_MaxEXP { get; private set; } // 최대 아이템 경첨치
    public int m_Lv { get; private set; } // 아이템 경험치
    public bool m_isEquiped { get; set; } // 이 아이템이 장착되었는가
    public int m_UniqueItemID { get; private set; } // 아이템 고유 아이디
    public int m_Count { get; private set; } // 중첩된 아이템 갯수

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
