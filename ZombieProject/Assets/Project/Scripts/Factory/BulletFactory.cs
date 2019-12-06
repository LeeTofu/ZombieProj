using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie 생성하는 객체

public class BulletFactory : ObjectFactory
{

    public override void Initialize(int _MaxCount)
    {
        m_MaxCount = _MaxCount;
        m_MovingObejctPrefabs = Resources.Load<GameObject>("Prefabs/Weapon/Bullet/TestBullet");
        m_ModelPrefabs = Resources.LoadAll<GameObject>("Prefabs/Weapon/Bullet/Models");

        if (!m_MovingObejctPrefabs)
        {
            Debug.LogError("m_MovingObejctPrefabs Load Success");
        }

        if (m_ModelPrefabs.Length == 0)
        {
            Debug.LogError("m_ZombieModelPrefabs Load Success");
        }

        CreateObjectPool();
    }
}
