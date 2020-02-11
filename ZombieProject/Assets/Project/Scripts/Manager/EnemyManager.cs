using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


public class EnemyManager : Singleton<EnemyManager>
{

    private ObjectFactory m_ZombieFactory;

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

            float len = (zombie.transform.position - _pos).magnitude;

            if (len >= _maxDistance) continue;

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
        if (m_ZombieFactory == null) return null;

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

    // _maxCount 만큼 범위에 있는 좀비 객체들을 가져온다.
    public List<MovingObject> GetRangeZombies(Vector3 _pos, float _maxDistance,int _maxCount = 5)
    {
        if (m_ZombieFactory == null) return null;

        List<MovingObject> target = new List<MovingObject>();

        foreach (MovingObject zombie in m_ZombieFactory.m_ListAllMovingObject)
        {
            if (zombie.m_Stat.isDead) continue;
            if (!zombie.gameObject.activeSelf) continue;

            float len = (zombie.transform.position - _pos).magnitude;

            if (len < _maxDistance)
            {
                target.Add(zombie);

                if (target.Count >= _maxCount)
                    break;
            }
        }

        return target;
    }



    // w해당 범위의 좀비들의 어그로를 변경한당
    public void SetTargetingRangeZombies(MovingObject _targetObject, Vector3 _pos, float _maxDistance)
    {
        if (m_ZombieFactory == null) return;
        if (_targetObject == null) return;
        
        foreach (MovingObject zombie in m_ZombieFactory.m_ListAllMovingObject)
        {
            if (zombie.m_Stat.isDead) continue;
            if (!zombie.gameObject.activeSelf) continue;

            float len = (zombie.transform.position - _pos).magnitude;

            if (len < _maxDistance)
            {
                zombie.SetTarget(_targetObject);
            }
        }
    }

    public void SetZombieAttackObject(MovingObject _object)
    {
        m_ZombieAttackObject = _object;
    }

    public void CreateZombie(Vector3 _pos, Quaternion _quat, OBJECT_TYPE _ZombieType, STAT _stat)
    {
        if (m_ZombieFactory)
        {
            MovingObject zombie = m_ZombieFactory.PopObject(_pos, _quat, (int)_ZombieType);
            zombie.SetStat(_stat);
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
        AllZombiePushToMemory();
    }

    public override void DestroyManager()
    {
        AllZombiePushToMemory();
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

            m_ZombieFactory.Initialize("Prefabs/Zombies/RangeZombie", Resources.LoadAll<GameObject>("Prefabs/Zombies/Models/Normal"));
            m_ZombieFactory.CreateObjectPool((int)OBJECT_TYPE.RANGE_ZOMBIE, 30);

            m_ZombieFactory.Initialize("Prefabs/Zombies/DashZombie", Resources.LoadAll<GameObject>("Prefabs/Zombies/Models/Normal"));
            m_ZombieFactory.CreateObjectPool((int)OBJECT_TYPE.DASH_ZOMBIE, 30);

          //  m_ZombieFactory.Initialize("Prefabs/Zombies/BombZombie", Resources.LoadAll<GameObject>("Prefabs/Zombies/Models/Normal"));
          //  m_ZombieFactory.CreateObjectPool((int)OBJECT_TYPE.BOMB_ZOMBIE, 30);
        }

        m_ZombieFactory.m_PushMemoryAction =
        (MovingObject mobject) =>
        {
            RespawnManager.Instance.PushToPoolZombieAction();
        };
        
        return true;
    }

    void LoadXML()
    {
        
    }

    
}
