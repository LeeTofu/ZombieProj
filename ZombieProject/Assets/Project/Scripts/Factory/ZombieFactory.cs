using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie 생성하는 객체

public class ZombieFactory : ObjectFactory
{

    public override void Initialize(int _MaxCount)
    {
        m_MaxCount = _MaxCount;
        m_MovingObejctPrefabs = Resources.Load<GameObject>("Prefabs/Zombies/Zombie");
        m_ModelPrefabs = Resources.LoadAll<GameObject>("Prefabs/Zombies/Models/Normal");

        if (m_MovingObejctPrefabs)
        {
            Debug.Log("m_MovingObejctPrefabs Load Success");
        }

        if (m_ModelPrefabs.Length != 0)
        {
            Debug.Log("m_ZombieModelPrefabs Load Success");
        }

        CreateObjectPool();
    }
}
