using System.Collections;
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

    BUFF,// Quick
    HEALTH_PACK, // Quick
    AMMO,// Quick
    GRENADE,// Quick
    FIRE_GRENADE,
    INSTALL_BOMB,

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

    public float m_HealthPoint; // 체력을 증가 시키는 정도
    public float m_AttackPoint; // 공격력
    public float m_ArmorPoint; // 방어력
    public float m_MoveSpeed; // 이동속도를 증가시키나
    public float m_HPGenerator; // 체력을 초당 치유하는가
    public float m_Range; // 거리
    public float m_AttackSpeed; //공속 (쿨타임.)
    public float m_CoolTime; // 쿨타임

    public bool m_isAccumulate; // 중첩이 되나 1,2,3...
    public bool m_isHaveCoolTime; // 쿨타임을 가진 아이템인가? -> 없다면 그냥 버튼 누르기만 해도 공격함.

    public bool m_isKnockBack; // 넉백 시키는가

    public float m_BulletSpeed; // 공격템인데 Bullet의 공속은? ()
    public string m_BulletString; // 공격템인데 Bullet의 string;

    public int m_Count; // 시작 카운트 , 시작 총알 갯수, 시작 갯수

    public string m_Info;

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

    public ITEM_SLOT_SORT m_ItemSlotType { get; set; } // 이 아이템이 장착되었다면 
                                                       //어느 부위에 장착이 되었는지.

    public Item(Item _item)
    {
        m_UniqueItemID = _item.m_UniqueItemID;
        m_Lv = _item.m_Lv;
        m_CurrentEXP = _item.m_CurrentEXP;
        m_ItemStat = _item.m_ItemStat;
        m_ItemSlotType = _item.m_ItemSlotType;
        m_Count = _item.m_Count;
    }

    public Item(int _ownedID, int _LV, int _curEXP, ITEM_SLOT_SORT _sort, ItemStat _stat) 
    {
        m_UniqueItemID = _ownedID;
        m_Lv = _LV;
        m_CurrentEXP = _curEXP;
        m_ItemStat = _stat;
        m_ItemSlotType = _sort;
        m_Count = _stat.m_Count;
    }

    public void EquipItem()
    {
        m_isEquiped = true;
    }

    public void spendItem()
    {
        m_Count -= 1;

        if(m_Count < 0)
        {
            m_Count = 0;
        }
    }

    public void plusItem(int _accum)
    {
        m_Count += _accum;
    }

}
