using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class SceneMaster : Singleton<SceneMaster>
{
    public GAME_STAGE m_CurrentGameStage { get; private set; }
    public GAME_SCENE m_NextScene { get; private set; }
    public GAME_SCENE m_CurrentScene { get; private set; }
    public GameObject m_CurrentBattleMap { get; private set; }
    GameObject m_CurSceneMain;

    bool m_isLoadingInitialize = false;

    // Scene을 Init 하고 Delete 할 때 쓰는 오브젝트를 모은 Table
    private Dictionary<GAME_SCENE, GameObject> m_SceneInitializerTable
        = new Dictionary<GAME_SCENE, GameObject>();

    public override bool Initialize()
    {
        
        GameObject[] go = Resources.LoadAll<GameObject>("Prefabs/SceneMain");

        for (int i = 0; i < go.Length; i++)
        {
            SceneMain sceneMain = go[i].GetComponent<SceneMain>();

            if (!sceneMain)
            {
                Debug.Log("Scene Load 실패");
                continue;
            }

            if (m_SceneInitializerTable.ContainsKey(sceneMain.m_Scene))
            {
                Debug.LogWarning("중복된 씬이 들어갔다. Prefabs/SceneMain에서 확인 : " + sceneMain.m_Scene.ToString());
                continue;
            }

            GameObject newGo = Instantiate(go[i]);
            newGo.SetActive(false);
            newGo.transform.SetParent(transform);

            m_SceneInitializerTable.Add(sceneMain.m_Scene, newGo);
        }

        m_CurrentBattleMap = null;
        m_CurrentGameStage = GAME_STAGE.NONE;
        StartSceneInitialize();
     
        return true;
    }

    // 에디터에서 최초로 실행한 씬 초기화 해주는 함수.
    void StartSceneInitialize()
    {
        GAME_SCENE scene = GAME_SCENE.NONE;
        bool success = System.Enum.TryParse<GAME_SCENE>(SceneManager.GetActiveScene().name, out scene);
        if (!success)
        {
            Debug.LogError("현재 이 Scene은 enum에 넣지 않은거 같은데? Scene enum에 씬 추가하셈.  fail");
            return;
        }

        m_CurrentScene = scene;


            m_CurSceneMain = m_SceneInitializerTable[scene];

            m_CurSceneMain.SetActive(true);
            SceneMain main = m_CurSceneMain.GetComponent<SceneMain>();

            if (m_CurrentScene == GAME_SCENE.IN_GAME)
            {
                m_CurrentGameStage = main.m_Stage;
                Debug.Log("InGame Scene이라 stage ; " + m_CurrentGameStage.ToString() + "불러옴");
            }

            UIManager.Instance.LoadUI(m_CurrentScene);
            SoundManager.Instance.PlayBGM(m_CurrentScene);

            main.InitializeScene();
        

    }

    public SceneMain GetCurrentMain()
    {
        if (m_CurSceneMain == null)
            return null;

        return m_CurSceneMain.GetComponent<SceneMain>();
    }

    public SceneMain GetGameMain(GAME_SCENE _scene)
    {
        GameObject main;
        if(m_SceneInitializerTable.TryGetValue(_scene, out main))
        {
            return main.GetComponent<SceneMain>();
        }

        return null;
    }

    public void SetBattleMap(GameObject _obj)
    {
        DestoryCurrentBattleMap();
        m_CurrentBattleMap = _obj;
        _obj.transform.SetParent(transform);
    }

    private void DestoryCurrentBattleMap()
    {
        if (m_CurrentBattleMap != null)
        {
            m_CurrentBattleMap.transform.SetParent(null);
            Destroy(m_CurrentBattleMap);
            m_CurrentBattleMap = null;
        }
    }

    public void LoadSceneStart()
    {
        if (m_CurSceneMain != null)
        {
            m_CurSceneMain.GetComponent<SceneMain>().DeleteScene();
            m_CurSceneMain.SetActive(false);
        }

        // 로딩씬에 있는 로딩 로더만 쓰는 함수임. 딴데 쓰지마셈.
        StartCoroutine(Loading());
    }

    // 일반 씬 불러올때 사용
    public void LoadScene(GAME_SCENE _SceneLoad)
    {
        if(_SceneLoad == GAME_SCENE.IN_GAME || _SceneLoad == GAME_SCENE.NONE || _SceneLoad == GAME_SCENE.END)
        {
            Debug.LogError("LoadScene 함수로 배틀씬 부를 수 없음. LoadBattleScene 함수로 부르세요");
            return;
        }

        m_CurrentGameStage = GAME_STAGE.NONE;
        m_NextScene = _SceneLoad;
        SceneManager.LoadScene("LOADING");
    }

    // 인 게임 씬 불러올 때 사용
    public void LoadBattleScene(GAME_STAGE _stage)
    {
        if (_stage == GAME_STAGE.NONE || _stage == GAME_STAGE.END)
        {
            Debug.LogError("LoadBattleScene 함수로 NONE, END 파라미터 못씀");
            return;
        }

        m_NextScene = GAME_SCENE.IN_GAME;
        m_CurrentGameStage = _stage;
        SceneManager.LoadScene("LOADING");
    }

    public E_MAP GetBattleMap(GAME_STAGE _gameStage)
    {
        switch (_gameStage)
        {
            case GAME_STAGE.STAGE_1:
            case GAME_STAGE.STAGE_1_HERO:
                return E_MAP.CITY1;
            case GAME_STAGE.STAGE_2:
            case GAME_STAGE.STAGE_2_HERO:
                return E_MAP.SEWER;
            case GAME_STAGE.STAGE_3:
            case GAME_STAGE.STAGE_3_HERO:
                return E_MAP.HOSPITAL;
            case GAME_STAGE.STAGE_4:
            case GAME_STAGE.STAGE_4_HERO:
                return E_MAP.BUNKER;
            default:
                return E_MAP.BUNKER;
        }
    }

    public string GetSceneName(GAME_STAGE _gameStage)
    {
        switch(_gameStage)
        {
            case GAME_STAGE.EVENT_DUNGEON:
                return "이벤트";
            case GAME_STAGE.MONEY_DUNGEON:
                return "폐허가 된 은행";
            case GAME_STAGE.MUGEN_ZOMBIE:
                return "무한 좀비";
            case GAME_STAGE.STAGE_1:
                return "마을";
            case GAME_STAGE.STAGE_1_HERO:
                return "마을 (지옥)";
            case GAME_STAGE.STAGE_2:
                return "지하 수도";
            case GAME_STAGE.STAGE_2_HERO:
                return "지하 수도 (지옥)";
            case GAME_STAGE.STAGE_3:
                return "병원";
            case GAME_STAGE.STAGE_3_HERO:
                return "병원 (지옥)";
            case GAME_STAGE.STAGE_4:
                return "벙커 내부";
            case GAME_STAGE.STAGE_4_HERO:
                return "벙커 내부 (지옥)";
            case GAME_STAGE.CHALLENGE:
                return "보스 사냥";
        }
        return " ";
    }


  
    private IEnumerator Loading()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(m_NextScene.ToString(), LoadSceneMode.Single);
        op.allowSceneActivation = false;

        if ( !m_SceneInitializerTable.ContainsKey(m_NextScene))
        {
            Debug.LogError("Resources/Prefabs/SceneMain에 Scene Initialize 만들어 오세요. 일단 메인으로 감" + m_NextScene);
            LoadScene(GAME_SCENE.MAIN);
            yield break;
        }

        yield return null;

        while(!op.isDone)
        {
            Debug.Log("Scene Loading...");

            if (op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.1f);

                op.allowSceneActivation = true;
                if (!m_isLoadingInitialize)
                {
                    DestoryCurrentBattleMap();

                    GameObject go = m_SceneInitializerTable[m_NextScene];

                    go.SetActive(true);
                    while (!go.GetComponent<SceneMain>().InitializeScene())
                    {
                        yield return null;
                    }

                    m_CurSceneMain = go;

                    UIManager.Instance.AllUISetActive(false);
                    m_isLoadingInitialize = true;

                   
                    m_CurSceneMain = go;
                }

                yield return null;
            }

            yield return null;
        }

        UIManager.Instance.LoadUI(m_NextScene);
        SoundManager.Instance.PlayBGM(m_NextScene);

        m_isLoadingInitialize = false;
        m_CurrentScene = m_NextScene;
    }
}
