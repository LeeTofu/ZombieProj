using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class SceneMaster : Singleton<SceneMaster>
{
    public GAME_SCENE m_NextScene { get; private set; }
    public GAME_SCENE m_CurrentScene { get; private set; }

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
                Debug.LogWarning("중복된 씬이 들어갔다. 확인" + sceneMain.m_Scene.ToString());
                continue;
            }

            GameObject newGo = Instantiate(go[i]);
            newGo.SetActive(false);
            newGo.transform.SetParent(transform);

            m_SceneInitializerTable.Add(sceneMain.m_Scene, newGo);
        }


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

        UIManager.Instance.LoadUI(m_CurrentScene);
        SoundManager.Instance.PlayBGM(m_CurrentScene);

        main.InitializeScene();

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

    public void LoadScene(GAME_SCENE _SceneLoad)
    {
        m_NextScene = _SceneLoad;
        SceneManager.LoadScene("LOADING");
    }

    private IEnumerator Loading()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(m_NextScene.ToString());
        op.allowSceneActivation = false;

        UIManager.Instance.AllUISetActive(false);

        if ( !m_SceneInitializerTable.ContainsKey(m_NextScene))
        {
            Debug.LogError("Resources/Prefabs/SceneMain에 Scene Initialize 만들어 오세요. 일단 메인으로 감");
            LoadScene(GAME_SCENE.MAIN);
            yield break;
        }

        yield return null;

        while(!op.isDone)
        {
            Debug.Log("Scene Loading...");

            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;

                if (!m_isLoadingInitialize)
                {
                    m_isLoadingInitialize = true;

                    GameObject go = m_SceneInitializerTable[m_NextScene];

                    go.SetActive(true);
                    go.GetComponent<SceneMain>().InitializeScene();

                    m_CurSceneMain = go;

                    UIManager.Instance.LoadUI(m_NextScene);
                    SoundManager.Instance.PlayBGM(m_NextScene);
                }
            }

            yield return null;
        }


        m_isLoadingInitialize = false;
        m_CurrentScene = m_NextScene;
    }
}
