using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : Singleton<RespawnManager>
{
    public int m_EndPhase = 10;

    // 좀비 각 페이즈마다 리스폰할 
    List<ZombieRespawn> m_ListZombiePhase = new List<ZombieRespawn>();

   // Dictionary<int, List<ZombieRespawn>> m_ZombiePhaseTable = new Dictionary<int, List<ZombieRespawn>>();
    public int m_CurRespawnZombieCount { private set; get; }
    public int m_CurWave { private set; get; }

    // 게임에서 패배했는가.
    public bool m_isGameOver;

    //게임에서 승리를 하셨나
    public bool m_isGameClear;

    public override bool Initialize()
    {
        m_isGameOver = false;
        m_isGameClear = false;
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
        ForceZombieClear();
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
        (UIManager.Instance.m_CurrentUI as BattleUI).PlayInfoMessage("15초 뒤 좀비가 몰려옵니다!");
        StartCoroutine(WaveChange_C(15.0f));
    }

    // 웨이브 변화 조건을 만족하면 다음 웨이브로 30초뒤에 진행됩니다.
    public void WaveChangeFunction()
    {
        if (CheckCanNextWave())
        {
            (UIManager.Instance.m_CurrentUI as BattleUI).PlayInfoMessage("정비 시간");

            StartCoroutine(WaveChange_C(30.0f));
        }
    }

    // Zombie를 오브젝트 풀에 넣을때 실행되는 함수입니다.
    public void PushToPoolZombieAction()
    {
        m_CurRespawnZombieCount--;

        if (m_CurRespawnZombieCount < 0)
        {
            m_CurRespawnZombieCount = 0;
        }
        // 좀비 제거 시 
        WaveChangeFunction();
    }

    IEnumerator WaveChange_C(float _changeTime)
    {
        yield return new WaitForSeconds(_changeTime);

        (UIManager.Instance.m_CurrentUI as BattleUI).PlayInfoMessage(" ");

        OccurZombiePhase(m_CurWave + 1);
    }

    //게임 종료/클리어시 발동
    void GameClear()
    {
        (UIManager.Instance.m_CurrentUI as BattleUI).EndWaveAction();
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
