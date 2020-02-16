using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SHOP_SORT
{
    NONE = 0,
    AMMO,
    QUICK,
    RANGEUP,
    ATTACKUP,
    ATTACKSPEEDUP,
    HPUP,
    END
}
public class NpcShopButton : UIPressSubject
{
    [SerializeField]
    int m_ButtonMaxLevel;

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

    GameObject m_NoMoneyImage;

    private void Awake()
    {
        m_NoMoneyImage = transform.Find("NoMoney").gameObject;
        m_NoMoneyImage.SetActive(false);
    }


    private void Start()
    {
        m_MoneyText.text = "$" + (m_StartCost + m_PlusCost * m_UpgLevel).ToString();
        m_MoneyText.color = Color.green;

        if(m_slotType != SHOP_SORT.AMMO)
            m_UpgLevelText.text = "Lv." + m_UpgLevel.ToString();
        else m_UpgLevelText.text = " ";
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
        }

        if(Type != UPGRADE_TYPE.NONE)
        {
            m_UpgLevel = m_CurrentUpgradedItem.GetUpgradeCount(Type);
            (UIManager.Instance.m_CurrentUI as BattleUI).UpdateWeapnStatUI(m_CurrentUpgradedItem);
        }

        UpdateUpgradeUI();
        UpdateMoneyText();

    }

    public override void OnPointerDown(PointerEventData eventData)
    {

    }

    public void UpdateMoneyText()
    {
        m_MoneyText.text = "$" + (m_StartCost + m_PlusCost * m_UpgLevel).ToString();

        if(PlayerManager.Instance.CurrentMoney < (m_StartCost + m_PlusCost * m_UpgLevel))
        {
            m_MoneyText.color = Color.red;
            m_NoMoneyImage.SetActive(true);
        }
        else
        {
            m_MoneyText.color = Color.green;
            m_NoMoneyImage.SetActive(false);
        }

    }

    void UpdateUpgradeUI()
    {
        if (m_slotType == SHOP_SORT.AMMO) return;

        if(m_UpgLevel == -1 ) return;

        if (m_slotType != SHOP_SORT.QUICK)
        {
            if (m_UpgLevel >= m_ButtonMaxLevel)
            {
                m_MoneyText.text = " ";
                m_UpgLevelText.text = "Lv.MAX";
            }
            else
            {
                m_UpgLevelText.text = "Lv." + m_UpgLevel.ToString();
            }
        }
        else if (m_slotType == SHOP_SORT.QUICK)
        {
            m_UpgLevelText.text = " ";
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (PlayerManager.Instance.CurrentMoney < m_StartCost + m_PlusCost * m_UpgLevel) return;
        if (m_UpgLevel >= m_ButtonMaxLevel && m_slotType != SHOP_SORT.QUICK) return;
        if (PlayerManager.Instance.m_CurrentEquipedItemObject == null) return;
        if (m_CurrentUpgradedItem == null) return;

        PlayerManager.Instance.CurrentMoney -= m_StartCost + m_PlusCost * m_UpgLevel;
        SoundManager.Instance.OneShotPlay(UI_SOUND.CASH_REGISTER);

        switch (m_slotType)
        {
            case SHOP_SORT.AMMO:
                (UIManager.Instance.m_CurrentUI as BattleUI).UpdateFullMaxCount(ITEM_SLOT_SORT.MAIN);
                break;
            case SHOP_SORT.QUICK:
                (UIManager.Instance.m_CurrentUI as BattleUI).UpdateFullMaxCount(ITEM_SLOT_SORT.THIRD);
                (UIManager.Instance.m_CurrentUI as BattleUI).UpdateFullMaxCount(ITEM_SLOT_SORT.FOURTH);
                m_UpgLevel++;
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
            case SHOP_SORT.HPUP:
                PlayerManager.Instance.CurrentEquipedWeaponUpgrade(UPGRADE_TYPE.HP, 5);
                m_UpgLevel++;
                break;
            case SHOP_SORT.ATTACKSPEEDUP:
                float currentattackspeed = m_CurrentUpgradedItem.m_CurrentStat.m_AttackSpeed;
                PlayerManager.Instance.CurrentEquipedWeaponUpgrade(UPGRADE_TYPE.ATTACK_SPEED, -currentattackspeed * 0.05f);
                m_UpgLevel = m_CurrentUpgradedItem.GetUpgradeCount(UPGRADE_TYPE.ATTACK_SPEED);

                break;
        }

        BattleUI.SetUpgradeItem(m_CurrentUpgradedItem);
    }
    public override void OnPressed()
    {
    }
}
