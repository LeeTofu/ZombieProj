using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneMain : SceneMain
{
    private BattleMapCreator m_BattleMapCreator;

    Transform m_PlayerCreateZone;
    Transform m_ZombieCreateZone;

    static ObjectFactory s_DropItemFactory;
    static ObjectFactory s_FireFactory;
    static ObjectFactory s_Fire_ZombieFactory;

    Dictionary<int, List<ZombieRespawn>> m_ZombiePhaseTable = new Dictionary<int, List<ZombieRespawn>>();

    public IEnumerator E_Start()
    {
        while (m_PlayerCreateZone == null)
            yield return null;
        yield return new WaitForSeconds(0.1f);


        if (SceneMaster.Instance.m_CurrentGameStage == GAME_STAGE.STAGE_2 )
        {
            RenderSettings.ambientSkyColor = new Color(0.41f, 0.41f, 1.0f);
            RenderSettings.ambientEquatorColor = new Color(0.33f, 0.46f, 1.0f);
            RenderSettings.ambientGroundColor = new Color(0, 0.086f, 1.0f);
        }
        else
        {
            RenderSettings.ambientSkyColor = new Color(0.95f, 0.85f, 0.45f);
            RenderSettings.ambientEquatorColor = new Color(1, 1, 1.0f);
            RenderSettings.ambientGroundColor = new Color(1, 0.968f, 0.79f);
        }

        CameraManager.Instance.CameraInitialize(m_PlayerCreateZone.position);

        yield return new WaitForSeconds(3.0f);

        PlayerManager.Instance.CurrentMoney = 40;

        PlayerManager.Instance.CreatePlayer(m_PlayerCreateZone.position, m_PlayerCreateZone.rotation);
        CameraManager.Instance.SetTargeting(PlayerManager.Instance.m_Player.gameObject);


        if (s_DropItemFactory == null)
        {
            s_DropItemFactory = gameObject.AddComponent<ObjectFactory>();
            s_DropItemFactory.Initialize("Prefabs/BuffGiver/ItemBox", ("Prefabs/BuffGiver/Models/ItemBox"), (int)OBJECT_TYPE.BUFF_OBJECT);
            s_DropItemFactory.CreateObjectPool((int)OBJECT_TYPE.BUFF_OBJECT, 10);
        }

        if (s_FireFactory == null)
        {
            s_FireFactory = gameObject.AddComponent<ObjectFactory>();
            s_FireFactory.Initialize("Prefabs/BuffGiver/FireArea",("Prefabs/BuffGiver/Models/Fire"), (int)OBJECT_TYPE.BUFF_OBJECT);
            s_FireFactory.CreateObjectPool((int)OBJECT_TYPE.BUFF_OBJECT, 10);
        }

        if (s_Fire_ZombieFactory == null)
        {
            s_Fire_ZombieFactory = gameObject.AddComponent<ObjectFactory>();
            s_Fire_ZombieFactory.Initialize("Prefabs/BuffGiver/FireArea_Zombie", ("Prefabs/BuffGiver/Models/Fire"), (int)OBJECT_TYPE.BUFF_OBJECT);
            s_Fire_ZombieFactory.CreateObjectPool((int)OBJECT_TYPE.BUFF_OBJECT, 10);
        }

        // CreateBuffItem(m_PlayerCreateZone.position + Vector3.forward * 5.0f, m_PlayerCreateZone.rotation);

        RespawnManager.Instance.GameStartWave();

    }


    public override bool DeleteScene()
    {
        StopCoroutine(E_Start());
        s_DropItemFactory.AllPushToMemoryPool((int)OBJECT_TYPE.BUFF_OBJECT);
        s_FireFactory.AllPushToMemoryPool((int)OBJECT_TYPE.BUFF_OBJECT);

        PlayerManager.Instance.CurrentMoney = 0;
        return true;
    }

    // 이거 로딩중에 실행되는 함수이므로 주의해서 쓰세요.
    // 맵 로딩 중
    // 씬 다 초기화 안될때 불리는 함수임.
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

        EnemyManager.Instance.Initialize_InGame();

        Debug.Log("Battle init 불러옴");
        StartCoroutine(E_Start());
        return true;
    }

    static public void CreateBuffItem(Vector3 _pos, Quaternion _quat)
    {
        s_DropItemFactory.PopObject(_pos, _quat, (int)OBJECT_TYPE.BUFF_OBJECT);
    }

    static public void CreateFireArea(Vector3 _pos, Quaternion _quat)
    {
        s_FireFactory.PopObject(_pos, _quat, (int)OBJECT_TYPE.BUFF_OBJECT);
    }

    static public void CreateFireArea_Zombie(Vector3 _pos, Quaternion _quat)
    {
        s_Fire_ZombieFactory.PopObject(_pos, _quat, (int)OBJECT_TYPE.BUFF_OBJECT);
    }

}
