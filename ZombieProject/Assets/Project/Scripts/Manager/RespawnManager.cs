using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RespawnManager : Singleton<RespawnManager>
{
    public readonly int m_EndPhase = 10;

    // 좀비 각 페이즈마다 리스폰할 
    List<ZombieRespawn> m_ListZombiePhase = new List<ZombieRespawn>();

   // Dictionary<int, List<ZombieRespawn>> m_ZombiePhaseTable = new Dictionary<int, List<ZombieRespawn>>();
    public int m_CurRespawnZombieCount { set; get; }
    public int m_CurWave { private set; get; }

    // 게임에서 패배했는가.
    public bool m_isGameOver;

    //게임에서 승리를 하셨나
    public bool m_isGameClear;

    // 쉬는 시간인가?
    public bool m_isRest { private set; get; }

    public override bool Initialize()
    {
        m_isGameOver = false;
        m_isGameClear = false;
        m_isRest = true;
        m_CurWave = 0;

        return true;
    }

    public override void DestroyManager()
    {
        m_isGameOver = false;
        m_isGameClear = false;
        
        GameClear();
    }

    private void Update()
    {
#if UNITY_EDITOR
        ForceZombieClear();
#endif
    }

    // 다음 웨이브로 넘어가는 함수.
    public void OccurZombiePhase(int _phase)
    {
        if (_phase == m_CurWave) return;

        m_CurWave = _phase;
        (UIManager.Instance.m_CurrentUI as BattleUI).ChangeWaveAction(m_CurWave);
        AllStopRespawnZombie();
        RespawnPhaseZombie(_phase);
    }

    // 좀비 각 페이즈마다 생성할 좀비 리스폰을 등록한다.
    public void InsertPhaseZombie(ZombieRespawn _zombieRespawn)
    {
        m_ListZombiePhase.Add(_zombieRespawn);
    }

    // 현재 에너미 매니저에 등록된 모든 좀비 리스폰 오브젝트를 해제합니다.
    // 씬 변경시 꼭 실행합시다.
    public void AllStopRespawnZombie()
    {
        for (int i = 0; i < m_ListZombiePhase.Count; i++)
        {
             m_ListZombiePhase[i].StopRespawn();
        }
    }

    // 에너미 매니저에 등록된 모든 좀비 리스폰을 리스트에서 삭제합니다.
    public void AllDeleteRespawnZombie()
    {
        m_ListZombiePhase.Clear();
    }

    // 해당 _phase에 등록된 좀비 리스폰을 시작합니다.
    private void RespawnPhaseZombie(int _phase)
    {
        if(_phase >= m_EndPhase)
        {
            GameClear();
        }
        else
        {
            foreach (ZombieRespawn respawn in m_ListZombiePhase)
            {
                respawn.StartRespawn();
            }
        }
    }

    // 게임 처음 시작시 실행되는 함수.
    public void GameStartWave()
    {
#if UNITY_EDITOR
        StartCoroutine(WaveChange_C(15.0f));
#elif UNITY_ANDROID
            StartCoroutine(WaveChange_C(20.0f));
#endif
    }

    // 웨이브 변화 조건을 만족하면 다음 웨이브로 15초뒤에 진행됩니다.
    public void WaveChangeFunction()
    {
        if (CheckCanNextWave())
        {
            StartCoroutine(RestTime_C(10.0f));
        }
    }

    // 웨이브마다 있는 쉬는 시간
    IEnumerator RestTime_C(float _restTime)
    {
        (UIManager.Instance.m_CurrentUI as BattleUI).PlayInfoMessage("다음 전투를 위해 정비하세요!");

        m_isRest = true;
        yield return new WaitForSeconds(_restTime);

#if UNITY_EDITOR
        StartCoroutine(WaveChange_C(3.0f));
#elif UNITY_ANDROID
            StartCoroutine(WaveChange_C(5.0f));
#endif
    }

    // Zombie를 오브젝트 풀에 넣을때 실행되는 함수입니다.
    public void PushToPoolZombieAction()
    {
        m_CurRespawnZombieCount--;

        if (m_CurRespawnZombieCount < 0)
        {
            m_CurRespawnZombieCount = 0;
        }

        if(m_CurRespawnZombieCount == 0)
            WaveChangeFunction();
    }

    IEnumerator WaveChange_C(float _changeTime)
    {
        float time = 0.0f;
        while (time < _changeTime)
        {
            (UIManager.Instance.m_CurrentUI as BattleUI).PlayInfoMessage((int)(_changeTime - time) + "초 뒤 좀비가 몰려옵니다!");

            yield return new WaitForSeconds(1.0f);
            time += 1.0f;
        }

        (UIManager.Instance.m_CurrentUI as BattleUI).PlayInfoMessage("모든 좀비를 제거하고 생존하세요.");
        m_isRest = false;

        (UIManager.Instance.m_CurrentUI as BattleUI).NpcCollision(false);

        OccurZombiePhase(m_CurWave + 1);
    }

    //게임 클리어시 발동
    void GameClear()
    {
        (UIManager.Instance.m_CurrentUI as BattleUI).EndWaveAction();
        m_isGameClear = true;

        AllStopRespawnZombie();
        AllDeleteRespawnZombie();
    }

    //게임 종료/클리어시 발동
    public void GameOver()
    {
        (UIManager.Instance.m_CurrentUI as BattleUI).PlayInfoMessage("생존 실패");
        m_isGameClear = true;

        AllStopRespawnZombie();
        AllDeleteRespawnZombie();
    }

    // 다음 웨이브로 가도 되는지 조건을 계속 확인하는 함수.
    public bool CheckCanNextWave()
    {
          foreach (ZombieRespawn respawn in m_ListZombiePhase)
          {
              if (!respawn.m_isCompleteRespawn)
              {
                return false;
              }
          }

        if (m_CurRespawnZombieCount == 0)
              return true;
        return false;
    }

    public void ForceZombieClear()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EnemyManager.Instance.AllZombiePushToMemory();
        }
    }
}
