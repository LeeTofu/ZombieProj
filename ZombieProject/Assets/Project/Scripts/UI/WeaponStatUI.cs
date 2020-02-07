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

    private void OnDisable()
    {
        m_Attack.text = " ";
        m_AttackLv.text = " ";

        m_Range.text = " ";
        m_RangeLv.text = " ";

        m_AttackSpeed.text = " ";
        m_AttackSpeedLv.text = " ";
    }


    public void SetWeaponStat(ItemObject _object )
    {
        float attack = Mathf.Round(_object.m_CurrentStat.m_AttackPoint * 100) * 0.01f;
        float range = Mathf.Round(_object.m_CurrentStat.m_Range * 100) * 0.01f;
        float speed = Mathf.Round(_object.m_CurrentStat.m_AttackSpeed * 100) * 0.01f;

        m_Attack.text = attack.ToString();
        m_AttackLv.text = "Lv" + _object.GetUpgradeCount(UPGRADE_TYPE.ATTACK).ToString();

        m_Range.text = range.ToString();
        m_RangeLv.text = "Lv" + _object.GetUpgradeCount(UPGRADE_TYPE.RANGE).ToString();

        m_AttackSpeed.text = speed.ToString() + "s";
        m_AttackSpeedLv.text = "Lv" + _object.GetUpgradeCount(UPGRADE_TYPE.ATTACK_SPEED).ToString();

    }
}
