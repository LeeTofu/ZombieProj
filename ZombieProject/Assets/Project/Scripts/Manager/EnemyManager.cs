using System.Collections;
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
        m_ZombieCreateZone.GetComponent<MeshRenderer>().enabled = false;

        Invoke("CreateZombie" ,1.0f);
        Invoke("CreateZombie", 1.5f);
        Invoke("CreateZombie", 3.0f);
        Invoke("CreateZombie", 2.50f);
        Invoke("CreateZombie", 3.75f);
    }

    public void CreateZombie()
    {
        MovingObject newZombie = m_ZombieFactory.CreateObject(m_ZombieCreateZone.transform.position, m_ZombieCreateZone.transform.rotation);
        m_ListZombies.Add(newZombie);
    }

    public override bool Initialize()
    {
        m_ZombieFactory = gameObject.AddComponent<ZombieFactory>();
        m_ZombieFactory.Initialize();

        return true;
    }

    
}
