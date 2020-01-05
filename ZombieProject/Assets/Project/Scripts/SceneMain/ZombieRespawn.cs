using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieRespawn : MonoBehaviour
{
    // 좀비를 소환할 페이즈
    [SerializeField]
    int m_Phase;

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

    // 리스폰 실행중인 코루틴.
    Coroutine m_Coroutine;

    [SerializeField]
    OBJECT_TYPE m_RespawnType;

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
        m_CurRespawnCount = 0;
        GetComponent<MeshRenderer>().enabled = false;
        EnemyManager.Instance.InsertPhaseZombie(m_Phase, this);
    }

    // 리스폰을 시작하자
    public void StartRespawn()
    {
        if(m_Coroutine == null)
        m_Coroutine = StartCoroutine(RespawnStartZombie());
    }


    // 리스폰을 멈추자.
    public void StopRespawn()
    {
        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);

        m_Coroutine = null;
    }


    private void RespawnZombie()
    {
        EnemyManager.Instance.CreateZombie(transform.position, transform.rotation, m_RespawnType);
        m_CurRespawnCount++;
    }

    IEnumerator RespawnStartZombie()
    {
        yield return new WaitForSeconds(m_ZombieRespawnStartTime);

        while (true)
        {
            while(m_CurRespawnCount >= m_ZombieMaxRespawnCount)
            {
                yield return null;
            }

            RespawnZombie();

            yield return new WaitForSeconds(Random.Range(m_minZombieRespawnTime, m_maxZombieRespawnTime));
        }
    }


}
