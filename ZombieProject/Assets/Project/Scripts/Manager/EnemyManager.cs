using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


public class EnemyManager : Singleton<EnemyManager>
{
    private Dictionary<int, STAT> m_NormalZombieStatTable = new Dictionary<int, STAT>();
    private Dictionary<int, STAT> m_NamedZombieStatTable = new Dictionary<int, STAT>();
    private Dictionary<int, STAT> m_BossZombieStatTable = new Dictionary<int, STAT>();

    // 좀비 각 페이즈마다 리스폰할 
    Dictionary<int, List<ZombieRespawn>> m_ZombiePhaseTable = new Dictionary<int, List<ZombieRespawn>>();
    List<ZombieRespawn> m_curListRespawn;

    private ObjectFactory m_ZombieFactory;

    [HideInInspector]
    public GameObject m_ZombieCreateZone;


    // 좀비의 어그로가 될 오브젝트
    public MovingObject m_ZombieAttackObject { private set; get; }

    public int GetZombieCount()
    {
        return m_ZombieFactory.m_ListAllMovingObject.Count;
    }

    // _pos 에서 가장 가가운 좀비 찾아낸다.
    public MovingObject GetNearestZombie(Vector3 _pos, float _maxDistance)
    {
        float maxLen = 1000000.0f;
        MovingObject target = null;

        foreach(MovingObject zombie in m_ZombieFactory.m_ListAllMovingObject)
        {
            if (zombie.m_Stat.isDead) continue;
            if (!zombie.gameObject.activeSelf) continue;

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

    // _pos 에서 _maxDistance 안에 있는 모든 좀비 찾아낸다.
    public List<MovingObject> GetRangeZombies(Vector3 _pos, float _maxDistance)
    {
        List<MovingObject> target = new List<MovingObject>();

        foreach (MovingObject zombie in m_ZombieFactory.m_ListAllMovingObject)
        {
            if (zombie.m_Stat.isDead) continue;
            if (!zombie.gameObject.activeSelf) continue;

            float len = (zombie.transform.position - _pos).magnitude;

            if (len < _maxDistance)
            {
                target.Add(zombie);
            }
        }

        return target;
    }

    public void SetZombieAttackObject(MovingObject _object)
    {
        m_ZombieAttackObject = _object;
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

    // 에너미 매니저에 등록된 모든 좀비 리스폰을 리스트에서 삭제합니다.
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

    // 에너미 매니저에 등록된 좀비 리스폰을 삭제합니다.
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

    public void CreateZombie(Vector3 _pos, Quaternion _quat, OBJECT_TYPE _ZombieType)
    {
        if(m_ZombieFactory )
        {
           m_ZombieFactory.PopObject(_pos, _quat, (int)_ZombieType);
        }
    }

    // 좀비리스트에 있는 모든 좀비를 메모리에 집어넣습니다.
    public void AllZombiePushToMemory()
    {
       foreach(MovingObject obj in m_ZombieFactory.m_ListAllMovingObject  )
        {
            obj.pushToMemory((int)obj.m_Type);
        }
    }

    // 인게임 들어갓을때 마다 실행되는 초기화 함수입니다.
    // 인게임 들어갈때 마다 초기화.
    // 실행되는 시점은 게임씬이 생성하기 전 로딩때 임. 
    public void Initialize_InGame()
    {
        AllStopRespawnZombie();
        AllDeleteRespawnZombie();
        AllZombiePushToMemory();
    }

    public override void DestroyManager()
    {
    }

    // 매니저 생성시 무조건 실행되는 초기화 함수입니다.
    // 딱 한번만 매니저 생성시 함수 실행됨.
    public override bool Initialize()
    {
        if (m_ZombieFactory == null)
        {

            m_ZombieFactory = gameObject.AddComponent<ObjectFactory>();

            m_ZombieFactory.Initialize("Prefabs/Zombies/Zombie", Resources.LoadAll<GameObject>("Prefabs/Zombies/Models/Normal"));
            m_ZombieFactory.CreateObjectPool((int)OBJECT_TYPE.ZOMBIE, 30);
        }
        
        return true;
    }

    void LoadXML()
    {
        
    }

    
}
