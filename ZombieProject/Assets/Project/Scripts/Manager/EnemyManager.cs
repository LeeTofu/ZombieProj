﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private List<MovingObject> m_ListZombies = new List<MovingObject>();
    private List<MovingObject> m_ListEliteZombies = new List<MovingObject>();
    private List<MovingObject> m_ListBossZombies = new List<MovingObject>();

    private ObjectFactory m_ZombieFactory;

    [HideInInspector]
    public GameObject m_ZombieCreateZone;

    // Start is called before the first frame update
    void Start()
    {
        m_ZombieCreateZone = GameObject.Find("ZombieCreateZone");

        if(m_ZombieCreateZone != null)
            m_ZombieCreateZone.GetComponent<MeshRenderer>().enabled = false;
    }

    public void CreateZombie(Vector3 _pos, Quaternion _quat)
    {
        if(m_ZombieFactory )
        {
            MovingObject newZombie = m_ZombieFactory.CreateObject(_pos, _quat);
            m_ListZombies.Add(newZombie);
        }
    }

    public override bool Initialize()
    {
        m_ZombieFactory = gameObject.AddComponent<ZombieFactory>();
        m_ZombieFactory.Initialize();

        return true;
    }

    
}
