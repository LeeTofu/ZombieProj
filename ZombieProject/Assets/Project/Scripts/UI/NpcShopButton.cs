using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SHOP_SORT
{
    NONE = 0,
    AMMO,
    RANGEUP,
    ATTACKUP,
    ATTACKSPEEDUP,
    END
}
public class NpcShopButton : UIPressSubject
{
    static readonly ushort MAX_LEVEL = 20;

    int m_UpgLevel = 0;

    [SerializeField]
    ushort m_StartCost;

    [SerializeField]
    ushort m_PlusCost;

    public SHOP_SORT m_slotType;

    [SerializeField]
    TMPro.TextMeshProUGUI m_StackCountText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_MoneyText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_UpgLevelText;

    static ItemObject m_CurrentUpgradedItem;

    private void Awake()
    {

    }


    private void Start()
    {
        m_MoneyText.text = "$" + (m_StartCost + m_PlusCost * m_UpgLevel).ToString();
        m_MoneyText.color = Color.green;

        if(m_slotType != SHOP_SORT.AMMO)
            m_UpgLevelText.text = "Lv." + m_UpgLevel.ToString();
        else m_UpgLevelText.text = " ";

     //   (UIManager.Instance.m_CurrentUI as BattleUI).InsertNPCUpgradeButton(this);

    }

      public void SetUpgradeWeapon(ItemObject _item)
    {
        if (_item == null) return;

        m_CurrentUpgradedItem = _item;

        UPGRADE_TYPE Type = UPGRADE_TYPE.NONE;

        switch(m_slotType)
        {
            case SHOP_SORT.ATTACKUP:
                {
                    Type = UPGRADE_TYPE.ATTACK;
                }
                break;
            case SHOP_SORT.ATTACKSPEEDUP:
                {
                    Type = UPGRADE_TYPE.ATTACK_SPEED;
                }
                break;
            case SHOP_SORT.RANGEUP:
                {
                    Type = UPGRADE_TYPE.RANGE;
                }
                break;
            case SHOP_SORT.AMMO:
                {
                    return;
                }
                break;
        }

        

        if(Type == UPGRADE_TYPE.NONE)
        {
            Debug.LogError("Error.. 업그레이드 되는 아이템이 없다.");
            return;
        }

    
        m_UpgLevel = m_CurrentUpgradedItem.GetUpgradeCount(Type);
        UpdateUpgradeUI();

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (PlayerManager.Instance.CurrentMoney < m_StartCost + m_PlusCost * m_UpgLevel)
            return;
        if (m_UpgLevel >= MAX_LEVEL)
            return;
        if (PlayerManager.Instance.m_CurrentEquipedItemObject == null)
            return;
        if (m_CurrentUpgradedItem == null)
            return;

        PlayerManager.Instance.CurrentMoney -= m_StartCost + m_PlusCost * m_UpgLevel;
        SoundManager.Instance.OneShotPlay(UI_SOUND.CASH_REGISTER);

        switch (m_slotType)
        {
            case SHOP_SORT.AMMO:
                short current = m_CurrentUpgradedItem.m_Item.m_Count;
                short max = m_CurrentUpgradedItem.m_CurrentStat.m_Count;
                (UIManager.Instance.m_CurrentUI as BattleUI).UpdateCount(ITEM_SLOT_SORT.MAIN, (short)(max - current));
                break;
            case SHOP_SORT.RANGEUP:
                float currentrange = m_CurrentUpgradedItem.m_CurrentStat.m_Range;
                PlayerManager.Instance.CurrentEquipedWeaponUpgrade(UPGRADE_TYPE.RANGE, currentrange * 0.05f);
                m_UpgLevel = m_CurrentUpgradedItem.GetUpgradeCount(UPGRADE_TYPE.RANGE);
                
                break;
            case SHOP_SORT.ATTACKUP:
                float currentattack = m_CurrentUpgradedItem.m_CurrentStat.m_AttackPoint;
                PlayerManager.Instance.CurrentEquipedWeaponUpgrade(UPGRADE_TYPE.ATTACK, currentattack * 0.05f);
                m_UpgLevel = m_CurrentUpgradedItem.GetUpgradeCount(UPGRADE_TYPE.ATTACK);
                
                break;
            case SHOP_SORT.ATTACKSPEEDUP:
                float currentattackspeed = m_CurrentUpgradedItem.m_CurrentStat.m_AttackSpeed;
                PlayerManager.Instance.CurrentEquipedWeaponUpgrade(UPGRADE_TYPE.ATTACK_SPEED, -currentattackspeed * 0.05f);
                m_UpgLevel = m_CurrentUpgradedItem.GetUpgradeCount(UPGRADE_TYPE.ATTACK_SPEED);
                
                break;
        }
        
        UpdateUpgradeUI();

    }

    void UpdateUpgradeUI()
    {
        if (m_slotType == SHOP_SORT.AMMO) return;
        if(m_UpgLevel == -1 ) return;

        if (m_UpgLevel >= MAX_LEVEL)
        {
            m_MoneyText.text = "X";
            m_MoneyText.color = Color.red;
            m_UpgLevelText.text = "Lv.MAX";
        }
        else
        {
            m_MoneyText.text = "$" + (m_StartCost + m_PlusCost * m_UpgLevel).ToString();
            m_MoneyText.color = Color.green;
            m_UpgLevelText.text = "Lv." + m_UpgLevel.ToString();
        }

        (UIManager.Instance.m_CurrentUI as BattleUI).UpdateWeapnStatUI(m_CurrentUpgradedItem);
    }



    public override void OnPointerUp(PointerEventData eventData)
    {
    }
    public override void OnPressed()
    {
    }
}
