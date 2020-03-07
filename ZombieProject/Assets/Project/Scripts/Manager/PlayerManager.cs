using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum UPGRADE_TYPE
{
    NONE,
    ATTACK,
    RANGE,
    ATTACK_SPEED,
    HP,
    AMMO
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

    EffectObject m_CurTargtingEffect;

    public MovingObject m_TargetingZombie { private set; get; }

    StateController m_PlayerStateContoller;

    public int m_MaxClearWave { set; get; }

    Ray m_HitRay = new Ray();
    RaycastHit m_RayCastr = new RaycastHit();

    int m_WallLayerMask;

    GameObject m_FixedFireTransform;

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

    public void ClearWave()
    {
        if (m_MaxClearWave < RespawnManager.Instance.m_CurWave)
        {
            m_MaxClearWave = RespawnManager.Instance.m_CurWave;
#if !UNITY_EDITOR
            DBManager.Instance.UpdateUserClearWaveToFireBase(m_MaxClearWave);
#endif
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
        if (m_CurrentEquipedItemObject != null)
        {
            Destroy(m_CurrentEquipedItemObject);
            m_CurrentEquipedItemObject = null;
        }
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

    public bool GetRangePlayers(Vector3 _pos, float _maxDistance)
    {
        if (m_PlayerFactory == null) return false;
        if (m_Player == null) return false;
        if (m_Player.m_Stat == null) return false;
        if (m_Player.m_Stat.isDead) return false;
        if (!m_Player.gameObject.activeSelf) return false;

        float len = (m_Player.transform.position - _pos).magnitude;

        if (len < _maxDistance)
        {
            return true;
        }

        return false;
    }

    public void SplashAttackToPlayer(Vector3 _pos, float _rangeDistance, float _damage, bool _canKnockBackDamage, int _maxCount = 5)
    {
        if (GetRangePlayers(_pos, _rangeDistance))
        {
            EffectManager.Instance.PlayEffect(PARTICLE_TYPE.HIT_EFFECT, m_Player.transform.position + Vector3.up * 0.75f, Quaternion.identity, Vector3.one * 1.35f, true, 0.5f);

            (UIManager.Instance.m_CurrentUI as BattleUI).OnDamagedEffect();
            m_Player.HitDamage(_damage, _canKnockBackDamage, 1.0f);
        }
    }

    public void AttackToPlayer(float _damage, bool _canKnockBackDamage)
    {
        if (m_Player == null) return;
        if (m_Player.m_Stat == null) return;
        if (m_Player.m_Stat.isDead) return;
        if (!m_Player.gameObject.activeSelf) return;

        EffectManager.Instance.PlayEffect(PARTICLE_TYPE.HIT_EFFECT, m_Player.transform.position + Vector3.up * 0.75f, Quaternion.identity,Vector3.one * 1.35f ,true, 0.5f);

        (UIManager.Instance.m_CurrentUI as BattleUI).OnDamagedEffect();
        m_Player.HitDamage(_damage, _canKnockBackDamage, 1.0f);
    }


    public MovingObject CreatePlayer(Vector3 _pos, Quaternion _quat)
    {
        if (m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        // 설정 //
        m_Player = m_PlayerFactory.PopObject(_pos, _quat, (int)OBJECT_TYPE.PLAYER);

        PlayerWeaponInitialize();
        PushEffectToPool();

        m_PlayerStateContoller = (m_Player as PlayerObject).m_StateController;

        UpdateWeaponRange();

        if (m_FixedFireTransform == null)
        {
            m_FixedFireTransform = new GameObject("FixedFirePos");
        }


        //if(m_CurrentEquipedItemObject != null)
        //    m_Player.DrawCircle(m_CurrentEquipedItemObject.m_CurrentStat.m_Range);

        return m_Player;
    }

    public void HealPlayer(int _heal)
    {
        if (m_Player == null) return;
        if (m_Player.m_Stat == null) return;

        m_Player.m_Stat.CurHP += _heal;
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

        UpdateWeaponRange();

        BattleUI.SetUpgradeItem(m_CurrentEquipedItemObject);
        //(UIManager.Instance.m_CurrentUI as BattleUI).UpdateWeapnStatUI(m_CurrentEquipedItemObject);
    }

    public void UpdateWeaponRange()
    {
        if (m_CurrentEquipedItemObject == null) return;
        if (m_Player == null) return;

        m_Player.DrawCirclurSector(m_CurrentEquipedItemObject.m_CurrentStat.m_Range, m_CurrentEquipedItemObject.m_FireTransform.position, 48.0f);

    }

    public override bool Initialize()
    {
        LoadPlayerInfo();

#if UNITY_EDITOR
        m_MaxClearWave = 25;
#endif

        if (m_PlayerFactory == null)
        {
            m_PlayerFactory = gameObject.AddComponent<ObjectFactory>();
            m_PlayerFactory.Initialize("Prefabs/Players/Player", ("Prefabs/Players/Models/Normal"), (int)OBJECT_TYPE.PLAYER);
            m_PlayerFactory.CreateObjectPool((int)OBJECT_TYPE.PLAYER, 1);
        }

        m_PlayerCreateZone = GameObject.Find("PlayerCreateZone");

        if (m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        m_WallLayerMask = 1 << LayerMask.NameToLayer("Wall");

        return true;
    }

    // 해당 터치 위치가 플레이어가 공격할 수 있는 시야각에 있는지 체크하는 함수입니다.
    public bool CheckCanAttack(Vector3 _zombiePos)
    {
        if (m_Player == null) return false;

        float dot = Mathf.Cos(Mathf.Deg2Rad * (180.0f * 0.5f));

        Vector3 HitForward = _zombiePos - m_Player.transform.position;
        HitForward = HitForward.normalized;

        if (Vector3.Dot(HitForward, m_Player.transform.forward) > dot)
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
            case UPGRADE_TYPE.HP:
                m_Player.m_Stat.MaxHP += _value;
                break;
            case UPGRADE_TYPE.AMMO:
                m_CurrentEquipedItemObject.UpgradeAmmoCount((short)_value);
                break;
        }

        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).UpdateItemStat(m_CurrentEquipedItemObject.m_CurrentStat);
    }


    public void PlayerAttack()
    {
        if (m_CurrentEquipedItemObject == null) return;
        if (m_Player == null) return;
        if (m_Player.m_Stat.isDead) return;

        ItemStat itemStat = m_CurrentEquipedItemObject.m_Item.m_ItemStat;
        MovingObject newTargetZombie = EnemyManager.Instance.GetNearestZombie(m_Player.transform.position, itemStat.m_Range - 0.1f);

        if (newTargetZombie != null )
        {
            if (CheckCanAttack(newTargetZombie.transform.position))
            {
                if (m_TargetingZombie != newTargetZombie || m_TargetingZombie == null)
                {
                    PushEffectToPool();
                    m_CurTargtingEffect = EffectManager.Instance.AttachEffect(PARTICLE_TYPE.ENMETY_FOCUS, newTargetZombie, Vector3.up * 0.2f, Quaternion.Euler(90, 0, 0), Vector3.one);
                }

                m_TargetingZombie = newTargetZombie;

                Vector3 dir = m_TargetingZombie.transform.position - m_Player.transform.position;
                dir.y = 0.0f;
                dir = dir.normalized;

                m_Player.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

                m_CurrentEquipedItemObject.ItemAction(m_TargetingZombie, dir);
            }
            else
            {
                PushEffectToPool();
                m_TargetingZombie = null;

                m_CurrentEquipedItemObject.ItemAction(null, m_CurrentEquipedItemObject.m_FireTransform.forward);
            }
        }
        else
        {
            PushEffectToPool();
            m_TargetingZombie = null;

            m_CurrentEquipedItemObject.ItemAction(null, m_CurrentEquipedItemObject.m_FireTransform.forward);
        }

        
    }

    void PushEffectToPool()
    {
        if (m_CurTargtingEffect != null )
        {
            m_CurTargtingEffect.pushToMemory();
            m_CurTargtingEffect = null;
        }
    }

    public void PlayReservedAction()
    {
        if (m_Player == null) return;
        if ((m_Player as PlayerObject).m_ReservedAction == null) return;

        (m_Player as PlayerObject).m_ReservedAction.Invoke();
        (m_Player as PlayerObject).ReserveAction(null);
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
                (m_Player as PlayerObject).ReserveAction(
                    ()=>
                    {
                        BulletManager.Instance.FireBullet(
                        m_Player.transform.position + new Vector3(0, 1, 0),
                        m_Player.transform.forward,
                        item.m_ItemStat);
                    });
                (m_Player as PlayerObject).ChangeState(E_PLAYABLE_STATE.USE_QUICK);
                break;
            case ITEM_SORT.INSTALL_BOMB:
                (m_Player as PlayerObject).ReserveAction(
                    () =>
                    {
                        BulletManager.Instance.FireBullet(
                        m_Player.transform.position,
                        m_Player.transform.forward,
                        item.m_ItemStat);
                    });
                (m_Player as PlayerObject).ChangeState(E_PLAYABLE_STATE.PICK_UP);
                break;
        }
    }

    public E_PLAYABLE_STATE GetPlayerState()
    {
        if (m_Player == null) return E_PLAYABLE_STATE.NONE;
        if (m_PlayerStateContoller == null) return E_PLAYABLE_STATE.NONE;

        return m_PlayerStateContoller.m_eCurrentState;
    }

    public void FullChargeAllWeaponStackCount()
    {
        if (m_MainEquipedItemObject != null)
        {
            if (m_MainEquipedItemObject.m_Item != null)
                m_MainEquipedItemObject.m_Item.FullChargeItemCount(m_MainEquipedItemObject.m_CurrentStat.m_Count);
        }

        if (m_SecondEquipedItemObject != null)
        {
            if(m_SecondEquipedItemObject.m_Item != null)
            m_SecondEquipedItemObject.m_Item.FullChargeItemCount(m_SecondEquipedItemObject.m_CurrentStat.m_Count);
        }

        (UIManager.Instance.m_CurrentUI as BattleUI).UpdateCurrentWeaponCountText();
    }


    public override void DestroyManager()
    {
        foreach (MovingObject obj in m_PlayerFactory.m_ListAllMovingObject)
        {
            obj.pushToMemory();
        }

        PushEffectToPool();
        m_Player = null;

        if (m_CurrentEquipedItemObject != null)
        {
            Destroy(m_CurrentEquipedItemObject);
            m_CurrentEquipedItemObject = null;
        }
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


        CurrentMoney = 0;
    }
}
