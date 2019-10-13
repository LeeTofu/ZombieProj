using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private ZombieFactory m_ZombieFactory;

    // Start is called before the first frame update
    void Start()
    {
        m_ZombieFactory = gameObject.AddComponent<ZombieFactory>();
        m_ZombieFactory.Initialize();

        Invoke("CreateZombie" ,1.0f);
    }

    public void CreateZombie()
    {
        m_ZombieFactory.CreateZombie();
    }

    public override bool Initialize()
    {
        
        return true;
    }

    
}
