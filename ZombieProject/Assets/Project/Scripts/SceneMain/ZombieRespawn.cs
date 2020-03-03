using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZombieRespawn : MonoBehaviour
{
    // 좀비 리스폰 시작 시간
    [SerializeField]
    float m_ZombieRespawnStartTime;

    // 좀비 리스폰 최소 시간 주기
    [SerializeField]
    float m_minZombieRespawnTime;

    // 좀비 리스폰 최대 시간 주기
    [SerializeField]
    float m_maxZombieRespawnTime;

    // 최대 좀비 소환하는 카운트 수
    [SerializeField]
    int m_ZombieMaxRespawnCount;

    // 현재 소환한 좀비 수
    int m_CurRespawnCount = 0;

    float m_CurTime = 0.0f;

    // 현재 웨이브에서 좀비를 다 소환했는지?
    // true면 좀비 리스폰을 다해서 이제 좀비 리스폰을 안하겠다는 뜻.
    [HideInInspector]
    public bool m_isCompleteRespawn;

    // 리스폰 실행중인 코루틴.
    Coroutine m_Coroutine;

    public OBJECT_TYPE[] m_RespawnArray;

    List<MovingObject> m_ListRespawnZombie = new List<MovingObject>();

    public int CurRespawnCount
    {
        get => m_CurRespawnCount;
        set
        {
            m_CurRespawnCount = value;
            if(m_CurRespawnCount < 0)
            {
                m_CurRespawnCount = 0;
            }
        }
    }

    // 처음 시작시 리스폰을 에너미매니저에 등록하자.
    private void Start()
    {
        m_isCompleteRespawn = false;
        m_CurRespawnCount = 0;
        GetComponent<MeshRenderer>().enabled = false;
        RespawnManager.Instance.InsertPhaseZombie(this);
    }

    // 리스폰할 좀비 타입 고르기.
    OBJECT_TYPE SelectZombieType(int _phase)
    {
        int NormalZombieRespawn = Mathf.Max(250, 1000 - ((_phase - 1) * 25));

        if(Random.Range(0,1000) < NormalZombieRespawn)
        {
            return OBJECT_TYPE.ZOMBIE;
        }
        else
        {
            int EliteZombieRespawn = Random.Range((int)OBJECT_TYPE.DASH_ZOMBIE, (int)OBJECT_TYPE.BULLET);
            return (OBJECT_TYPE)EliteZombieRespawn;
        }
    }

    // 리스폰을 시작하자
    public void StartRespawn(int _phase, int _respawnZombieCount)
    {
        m_CurRespawnCount = 0;
        m_isCompleteRespawn = false;
        m_ZombieMaxRespawnCount = _respawnZombieCount;

        if (m_Coroutine == null)
        {
              m_Coroutine = StartCoroutine(RespawnStartZombie(_phase));
        }
     
    }


    // 리스폰을 멈추자.
    public void StopRespawn()
    {
        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);

        m_Coroutine = null;
    }

    // 좀비를 리스폰하자.
    private void RespawnZombie(OBJECT_TYPE _obj, STAT _stat)
    {
        MovingObject zombie = EnemyManager.Instance.CreateZombie(transform.position, transform.rotation, _obj);

        if (zombie != null)
        {
            zombie.SetStat(_stat.Clone());

            if(zombie.m_HpBarUI != null)
                zombie.m_HpBarUI.InGame_Initialize();

            if(zombie.m_NavAgent != null)
            {
                zombie.m_NavAgent.stoppingDistance = zombie.m_Stat.Range;
                zombie.m_NavAgent.speed = zombie.m_Stat.MoveSpeed;
                if (!zombie.m_NavAgent.enabled)
                    zombie.m_NavAgent.enabled = true;
            }

            RespawnManager.Instance.m_CurZombieCount++;
            m_CurRespawnCount++;
        }
    }

    // 리스폰을 코루틴 돌려 활성화하자.
    IEnumerator RespawnStartZombie(OBJECT_TYPE _type, STAT _stat)
    {
        yield return new WaitForSeconds(m_ZombieRespawnStartTime);

        while (m_CurRespawnCount < m_ZombieMaxRespawnCount)
        {
            if (_stat != null)
            {
                RespawnZombie(_type, _stat);
            }
            else
            {
                Debug.LogError("Stat이 없다 : " + _type );
                yield break;
            }

            if (m_CurRespawnCount >= m_ZombieMaxRespawnCount)
            {
                m_isCompleteRespawn = true;
                yield break;
            }
            yield return new WaitForSeconds(Random.Range(m_minZombieRespawnTime, m_maxZombieRespawnTime));
        }
    }

    IEnumerator RespawnStartZombie(int _phase)
    {
        yield return new WaitForSeconds(m_ZombieRespawnStartTime);

        while (m_CurRespawnCount < m_ZombieMaxRespawnCount)
        {
            OBJECT_TYPE objectType = SelectZombieType(_phase);
            STAT stat = RespawnManager.Instance.GetZombieStat(objectType, _phase);

            if(objectType == OBJECT_TYPE.NONE )
            {
                Debug.LogError("이런 좀비 타입은 없다 " + objectType);
                yield break;
            }
            if (stat != null)
            {
                RespawnZombie(objectType, stat);
            }
            else
            {
                Debug.LogError("Stat이 없다 : " + objectType);
                yield break;
            }

            if (m_CurRespawnCount >= m_ZombieMaxRespawnCount)
            {
                m_isCompleteRespawn = true;
                yield break;
            }
            yield return new WaitForSeconds(Random.Range(m_minZombieRespawnTime, m_maxZombieRespawnTime));
        }
    }


}
