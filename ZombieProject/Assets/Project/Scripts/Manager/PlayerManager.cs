using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum UPGRADE_TYPE
{
    NONE,
    ATTACK,
    RANGE,
    ATTACK_SPEED
}


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

    public ItemObject m_MainEquipedItemObject;
    public ItemObject m_SecondEquipedItemObject;

    public ItemObject m_CurrentEquipedItemObject;

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

    // 현재 가진 돈
    private int m_CurrentMoney;

    public int CurrentMoney
    {
        get => m_CurrentMoney;
        set
        {
            m_CurrentMoney = value;

            if(m_CurrentMoney < 0)
            {
                m_CurrentMoney = 0;
            }

            (UIManager.Instance.m_CurrentUI as BattleUI).UpdateMoney(m_CurrentMoney);
        }
    }


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


    // Player 게임 시작시에 무기 셋팅 하는 부분.
    void PlayerWeaponInitialize()
    {
        if (m_MainEquipedItemObject != null)
        {
            Destroy(m_MainEquipedItemObject);
            m_MainEquipedItemObject = null;
        }
        if (m_SecondEquipedItemObject != null)
        {
            Destroy(m_SecondEquipedItemObject);
            m_SecondEquipedItemObject = null;
        }

        m_CurrentEquipedItemObject = null;

        m_MainItem = InvenManager.Instance.GetEquipedItemSlot(ITEM_SLOT_SORT.MAIN);
        m_SecondaryItem = InvenManager.Instance.GetEquipedItemSlot(ITEM_SLOT_SORT.SECOND);

        m_MainEquipedItemObject = ItemManager.Instance.CreateItemObject(m_MainItem);
        m_SecondEquipedItemObject = ItemManager.Instance.CreateItemObject(m_SecondaryItem);

        m_CurrentEquipedItemObject = m_MainEquipedItemObject == null ? m_SecondEquipedItemObject : m_MainEquipedItemObject;

        if (m_MainEquipedItemObject != null)
            m_MainEquipedItemObject.gameObject.SetActive(false);
        if (m_SecondEquipedItemObject != null)
            m_SecondEquipedItemObject.gameObject.SetActive(false);

        if (m_CurrentEquipedItemObject)
            m_Player.SetWeapon(m_CurrentEquipedItemObject.gameObject);

        (UIManager.Instance.m_CurrentUI as BattleUI).UpdateWeapnStatUI(m_CurrentEquipedItemObject);
    }


    public MovingObject CreatePlayer(Vector3 _pos, Quaternion _quat)
    {
        if (m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        // 설정 //
        m_Player = m_PlayerFactory.PopObject(_pos, _quat, (int)OBJECT_TYPE.PLAYER);

        PlayerWeaponInitialize();

        return m_Player;
    }

    public void ChangeWeapon()
    {
        if (m_Player == null) return;
        if (m_CurrentEquipedItemObject == null)
        {

            m_CurrentEquipedItemObject = m_CurrentEquipedItemObject == m_MainEquipedItemObject ? m_SecondEquipedItemObject : m_MainEquipedItemObject;
            m_Player.SetWeapon(m_CurrentEquipedItemObject.gameObject);

            BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).Init(m_Player, m_CurrentEquipedItemObject.m_Item);

            return;
        }

        m_CurrentEquipedItemObject.gameObject.SetActive(false);

        m_CurrentEquipedItemObject = m_CurrentEquipedItemObject == m_MainEquipedItemObject ? m_SecondEquipedItemObject : m_MainEquipedItemObject;

        m_Player.SetWeapon(m_CurrentEquipedItemObject.gameObject);

        SoundManager.Instance.OneShotPlay(UI_SOUND.WEAPON_CHANGE);

        BattleItemSlotButton slot =  BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN);
        
        if(slot != null)
            slot.Init(m_Player, m_CurrentEquipedItemObject.m_Item);

        BattleUI.SetUpgradeItem(m_CurrentEquipedItemObject);
        (UIManager.Instance.m_CurrentUI as BattleUI).UpdateWeapnStatUI(m_CurrentEquipedItemObject);
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
    public bool CheckCanAttack(Vector3 _zombiePos)
    {
        if (m_Player == null) return false;

        Vector3 HitForward = _zombiePos - m_Player.transform.position;
        HitForward = HitForward.normalized;

        if (Vector3.Dot(HitForward, m_Player.transform.forward) > 0.85f)
        {
            return true;
        }
        else return false;
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

    // 인게임상에서 현재 장착하고 있는 아이템을 업그레이드합니다.
    public void CurrentEquipedWeaponUpgrade(UPGRADE_TYPE _upgrade, float _value)
    {
        if (m_CurrentEquipedItemObject == null)
        {
            Debug.LogError("현재 낀 아이템이 없는데?");
            return;
        }

        switch(_upgrade)
        {
            case UPGRADE_TYPE.ATTACK:
                m_CurrentEquipedItemObject.UpgradeAttack(_value);
                break;
            case UPGRADE_TYPE.ATTACK_SPEED:
                m_CurrentEquipedItemObject.UpgradeAttackSpeed(_value);
                break;
            case UPGRADE_TYPE.RANGE:
                m_CurrentEquipedItemObject.UpgradeRange(_value);
                break;
        }

        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).UpdateItemStat(m_CurrentEquipedItemObject.m_CurrentStat);
    }


    public void PlayerAttack()
    {
        if (m_CurrentEquipedItemObject == null) return;

        ItemStat itemStat = m_CurrentEquipedItemObject.m_Item.m_ItemStat;
        MovingObject zombie = EnemyManager.Instance.GetNearestZombie(m_Player.transform.position, itemStat.m_Range);

        if (zombie != null)
        {
            if (CheckCanAttack(zombie.transform.position))
            {
                Vector3 dir = zombie.transform.position - m_Player.transform.position;
                dir.y = 0.0f;
                dir = dir.normalized;

                m_Player.transform.rotation = Quaternion.LookRotation(dir);
            }
        }
        else
        {
            //  m_Player.transform.rotation = Quaternion.LookRotation(BattleUI.m_InputController.m_DragDirectionVector);
        }

        m_CurrentEquipedItemObject.ItemAction();
    }

    public void PlayerUseItem(ITEM_SLOT_SORT _type)
    {
        if (_type == ITEM_SLOT_SORT.MAIN || _type == ITEM_SLOT_SORT.SECOND) return;

        Item item = InvenManager.Instance.GetEquipedItemSlot(_type);

        if (item == null) return;

        switch (item.m_ItemStat.m_Sort)
        {
            case ITEM_SORT.HEALTH_PACK:
                (m_Player as PlayerObject).ReserveBuff(BuffManager.Instance.GetItemBuff(BUFF_TYPE.BLESSING, item.m_ItemStat));
                (m_Player as PlayerObject).ChangeState(E_PLAYABLE_STATE.DRINK);
                break;
            case ITEM_SORT.BUFF:
                (m_Player as PlayerObject).ReserveBuff(BuffManager.Instance.GetItemBuff(BUFF_TYPE.ADRENALINE, item.m_ItemStat));
                (m_Player as PlayerObject).ChangeState(E_PLAYABLE_STATE.DRINK);
                break;
            case ITEM_SORT.GRENADE:
            case ITEM_SORT.FIRE_GRENADE:
                BulletManager.Instance.FireBullet(
                m_Player.transform.position + new Vector3(0,1,0),
                m_Player.transform.forward,
                item.m_ItemStat);
                break;
            case ITEM_SORT.INSTALL_BOMB:
                BulletManager.Instance.FireBullet(
                m_Player.transform.position,
                m_Player.transform.forward,
                item.m_ItemStat);
                //(m_Player as PlayerObject).ChangeState(E_PLAYABLE_STATE.DRINK);
                break;
        }
    }



    public override void DestroyManager()
    {
        foreach (MovingObject obj in m_PlayerFactory.m_ListAllMovingObject)
        {
            obj.pushToMemory((int)obj.m_Type);
        }
    }
}
