using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ITEM_EVENT_TYPE
{
    FIRE_BULLET, // 불릿을 쏘는 아이템 타입
    MELEE, // 근접 공격하는 아이템 타입
    THROW_ARK, // 포물선으로 던지는 타입.
    END


};


public class ActionTypeManager : Singleton<ActionTypeManager>
{
    Dictionary<ITEM_EVENT_TYPE, System.Action<MovingObject, Item>> m_ButtonPressTable = new Dictionary<ITEM_EVENT_TYPE, System.Action<MovingObject, Item>>(); // 버튼을 누른채의 상태들을 담은 테이블
    Dictionary<ITEM_EVENT_TYPE, System.Action<MovingObject, Item>> m_ButtonReleaseTable = new Dictionary<ITEM_EVENT_TYPE, System.Action<MovingObject, Item>>(); // 버튼을 풀었을 때의 액션을 담은 테이블
    Dictionary<ITEM_EVENT_TYPE, System.Action<MovingObject, Item>> m_ButtonPressDownTable = new Dictionary<ITEM_EVENT_TYPE, System.Action<MovingObject, Item>>(); //버튼 눌렀을 때의 액션을 담은 테이블

    ObjectFactory m_BulletFactory;

    public override bool Initialize()
    {
        m_BulletFactory = gameObject.AddComponent<ObjectFactory>();
        m_BulletFactory.Initialize(30, "Prefabs/Weapon/Bullet/TestBullet" ,"Prefabs/Weapon/Bullet/Models");

        for (ITEM_EVENT_TYPE eventType = ITEM_EVENT_TYPE.FIRE_BULLET; eventType != ITEM_EVENT_TYPE.END; eventType++)
        {
            switch (eventType)
            {
                case ITEM_EVENT_TYPE.FIRE_BULLET:
                    {
                        m_ButtonPressTable.Add(eventType, (MovingObject character, Item _item) =>
                        {
                            MovingObject newBulletObj = m_BulletFactory.CreateObject(Vector3.zero, Quaternion.identity);
                            Bullet newBullet = newBulletObj as Bullet;

                            Vector3 ScreenToWorldPos;
                            MovingObject enemy = PlayerManager.Instance.GetTouchNearestEnemy(Input.mousePosition, out ScreenToWorldPos);

                            ScreenToWorldPos.y = character.transform.position.y;

                            if(character.m_CurrentEquipedItem != null)
                                character.m_CurrentEquipedItem.PlaySound();
                            
                            if (newBullet)
                            {
                                newBullet.FireBullet(
                                    character.m_CurrentEquipedItem.m_FireTransform.position,
                                    character.transform.forward, _item.m_ItemStat.m_BulletSpeed);
                            };
                        });
                    }
                    break;
                case ITEM_EVENT_TYPE.THROW_ARK:
                    {
                        m_ButtonPressTable.Add(eventType, (MovingObject character, Item _item) =>
                        {
                            character.transform.rotation = Quaternion.LookRotation(character.transform.forward, Vector3.up);

                            MovingObject newBulletObj = m_BulletFactory.CreateObject(Vector3.zero, Quaternion.identity);
                            Bullet newBullet = newBulletObj as Bullet;

                            if (newBullet)
                            {
                                newBullet.FireBullet(
                                    character.m_CurrentEquipedItem.m_FireTransform.position,
                                    character.transform.forward, _item.m_ItemStat.m_BulletSpeed);
                            };
                        });
                    }
                    break;
            }
        }


        return true;
    }

    public System.Action<MovingObject, Item> GetActionType(BUTTON_ACTION _action, ITEM_EVENT_TYPE _eventType )
    {
        System.Action< MovingObject, Item> action = null;

        switch (_action)
        {
            case BUTTON_ACTION.PRESS_DOWN:
                if (m_ButtonPressTable.TryGetValue(_eventType, out action))
                    return action;
                else return null;
            case BUTTON_ACTION.PRESS_ENTER:
                if (m_ButtonPressDownTable.TryGetValue(_eventType, out action))
                    return action;
                else return null;
            case BUTTON_ACTION.PRESS_RELEASE:
                if (m_ButtonReleaseTable.TryGetValue(_eventType, out action))
                    return action;
                else return null;
            default:
                return null;

        }
    }
    
    public void SetItemActionType()
    {
        
    }








}
