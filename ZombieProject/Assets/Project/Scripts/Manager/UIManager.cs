using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public BaseUI m_CurrentUI { get; private set; }
    private Dictionary<GAME_SCENE, GameObject> m_GameUITable = new Dictionary<GAME_SCENE, GameObject>();

    public void Update()
    {



    }


    public override bool Initialize()
    {
        GameObject[] go = Resources.LoadAll<GameObject>("Prefabs/UI");

        for (int i = 0; i < go.Length; i++)
        {
            BaseUI sceneUI = go[i].GetComponent<BaseUI>();

            if (m_GameUITable.ContainsKey(sceneUI.m_Scene))
            {
                Debug.LogWarning("중복된 UI가 들어갔다. 확인할것");
                continue;
            }

            GameObject obj = Instantiate(go[i]);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            m_GameUITable.Add(sceneUI.m_Scene, obj);
        }
        return true;
    }

    public override void DestroyManager()
    {
    }
    public void AllUISetActive(bool _active)
    {
        for(GAME_SCENE i = GAME_SCENE.MAIN; i < GAME_SCENE.END; i++)
        {
            if (!m_GameUITable.ContainsKey(i)) continue;

            if (m_GameUITable[i].activeSelf != _active)
            {
                if (_active == false)
                {
                    m_GameUITable[i].GetComponent<BaseUI>().DeleteUI();
                    m_GameUITable[i].SetActive(_active);
                }
                else if (_active == true)
                {
                    m_GameUITable[i].SetActive(_active);
                    m_GameUITable[i].GetComponent<BaseUI>().InitializeUI();
                }
            }
        }
    }

    public GameObject GetUIObject(GAME_SCENE _scene)
    {
        return m_GameUITable[_scene];
    }

    public void LoadUI(GAME_SCENE _scene)
    {
        if (!m_GameUITable.ContainsKey(_scene))
        {
            Debug.Log("불러올 Scene의 UI가 Table에 없는데? UI 오브젝트를 테이블에 넣으셈 : " + _scene.ToString());
            return;
        }

        if (m_CurrentUI != null)
        {

            if (_scene == m_CurrentUI.m_Scene)
                 return;

            m_CurrentUI.DeleteUI();
            m_CurrentUI.gameObject.SetActive(false);
        }

        BaseUI ui =  m_GameUITable[_scene].GetComponent<BaseUI>();

        if(ui == null)
        {
            Debug.Log("테이블에 가져온 게임오브젝트에 BaseUI 부모를 가진 스크립트가 없는대?");
            return;
        }

        ui.gameObject.SetActive(true);
        Debug.Log("액티브됬는데?");
        ui.InitializeUI();

        m_CurrentUI = ui;
    }


}
