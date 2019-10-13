using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private ZombieFactory m_ZombieFactory;

    // Start is called before the first frame update
    void Start()
    {
        if(GameMaster.Instance.m_Test)
        {
            Debug.Log("Test");
        }

        m_ZombieFactory = gameObject.AddComponent<ZombieFactory>();
        m_ZombieFactory.Initialize();

        Initialize();
    }

    protected override void Initialize()
    {
        
        return;
    }
}
