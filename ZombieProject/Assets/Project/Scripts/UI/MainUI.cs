using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MainUI : BaseUI
{
    [SerializeField]
    StageSelectUI m_stageSelectUI;

    [SerializeField]
    Button[] m_Buttons;

    [SerializeField]
    Image m_Title;

    [SerializeField]
    GameObject m_WarningWindow;




    private void Start()
    {
        m_WarningWindow.SetActive(false);
        Debug.Log("Main UI 불러옴");

        USER_DATA data = LoginManager.Instance.GetUserData();

    }

    public override void InitializeUI()
    {
        m_Title.gameObject.SetActive(true);

        for (int i = 0; i < m_Buttons.Length; i++)
        {
            m_Buttons[i].gameObject.SetActive(true);
        }

        m_stageSelectUI.InitializeUI();
        m_stageSelectUI.gameObject.SetActive(false);
    }


    public override void DeleteUI()
    {
        m_Title.gameObject.SetActive(false);

        for (int i = 0; i < m_Buttons.Length; i++)
        {
            m_Buttons[i].gameObject.SetActive(false);
        }

        m_stageSelectUI.gameObject.SetActive(false);
    }

    public void PressStartButton()
    {
        if (!InvenManager.Instance.isEquipedItemSlot(ITEM_SLOT_SORT.MAIN))
        {
            m_WarningWindow.SetActive(true);
            return;
        }

        if (m_WarningWindow.activeSelf) return;

        m_Title.gameObject.SetActive(false);

        for (int i = 0; i < m_Buttons.Length; i++)
        {
            m_Buttons[i].gameObject.SetActive(false);
        }

        if (!m_stageSelectUI.gameObject.activeSelf)
        {
            m_stageSelectUI.gameObject.SetActive(true);
            
        }
    }

    public void PressShopButton()
    {

        if (m_WarningWindow.activeSelf) return;

        SceneMaster.Instance.LoadScene(GAME_SCENE.SHOP);
    }

    public void PressInventoryButton()
    {

        if (m_WarningWindow.activeSelf) return;

        SceneMaster.Instance.LoadScene(GAME_SCENE.INVENTORY);
    }

    public void PressExitButton()
    {
        if (m_WarningWindow.activeSelf) return;

        //  SceneMaster.Instance.LoadScene(GAME_SCENE.IN_GAME);
    }

    
}