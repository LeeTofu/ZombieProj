using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eDROP_ITEM
{
    ADRENALIN, // 공속,이속 증가
    HEALTH, // 체력 회복
    POISON // 독
}

public class DropItem : MovingObject
{
    [SerializeField]
    eDROP_ITEM m_dropItem;

    // 이 아이템을 먹으면 주는 버프
    Buff m_Buff;

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        AddCollisionCondtion(CollisionCondition);
        AddCollisionFunction(CollisionEvent);

       
    }

    void CollisionEvent(GameObject _object)
    {
        MovingObject player = _object.GetComponent<MovingObject>();

        switch (m_dropItem)
        {
            case eDROP_ITEM.ADRENALIN:
                {
                    m_Buff = new Adrenaline(player.m_Stat);
                }
                break;
            case eDROP_ITEM.HEALTH:
                {
                    m_Buff = new Blessing(player.m_Stat);

                }
                break;
            case eDROP_ITEM.POISON:
                {
                    m_Buff = new Poison(player.m_Stat);

                }
                break;
        }



        player.AddBuff(m_Buff);

        Debug.Log("들어감");

        //pushToMemory();
    }

    bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (_defender.tag != "Player") return false;

        return true;
    }
}
