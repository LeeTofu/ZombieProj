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

    // 좀비 각 페이즈마다 리스폰할 
    Dictionary<int, List<ZombieRespawn>> m_ZombiePhaseTable = new Dictionary<int, List<ZombieRespawn>>();
    List<ZombieRespawn> m_curListRespawn;

    private ObjectFactory m_ZombieFactory;

    [HideInInspector]
    public GameObject m_ZombieCreateZone;


    public int GetZombieCount()
    {
        return m_ListZombies.Count;
    }


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


    // 페이즈 발생 시키는 함수.
    public void OccurZombiePhase(int _phase)
    {
        AllStopRespawnZombie();
        RespawnPhaseZombie(_phase);
    }

    // 좀비 각 페이즈마다 생성할 좀비 리스폰을 등록한다.
    public void InsertPhaseZombie( int _phase ,ZombieRespawn _zombieRespawn)
    {
        List<ZombieRespawn> zombieList;

        if(m_ZombiePhaseTable.TryGetValue(_phase, out zombieList))
        {
            zombieList.Add(_zombieRespawn);
        }
        else
        {
            zombieList = new List<ZombieRespawn>();
            zombieList.Add(_zombieRespawn);

            m_ZombiePhaseTable.Add(_phase, zombieList);
        }
    }


    // 현재 에너미 매니저에 등록된 모든 좀비 리스폰 오브젝트를 해제합니다.
    // 씬 변경시 꼭 실행합시다.
    public void AllStopRespawnZombie()
    {
        List<ZombieRespawn> zombieList;

        for (int i = 0; i < m_ZombiePhaseTable.Count; i++)
        {
            if (m_ZombiePhaseTable.TryGetValue(i, out zombieList))
            {
                foreach (ZombieRespawn respawn in zombieList)
                {
                    respawn.StopRespawn();
                }
            }
        }
    }

    // 에너미에 등록된 모든 좀비 리스폰을 삭제.
    public void AllDeleteRespawnZombie()
    {
        List<ZombieRespawn> zombieList;

        for (int i = 0; i < m_ZombiePhaseTable.Count; i++)
        {
            if (m_ZombiePhaseTable.TryGetValue(i, out zombieList))
            {
                zombieList.Clear();
            }
        }
    }

    public void DeletaRespawnZombie(int _phase, ZombieRespawn _zombieRespawn)
    {
        List<ZombieRespawn> zombieList;
        if (m_ZombiePhaseTable.TryGetValue(_phase, out zombieList))
        {
            _zombieRespawn.StopRespawn();
            zombieList.Remove(_zombieRespawn);
        }
    }

    // 해당 _phase에 등록된 좀비 리스폰을 시작합니다.
    public void RespawnPhaseZombie(int _phase)
    {
        List<ZombieRespawn> zombieList;
        if (m_ZombiePhaseTable.TryGetValue(_phase, out zombieList))
        {
            foreach (ZombieRespawn respawn in zombieList )
            {
                respawn.StartRespawn();
            }
        }
    }

    private void PushToPoolEvent(MovingObject _zombie)
    {
        if (m_ListZombies == null) return;
        if (_zombie == null) return;

        m_ListZombies.Remove(_zombie);
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

    void AllZombiePushToMemory()
    {
        foreach(var zombie in m_ListZombies)
        {
            zombie.pushToMemory();
        }
    }


    // 인게임 들어갓을때 마다 실행되는 초기화 함수입니다.
    // 인게임 들어갈때 마다 초기화.
    // 실행되는 시점은 씬이 다 초기화 되기전임. 
    public void Initialize_InGame()
    {
        AllStopRespawnZombie();
        AllDeleteRespawnZombie();

        AllZombiePushToMemory();
        m_ListZombies.Clear();
       
    }

    // 매니저 생성시 무조건 실행되는 초기화 함수입니다.
    // 딱 한번만 매니저 생성시 함수 실행됨.
    public override bool Initialize()
    {
        m_ZombieFactory = gameObject.AddComponent<ObjectFactory>();
        m_ZombieFactory.Initialize(10, "Prefabs/Zombies/Zombie", "Prefabs/Zombies/Models/Normal");

        m_ZombieCreateZone = GameObject.Find("ZombieCreateZone");

        if (m_ZombieCreateZone != null)
            m_ZombieCreateZone.GetComponent<MeshRenderer>().enabled = false;

        m_ZombieFactory.m_PushMemoryAction = (_zombie) => { PushToPoolEvent(_zombie); };

        return true;
    }

    void LoadXML()
    {
        
    }

    
}
