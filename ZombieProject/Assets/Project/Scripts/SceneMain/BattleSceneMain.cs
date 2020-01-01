using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneMain : SceneMain
{
    private BattleMapCreator m_BattleMapCreator;

    Transform m_PlayerCreateZone;
    Transform m_ZombieCreateZone;

    ObjectFactory m_ItemFactory;

    public IEnumerator Start()
    {
        while (m_PlayerCreateZone == null)
            yield return null;
        yield return new WaitForSeconds(0.1f);

        CameraManager.Instance.CameraInitialize(m_PlayerCreateZone.position);

        yield return new WaitForSeconds(3.0f);
        
        PlayerManager.Instance.CreatePlayer(m_PlayerCreateZone.position, m_PlayerCreateZone.rotation);

        EnemyManager.Instance.CreateZombie(m_ZombieCreateZone.position, m_ZombieCreateZone.rotation);

        CameraManager.Instance.SetTargeting(PlayerManager.Instance.m_Player.gameObject);

        CreateBuffItem(m_PlayerCreateZone.position + Vector3.forward, m_PlayerCreateZone.rotation);

    }

    public override bool DeleteScene()
    {
        return true;
    }

    // 이거 로딩중에 실해되는 함수이므로 주의해서 쓰세요.
    // 맵 로딩 중
    public override bool InitializeScene()
    {

        if (m_BattleMapCreator == null)
            m_BattleMapCreator = gameObject.AddComponent<BattleMapCreator>();

        // 어떤 스테이지에 어떤 맵을 가져올 것인가를 결정하는 '데이터매니저' 만들것...

       E_MAP map = SceneMaster.Instance.GetBattleMap(SceneMaster.Instance.m_CurrentGameStage);

        m_BattleMapCreator.CreateMap(SceneMaster.Instance.m_CurrentGameStage, map);

        m_PlayerCreateZone = m_BattleMapCreator.m_PlayerCreateZone;

        if (m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        m_ZombieCreateZone = m_BattleMapCreator.ZombieCreateZones;

        if (m_ZombieCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;


        Debug.Log("Battle init 불러옴");
        return true;
    }

    public void CreateBuffItem(Vector3 _pos, Quaternion _quat)
    {
        if (m_ItemFactory == null)
        {
            m_ItemFactory = gameObject.AddComponent<ObjectFactory>();
            m_ItemFactory.Initialize(10, "Prefabs/Item/ItemBox", "Prefabs/Item/Models");
        }

        m_ItemFactory.CreateObject(_pos, _quat);
    }

}
