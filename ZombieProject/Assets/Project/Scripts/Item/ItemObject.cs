using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// InGame에 생성될 아이템 오브젝트임... 그냥 발사 위치나 이런거 알때 씀.
public class ItemObject : MonoBehaviour
{
    Dictionary<UPGRADE_TYPE, int> m_DicUpdateCount = new Dictionary<UPGRADE_TYPE, int>();

    // 이 Stat에 따라 아이템의 공격력, 공격스피드 결정.
    public ItemStat m_CurrentStat;

    const string m_FirePosString = "FirePos";
    public Transform m_FireTransform { get; private set; }
    AudioSource m_audio;
    AudioClip[] m_auidoClip;

    public short m_currentBulletCount;

    public Item m_Item { private set; get; }

    private void Awake()
    {
        var trArr = transform.GetComponentsInChildren<Transform>();
        foreach (Transform tr in trArr)
        {
            if (tr.name == m_FirePosString)
            {
                m_FireTransform = tr;
                return;
            }
        }

        m_audio = GetComponent<AudioSource>();

        if (m_audio == null)
            m_audio = gameObject.AddComponent<AudioSource>();

        m_audio.volume = 1.0f;

    }

    public void Init(Item _item)
    {
        m_auidoClip = Resources.LoadAll<AudioClip>("Sound/WeaponSound/" + _item.m_ItemStat.m_Sort.ToString());

        m_Item = _item;
        m_CurrentStat = m_Item.m_ItemStat;
        m_currentBulletCount = _item.m_Count;
        m_audio = GetComponent<AudioSource>();

        if (m_audio == null)
            m_audio = gameObject.AddComponent<AudioSource>();

        m_DicUpdateCount.Clear();
        m_DicUpdateCount.Add(UPGRADE_TYPE.ATTACK, 0);
        m_DicUpdateCount.Add(UPGRADE_TYPE.ATTACK_SPEED, 0);
        m_DicUpdateCount.Add(UPGRADE_TYPE.RANGE, 0);
        m_DicUpdateCount.Add(UPGRADE_TYPE.AMMO, 0);
    }


    public void PlaySound()
    {
        if (m_audio)
            m_audio.PlayOneShot(m_auidoClip[Random.Range(0, m_auidoClip.Length)]);

        else Debug.LogError("무기에 오디오 없습니다.");
    }

    public void UpgradeRange(float _rangePlus)
    {
        int count = 0;
        if(m_DicUpdateCount.TryGetValue(UPGRADE_TYPE.RANGE, out count))
        {
            count++;
            m_DicUpdateCount[UPGRADE_TYPE.RANGE] = count;
        }
        m_CurrentStat.m_Range += _rangePlus;
    }

    public void UpgradeAttack(float _attackPlus)
    {
        int count = 0;
        if (m_DicUpdateCount.TryGetValue(UPGRADE_TYPE.ATTACK, out count))
        {
            count++;
            m_DicUpdateCount[UPGRADE_TYPE.ATTACK] = count;
        }
        else
        {
            Debug.Log("그어ㅗㅄ");
        }

        m_CurrentStat.m_AttackPoint += _attackPlus;
    }

    public void UpgradeAttackSpeed(float _attackSpeed)
    {
        int count = 0;
        if (m_DicUpdateCount.TryGetValue(UPGRADE_TYPE.ATTACK_SPEED, out count))
        {
            count++;
            m_DicUpdateCount[UPGRADE_TYPE.ATTACK_SPEED] = count;
        }
        m_CurrentStat.m_AttackSpeed += _attackSpeed;
    }

    public void UpgradeAmmoCount(short _count)
    {
        int count = 0;
        if (m_DicUpdateCount.TryGetValue(UPGRADE_TYPE.AMMO, out count))
        {
            count++;
            m_DicUpdateCount[UPGRADE_TYPE.AMMO] = count;
        }

        if (_count == 0) _count = 1;

        m_CurrentStat.m_Count += _count;
    }

    public int GetUpgradeCount(UPGRADE_TYPE _type)
    {
        int count;
        if(m_DicUpdateCount.TryGetValue(_type, out count))
        {
            return count;
        }
        return -1;
    }

    public void ItemAction(MovingObject _object, Vector3 _dir)
    {
        if (_object == null)
        {
            ItemAction(m_FireTransform.transform.forward);
        }
        else
        {
            ItemAction(_dir);
        }
    }


    private void ItemAction(Vector3 _dir)
    {
        PlaySound();

        switch (ItemManager.Instance.GetItemActionType(m_Item))
        {
            case ITEM_EVENT_TYPE.FIRE_BULLET:
            case ITEM_EVENT_TYPE.PIERCE:
            case ITEM_EVENT_TYPE.THROW_ARK:
                BulletManager.Instance.FireBullet(
                 m_FireTransform.position,
                new Vector3(_dir.x, 0 , _dir.z),
                m_CurrentStat);
                break;
            case ITEM_EVENT_TYPE.SHOT_GUN:

                for(int i = 0; i < 5; i++)
                {
                    Vector3 forward = new Vector3(_dir.x , 0, _dir.z);
                    Vector3 dir = Quaternion.Euler(0, -30.0f + i * 15.0f, 0) * forward;

                    BulletManager.Instance.FireBullet(
                    m_FireTransform.position,
                    dir,
                    m_CurrentStat);
                }
                break;
            case ITEM_EVENT_TYPE.MELEE:

                break;
        }
    }

}
