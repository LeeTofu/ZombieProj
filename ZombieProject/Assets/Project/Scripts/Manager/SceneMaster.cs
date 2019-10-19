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

    // Scene을 Init 하고 Delete 할 때 쓰는 Table과 같음.
    private Dictionary<GAME_SCENE, GameObject> m_SceneInitializer
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

            if (m_SceneInitializer.ContainsKey(sceneMain.m_Scene))
            {
                Debug.LogWarning("중복된 씬이 들어갔다. 확인" + sceneMain.m_Scene.ToString());
                continue;
            }

            GameObject newGo = Instantiate(go[i]);
            newGo.SetActive(false);
            newGo.transform.SetParent(transform);

            m_SceneInitializer.Add(sceneMain.m_Scene, newGo);
        }

        GAME_SCENE scene = GAME_SCENE.NONE;
        bool success = System.Enum.TryParse<GAME_SCENE>(SceneManager.GetActiveScene().name, out scene);
        if(!success)
        {
            Debug.Log("현재 Scene 없다는데? fail");
            return false;
        }

        m_CurrentScene = scene;
        m_CurSceneMain = m_SceneInitializer[scene];
        m_CurSceneMain.SetActive(true);
        SceneMain main = m_CurSceneMain.GetComponent<SceneMain>();

        main.InitializeScene();
     
        return true;
    }

    public void LoadSceneStart()
    {
        if (m_CurSceneMain != null)
        {
            m_CurSceneMain.GetComponent<SceneMain>().DeleteScene();
            m_CurSceneMain.SetActive(false);
        }
        SoundManager.Instance.StopCurrentBGM();

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

        if ( !m_SceneInitializer.ContainsKey(m_NextScene))
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
                m_CurrentScene = m_NextScene;

                op.allowSceneActivation = true;

                GameObject go = m_SceneInitializer[m_NextScene];

                go.SetActive(true);
                go.GetComponent<SceneMain>().InitializeScene();

                UIManager.Instance.LoadUI(m_NextScene);
                m_CurSceneMain = go;
            }

            yield return null;
        }

        m_CurrentScene = m_NextScene;
    }
}
