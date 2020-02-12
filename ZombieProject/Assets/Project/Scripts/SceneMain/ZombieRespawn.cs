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



    // 리스폰을 시작하자
    public void StartRespawn(int _phase)
    {
        m_CurRespawnCount = 0;
        m_isCompleteRespawn = false;


        if(m_RespawnArray.Length - 1 < _phase)
        {
            _phase = m_RespawnArray.Length - 1;
        }

        if (m_Coroutine == null)
        {
            m_Coroutine = StartCoroutine(RespawnStartZombie(m_RespawnArray[_phase]));
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
    private void RespawnZombie(OBJECT_TYPE _obj)
    {
        MovingObject zombie = EnemyManager.Instance.CreateZombie(transform.position, transform.rotation, _obj);

        if (zombie != null)
        {
            STAT stat = RespawnManager.Instance.GetZombieStat(_obj, RespawnManager.Instance.m_CurWave);
            
            if(stat != null)
                zombie.SetStat(stat.Clone());
            else
            {
                Debug.LogError("설정하신 스탯이 없다");
                return;
            }

            RespawnManager.Instance.m_CurZombieCount++;
            m_CurRespawnCount++;
        }
    }

    // 리스폰을 코루틴 돌려 활성화하자.
    IEnumerator RespawnStartZombie(OBJECT_TYPE _type)
    {
        yield return new WaitForSeconds(m_ZombieRespawnStartTime);

        while (m_CurRespawnCount < m_ZombieMaxRespawnCount)
        {
            RespawnZombie(_type);

            if (m_CurRespawnCount >= m_ZombieMaxRespawnCount)
            {
                m_isCompleteRespawn = true;
                yield break;
            }
            yield return new WaitForSeconds(Random.Range(m_minZombieRespawnTime, m_maxZombieRespawnTime));
        }

       
    }


}
