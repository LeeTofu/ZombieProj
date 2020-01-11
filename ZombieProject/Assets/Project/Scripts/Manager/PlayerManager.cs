﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;


public class PlayerManager : Singleton<PlayerManager>
{
    public struct PlayerInfo
    {
        public int PlayerID;
        public int Lv;
        public int Money;
        public int Parts;
        public int exp;
        public string playerName;
    }

    public Item m_MainItem;
    public Item m_SecondaryItem;


    [SerializeField]
    private GameObject m_PlayerHeadModelPrefabs;

    [SerializeField]
    private GameObject m_PlayerBodyModelPrefabs;

    public MovingObject m_Player;

    // 플레이어를 도울 용병등등... 드론..? 용병?
    public List<MovingObject> m_PlayableObject = new List<MovingObject>();

    private ObjectFactory m_PlayerFactory;

    [HideInInspector]
    public GameObject m_PlayerCreateZone;

    public PlayerInfo m_PlayerInfo;


    public void LoadPlayerInfo()
    {
        TextAsset playerInvenList = (TextAsset)Resources.Load("Data/Player/PlayerInfo");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(playerInvenList.text);

        XmlNodeList all_nodes = xmlDoc.SelectNodes("Root/text");

        foreach (XmlNode node in all_nodes)
        {
            int playerID = int.Parse(node.SelectSingleNode("PlayerID").InnerText);
            int Lv = int.Parse(node.SelectSingleNode("Level").InnerText);
            int Money = int.Parse(node.SelectSingleNode("Money").InnerText);
            int Parts = int.Parse(node.SelectSingleNode("Parts").InnerText);
            int exp = int.Parse(node.SelectSingleNode("Exp").InnerText);
            string playerName = node.SelectSingleNode("PlayerName").InnerText;

            m_PlayerInfo.PlayerID = playerID;
            m_PlayerInfo.Lv = Lv;
            m_PlayerInfo.Money = Money;
            m_PlayerInfo.Parts = Parts;
            m_PlayerInfo.exp = exp;
            m_PlayerInfo.playerName = playerName;
        }
    }

    public MovingObject CreatePlayer(Vector3 _pos, Quaternion _quat)
    {
        if (m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        // 설정 //
        m_Player = m_PlayerFactory.PopObject(_pos, _quat, (int)OBJECT_TYPE.PLAYER);

        m_MainItem = InvenManager.Instance.GetEquipedItemSlot(ITEM_SLOT_SORT.MAIN);
        m_SecondaryItem = InvenManager.Instance.GetEquipedItemSlot(ITEM_SLOT_SORT.SECOND);

        m_Player.SetWeapon(m_MainItem == null ? m_SecondaryItem : m_MainItem);

        
      
        return m_Player;
    }

    public override bool Initialize()
    {
        LoadPlayerInfo();

        if (m_PlayerFactory == null)
        {
            m_PlayerFactory = gameObject.AddComponent<ObjectFactory>();
            m_PlayerFactory.Initialize("Prefabs/Players/Player", Resources.LoadAll<GameObject>("Prefabs/Players/Models/Normal"));
            m_PlayerFactory.CreateObjectPool((int)OBJECT_TYPE.PLAYER, 1);
        }

        m_PlayerCreateZone = GameObject.Find("PlayerCreateZone");

        if (m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        return true;
    }

    // 해당 터치 위치가 플레이어가 공격할 수 있는 시야각에 있는지 체크하는 함수입니다.
    public bool CheckCanAttack(Vector3 _inputScreenPosition)
    {
        if (m_Player == null) return false;

        Ray ray = Camera.main.ScreenPointToRay(_inputScreenPosition);
        RaycastHit castHit;
        if(Physics.Raycast(ray, out castHit, 100.0f, 1 << LayerMask.NameToLayer("Ground") ))
        {
            Vector3 HitPositon = castHit.point;
            HitPositon.y = m_Player.transform.position.y;
            Vector3 HitForward = HitPositon - m_Player.transform.position;

            HitForward = HitForward.normalized;

            if (Vector3.Dot(HitForward, m_Player.transform.forward) > 0.8f)
            {
                return true;
            }
            else return false;
        }

        return false;
    }


    // 화면 터치시 가장 가까운 좀비를 찾아내는 함수입니다.
    public MovingObject GetTouchNearestEnemy(Vector3 _inputScreenPosition, out Vector3 _hitPoint)
    {
        _hitPoint = Vector3.zero;

        if (m_Player == null) return null;

        Ray ray = Camera.main.ScreenPointToRay(_inputScreenPosition);
        RaycastHit castHit;
        if (Physics.Raycast(ray, out castHit, 100.0f, 1 << LayerMask.NameToLayer("Ground")))
        {
            _hitPoint = castHit.point;

            MovingObject zombie = EnemyManager.Instance.GetNearestZombie(_hitPoint, 1.0f);

            if (zombie == null)
            {
                return null;
            }
            else
            {
                Debug.LogError("Zombie 감지" + zombie.gameObject.name);
                return zombie;
            }
        }

        return null;
    }

    public MovingObject GetNearestZombie(Vector3 _playerPos)
    {
        MovingObject zombie = EnemyManager.Instance.GetNearestZombie(_playerPos, 1.0f);

        if (zombie == null)
        {
            return null;
        }
        else
        {
            return zombie;
        }
    }

    public void PlayerChangeWeapon()
    {
        Item item = InvenManager.Instance.GetEquipedItemSlot(ITEM_SLOT_SORT.SECOND);
        if(item != null)
        {
            m_Player.SetWeapon(item);
        }
    }

    public void PlayerAttack()
    {
        if (m_Player.m_CurrentEquipedItem != null)
            m_Player.m_CurrentEquipedItem.ItemAction();
    }

    public void PlayerUseItem(ITEM_SLOT_SORT _type)
    {
        if (_type == ITEM_SLOT_SORT.MAIN || _type == ITEM_SLOT_SORT.SECOND) return;

        Item item = InvenManager.Instance.GetEquipedItemSlot(_type);

        if (item == null) return;

        switch(item.m_ItemStat.m_Sort)
        {
            case ITEM_SORT.HEALTH_PACK:
                (m_Player as PlayerObject).ChangeState(E_PLAYABLE_STATE.DRINK);
                break;
            case ITEM_SORT.BUFF:
                (m_Player as PlayerObject).ChangeState(E_PLAYABLE_STATE.DRINK);
                break;
        }
    }


}
