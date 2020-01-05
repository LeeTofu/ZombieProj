using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BULLET_TYPE
{
    NONE,
    BAZUKA,
    NORMAL_BULLET,

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
        m_BulletFactory = gameObject.AddComponent<ObjectFactory>();

        m_BulletFactory.Initialize("Prefabs/Bullet/BulletPrefab", Resources.LoadAll<GameObject>("Prefabs/Bullet/Models"));

        for (BULLET_TYPE i = BULLET_TYPE.BAZUKA; i != BULLET_TYPE.END; i++)
        {
            m_BulletFactory.CreateObjectPool((int)i, 10);
        }

        return true;
    }

    public BULLET_TYPE GetBulletTypeFromItemStat(ItemStat _stat)
    {
        switch(_stat.m_Sort)
        {
            case ITEM_SORT.LAUNCHER:
                return BULLET_TYPE.BAZUKA;
            case ITEM_SORT.MACHINE_GUN:
            case ITEM_SORT.RIFLE:
            case ITEM_SORT.SNIPER:
                return BULLET_TYPE.NORMAL_BULLET;
            default:
                return BULLET_TYPE.ZOMBIE_RANGE_ATTACK_BULLET1;
        }
    }

    public void FireBullet(Vector3 _pos, Vector3 _dir, ItemStat _itemStat)
    {
        var bulletType = GetBulletTypeFromItemStat(_itemStat);

        Bullet bulletObject = m_BulletFactory.GetObjectFromFactory(_pos, Quaternion.identity, (int)bulletType) as Bullet;

        if(bulletObject == null)
        {
            Debug.LogError("Bullet 없슈");
            return;
        }

        bulletObject.FireBullet(_pos, _dir, _itemStat);
    }


}
