using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneMain : SceneMain
{

    private BattleMapCreator m_BattleMapCreator;

    Transform m_PlayerCreateZone;
    Transform m_ZombieCreateZone;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(2.5f);


        PlayerManager.Instance.CreatePlayer(m_PlayerCreateZone.position, m_PlayerCreateZone.rotation);
        EnemyManager.Instance.CreateZombie(m_ZombieCreateZone.position, m_ZombieCreateZone.rotation);
    }

    public override bool DeleteScene()
    {
        return true;
    }

    public override bool InitializeScene()
    {

        if (m_BattleMapCreator == null)
            m_BattleMapCreator = gameObject.AddComponent<BattleMapCreator>();

        // 어떤 스테이지에 어떤 맵을 가져올 것인가를 결정하는 '데이터매니저' 만들것...

        m_BattleMapCreator.CreateMap(SceneMaster.Instance.m_CurrentGameStage, E_MAP.CITY1);

        m_PlayerCreateZone = m_BattleMapCreator.m_PlayerCreateZone;

        if (m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        m_ZombieCreateZone = m_BattleMapCreator.ZombieCreateZones;

        if (m_ZombieCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        ItemManager.Instance.DebuggingLogItem(1);
        ItemManager.Instance.DebuggingLogItem(2);
        ItemManager.Instance.DebuggingLogItem(3);

        Debug.Log("Battle init 불러옴");
        return true;
    }

}
