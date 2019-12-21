using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ATTACK_TYPE
{
    BULLET,
    BOUNCING_BULLET,
    ARC_BULLET,
    RAY,
    MELEE
};


public class ActionTypeManager : Singleton<ActionTypeManager>
{
    Dictionary<ATTACK_TYPE, System.Action<Vector3, Vector3, MovingObject>> m_ActionTypeTable;
    public AudioClip[] m_GunSound;
    BulletFactory m_BulletFactory;
    AudioSource m_Audio;

    public override bool Initialize()
    {
        m_BulletFactory = gameObject.AddComponent<BulletFactory>();
        m_BulletFactory.Initialize(30);
        m_Audio = gameObject.AddComponent<AudioSource>();
        m_GunSound = Resources.LoadAll<AudioClip>("Sound/WeaponSound");

        return true;
    }

    public void SetItemActionType(Item _item)
    {
       switch(_item.m_ItemStat.m_Sort)
       {
            case ITEM_SORT.RIFLE:
            case ITEM_SORT.LAUNCHER:
                _item.SetAttackAction((pos, dir, character) =>
                {
                    character.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

                    MovingObject newBulletObj = m_BulletFactory.CreateObject(pos, Quaternion.identity);
                    Bullet newBullet = newBulletObj as Bullet;

                    if (newBullet)
                    {
                       newBullet.FireBullet(pos, dir, _item.m_ItemStat.m_BulletSpeed);
                    }
                });
                break;
            case ITEM_SORT.SNIPER:
                _item.SetAttackAction((pos, dir, character) =>
                {
                    character.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

                    GameObject newBulletObj = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/Bullet/" + _item.m_ItemStat.m_BulletString));
                    Bullet newBullet = newBulletObj.GetComponent<Bullet>();

                    if (newBullet)
                    {
                         newBullet.Initialize(null, null);
                         newBullet.FireBullet(pos, dir, _item.m_ItemStat.m_BulletSpeed);
                    }
                });
                break;
       }




    }








}
