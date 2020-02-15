using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectUI : BaseUI
{
    [SerializeField]
    private RawImage m_StageInfoImage;

    [SerializeField]
    private TextMeshProUGUI m_StageInfoText;

    private Texture[] m_StageTextures;
    private GAME_STAGE m_CurretSelectedStage = GAME_STAGE.STAGE_1;


    public override void InitializeUI()
    {
        if(m_StageTextures == null)
        m_StageTextures = Resources.LoadAll<Texture>("Image/StageInfo");

        if(m_StageTextures != null)
        {
            //for(int i = 0; i < m_StageTextures.Length; i++)
            //{
            ////    Debug.Log(m_StageTextures[i].name + "Success ");
            //}

        }

        SetStageInfo();
    }

    void SetStageInfo()
    {
        m_StageInfoImage.texture = m_StageTextures[(int)1];
        m_StageInfoText.text = "랜 덤";
    }

    public override void DeleteUI()
    {

    }

    public void PressPrevButton()
    {
        m_CurretSelectedStage = m_CurretSelectedStage - 1;

        if (m_CurretSelectedStage <= GAME_STAGE.NONE)
        {
            m_CurretSelectedStage = GAME_STAGE.STAGE_1;
           
        }

        SetStageInfo();
    }

    public void PressNextButton()
    {
        m_CurretSelectedStage = m_CurretSelectedStage + 1;

        if(m_CurretSelectedStage >= GAME_STAGE.END)
        {
            m_CurretSelectedStage = GAME_STAGE.STAGE_1;
           
        }

        SetStageInfo();
    }

    public void SelectStage()
    {
        SceneMaster.Instance.LoadBattleScene(m_CurretSelectedStage);
    }

    public void BackButton()
    {
        DeleteUI();
        gameObject.SetActive(false);
        UIManager.Instance.m_CurrentUI.InitializeUI();
    }
}
