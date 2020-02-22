using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BULLET_TYPE
{
    NONE,
    BAZUKA,
    NORMAL_BULLET,

    SHOT_GUN_BULLET,
    SNIPER_BULLET,
    THROW_BULLET,
    FIRE_THROW_BULLET,
    INSTALL_BOMB,

    ZOMBIE_RANGE_ATTACK_BULLET1,
    ZOMBIE_RANGE_ATTACK_BULLET2,
    ZOMBIE_RANGE_ATTACK_BULLET3,
    END
}

public class BulletManager : Singleton<BulletManager>
{
    ObjectFactory m_BulletFactory;

    public override bool Initialize()
    {
        if (m_BulletFactory == null)
        {
            m_BulletFactory = gameObject.AddComponent<ObjectFactory>();

            m_BulletFactory.Initialize("Prefabs/Bullet/BulletPrefab", "Prefabs/Bullet/Models/NormalBullet", (int)BULLET_TYPE.NORMAL_BULLET);
            m_BulletFactory.CreateObjectPool((int)BULLET_TYPE.NORMAL_BULLET, 5);

            m_BulletFactory.Initialize("Prefabs/Bullet/ShotGunBulletPrefab", "Prefabs/Bullet/Models/NormalBullet", (int)BULLET_TYPE.SHOT_GUN_BULLET);
            m_BulletFactory.CreateObjectPool((int)BULLET_TYPE.SHOT_GUN_BULLET, 25);

            m_BulletFactory.Initialize("Prefabs/Bullet/BazukaBulletPrefab", "Prefabs/Bullet/Models/Bazuka", (int)BULLET_TYPE.BAZUKA);
            m_BulletFactory.CreateObjectPool((int)BULLET_TYPE.BAZUKA, 5);

            m_BulletFactory.Initialize("Prefabs/Bullet/PierceBulletPrefab", ("Prefabs/Bullet/Models/PierceBullet"), (int)BULLET_TYPE.SNIPER_BULLET);
            m_BulletFactory.CreateObjectPool((int)BULLET_TYPE.SNIPER_BULLET, 5);

            m_BulletFactory.Initialize("Prefabs/Bullet/ThrowBulletPrefab", ("Prefabs/Bullet/Models/Grenade") , (int)BULLET_TYPE.THROW_BULLET);
            m_BulletFactory.CreateObjectPool((int)BULLET_TYPE.THROW_BULLET, 5);

            m_BulletFactory.Initialize("Prefabs/Bullet/FireThrowBulletPrefab",("Prefabs/Bullet/Models/FireGrenade"), (int)BULLET_TYPE.FIRE_THROW_BULLET);
            m_BulletFactory.CreateObjectPool((int)BULLET_TYPE.FIRE_THROW_BULLET, 5);

            m_BulletFactory.Initialize("Prefabs/Bullet/InstallBombPrefab",("Prefabs/Bullet/Models/InstallBomb"), (int)BULLET_TYPE.INSTALL_BOMB);
            m_BulletFactory.CreateObjectPool((int)BULLET_TYPE.INSTALL_BOMB, 5);

        }

        return true;
    }

    public override void DestroyManager()
    {
    }

    public BULLET_TYPE GetBulletTypeFromItemStat(ItemStat _stat)
    {
        switch(_stat.m_Sort)
        {
            case ITEM_SORT.LAUNCHER:
                return BULLET_TYPE.BAZUKA;
            case ITEM_SORT.MACHINE_GUN:
            case ITEM_SORT.RIFLE:
                return BULLET_TYPE.NORMAL_BULLET;
            case ITEM_SORT.SHOT_GUN:
                return BULLET_TYPE.SHOT_GUN_BULLET;
            case ITEM_SORT.SNIPER:
                return BULLET_TYPE.SNIPER_BULLET;
            case ITEM_SORT.GRENADE:
                return BULLET_TYPE.THROW_BULLET;
            case ITEM_SORT.FIRE_GRENADE:
                return BULLET_TYPE.FIRE_THROW_BULLET;
            case ITEM_SORT.INSTALL_BOMB:
                return BULLET_TYPE.INSTALL_BOMB;
            default:
                return BULLET_TYPE.ZOMBIE_RANGE_ATTACK_BULLET1;
        }
    }

    public void FireBullet(Vector3 _pos, Vector3 _dir, ItemStat _itemStat)
    {
        var bulletType = GetBulletTypeFromItemStat(_itemStat);

        Bullet bulletObject = m_BulletFactory.PopObject(_pos, Quaternion.identity, (int)bulletType) as Bullet;

        if(bulletObject == null)
        {
            Debug.LogError("Bullet 없슈");
            return;
        }

        bulletObject.FireBullet(_pos, _dir, _itemStat);
    }

    public void FireBullet(Vector3 _pos, Vector3 _dir, BULLET_TYPE _bulletType, STAT _stat)
    {
        Bullet bulletObject = m_BulletFactory.PopObject(_pos, Quaternion.identity, (int)_bulletType) as Bullet;

        if (bulletObject == null)
        {
            Debug.LogError("Bullet 없슈");
            return;
        }

        bulletObject.FireBullet(_pos, _dir, _stat);
    }

    public void FireBullet(Vector3 _pos, Vector3 _dir, BULLET_TYPE _bulleyType)
    {
        //좀비용 FireBullet, 아직 미구현
    }
}
