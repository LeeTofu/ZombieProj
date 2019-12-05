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
    FIFTH, // 방어구류
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

    public float m_BulletSpeed; // 공격템인데 Bullet의 공속은?
    public string m_BulletString; // 공격템인데 Bullet의 string;
}

public class Item
{
    public System.Action<Vector3, Vector3, MovingObject> m_AttackMethod;
       
    int MAX_LEVEL = 10;
    public ItemStat m_ItemStat { get; private set; } // 아이템 능력치

    public int m_CurrentEXP { get; private set; } // 아이템의 경험치
    public int m_MaxEXP { get; private set; } // 최대 아이템 경첨치
    public int m_Lv { get; private set; } // 아이템 경험치
    public bool m_isEquiped { get; set; } // 이 아이템이 장착되었는가
    public int m_UniqueItemID { get; private set; } // 아이템 고유 아이디
    public int m_Count { get; private set; } // 중첩된 아이템 갯수

    public ITEM_SLOT_SORT m_ItemSlotType { get; set; } // 이 아이템이 장착되었다면 
    //어느 부위에 장착이 되었는지.

    public Item(int _ownedID, int _LV, int _curEXP, ITEM_SLOT_SORT _sort, ItemStat _stat) 
    {
        m_UniqueItemID = _ownedID;
        m_Lv = _LV;
        m_CurrentEXP = _curEXP;
        m_ItemStat = _stat;
        m_ItemSlotType = _sort;
    }

    public void SetAttackAction(System.Action<Vector3, Vector3, MovingObject> _action)
    {
        m_AttackMethod = _action;
    }

    public void EquipItem()
    {
        m_isEquiped = true;
    }

    public void Attack(Vector3 _pos, Vector3 _dir, MovingObject _itemMaster)
    {
        if (m_AttackMethod == null) return;

        m_AttackMethod(_pos, _dir, _itemMaster);
    }

}
