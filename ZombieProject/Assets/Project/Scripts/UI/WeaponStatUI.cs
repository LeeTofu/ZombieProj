using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WeaponStatUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI m_AttackLv;

    [SerializeField]
    TextMeshProUGUI m_Attack;


    [SerializeField]
    TextMeshProUGUI m_RangeLv;


    [SerializeField]
    TextMeshProUGUI m_Range;


    [SerializeField]
    TextMeshProUGUI m_AttackSpeedLv;


    [SerializeField]
    TextMeshProUGUI m_AttackSpeed;

    [SerializeField]
    TextMeshProUGUI m_AmmoCountLv;


    [SerializeField]
    TextMeshProUGUI m_AmmoCount;

    [SerializeField]
    GameObject m_RollObject;

    ItemObject m_ItemObject;


    private void OnDisable()
    {
        if (m_ItemObject == null) return;


        m_Attack.text = " ";
        m_AttackLv.text = " ";

        m_Range.text = " ";
        m_RangeLv.text = " ";

        m_AttackSpeed.text = " ";
        m_AttackSpeedLv.text = " ";

        m_AmmoCountLv.text = " ";
        m_AmmoCount.text = " ";

    }

    public void RollUI()
    {
        m_RollObject.gameObject.SetActive(true);
        gameObject.SetActive(false);
    
        if(m_ItemObject != null)
        {
            SetWeaponStat(m_ItemObject);
        }
    
    }


    public void SetWeaponStat(ItemObject _object )
    {
        m_ItemObject = _object;

        float attack = Mathf.Round(_object.m_CurrentStat.m_AttackPoint * 100) * 0.01f;
        float range = Mathf.Round(_object.m_CurrentStat.m_Range * 100) * 0.01f;
        float speed = Mathf.Round(_object.m_CurrentStat.m_AttackSpeed * 100) * 0.01f;
        float ammo = Mathf.Round(_object.m_CurrentStat.m_Count);

        m_Attack.text = attack.ToString();
        m_AttackLv.text = "Lv" + _object.GetUpgradeCount(UPGRADE_TYPE.ATTACK).ToString();

        m_Range.text = range.ToString();
        m_RangeLv.text = "Lv" + _object.GetUpgradeCount(UPGRADE_TYPE.RANGE).ToString();

        m_AttackSpeed.text = speed.ToString() + "s";
        m_AttackSpeedLv.text = "Lv" + _object.GetUpgradeCount(UPGRADE_TYPE.ATTACK_SPEED).ToString();

        m_AmmoCount.text = ammo.ToString();
        m_AmmoCountLv.text = "Lv" + _object.GetUpgradeCount(UPGRADE_TYPE.AMMO).ToString();

    }
}
