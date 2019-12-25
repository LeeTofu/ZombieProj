using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


public class EnemyManager : Singleton<EnemyManager>
{
    private List<MovingObject> m_ListZombies = new List<MovingObject>();
    private List<MovingObject> m_ListEliteZombies = new List<MovingObject>();
    private List<MovingObject> m_ListBossZombies = new List<MovingObject>();

    private Dictionary<int, STAT> m_NormalZombieStatTable = new Dictionary<int, STAT>();
    private Dictionary<int, STAT> m_NamedZombieStatTable = new Dictionary<int, STAT>();
    private Dictionary<int, STAT> m_BossZombieStatTable = new Dictionary<int, STAT>();

    private ObjectFactory m_ZombieFactory;

    [HideInInspector]
    public GameObject m_ZombieCreateZone;

    public MovingObject GetZombie()
    {
        return m_ListZombies[0];
    }

    // _pos 에서 가장 가가운 좀비 찾아낸다.
    public MovingObject GetNearestZombie(Vector3 _pos, float _maxDistance)
    {
        float maxLen = 1000000.0f;
        MovingObject target = null;

        foreach(MovingObject zombie in m_ListZombies)
        {
            float sqrlen = (zombie.transform.position - _pos).sqrMagnitude;
            float len = (zombie.transform.position - _pos).magnitude;

            if (len > _maxDistance) continue;

            if (len < maxLen)
            {
                target = zombie;
                maxLen = len;
            }
        }

        return target;
    }


    public void CreateZombie(Vector3 _pos, Quaternion _quat)
    {
        if(m_ZombieFactory )
        {
            MovingObject newZombie = m_ZombieFactory.CreateObject(_pos, _quat);
            m_ListZombies.Add(newZombie);
        }
    }

    public void CreateZombie()
    {
        if (m_ZombieFactory)
        {
            MovingObject newZombie = m_ZombieFactory.CreateObject(m_ZombieCreateZone.transform.position, m_ZombieCreateZone.transform.rotation);

            m_ListZombies.Add(newZombie);
        }
    }

    public override bool Initialize()
    {
        m_ZombieFactory = gameObject.AddComponent<ObjectFactory>();
        m_ZombieFactory.Initialize(10, "Prefabs/Zombies/Zombie", "Prefabs/Zombies/Models/Normal");

        m_ZombieCreateZone = GameObject.Find("ZombieCreateZone");

        if (m_ZombieCreateZone != null)
            m_ZombieCreateZone.GetComponent<MeshRenderer>().enabled = false;

        return true;
    }

    void LoadXML()
    {
        
    }

    
}
