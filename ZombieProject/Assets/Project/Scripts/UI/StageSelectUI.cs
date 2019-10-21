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
            for(int i = 0; i < m_StageTextures.Length; i++)
            {
                Debug.Log(m_StageTextures[i].name + "Success ");
            }

        }

        m_StageInfoImage.texture = m_StageTextures[(int)m_CurretSelectedStage];
        m_StageInfoText.text = m_CurretSelectedStage.ToString();
    }

    public override void DeleteUI()
    {

    }

    public void PressPrevButton()
    {
        m_CurretSelectedStage = m_CurretSelectedStage - 1;

        if (m_CurretSelectedStage <= GAME_STAGE.NONE)
        {
            m_CurretSelectedStage = GAME_STAGE.STAGE_5;
           
        }

        m_StageInfoImage.texture = m_StageTextures[(int)(m_CurretSelectedStage - 1)];
        m_StageInfoText.text = m_CurretSelectedStage.ToString();
    }

    public void PressNextButton()
    {
        m_CurretSelectedStage = m_CurretSelectedStage + 1;

        if(m_CurretSelectedStage >= GAME_STAGE.END)
        {
            m_CurretSelectedStage = GAME_STAGE.STAGE_1;
           
        }

        m_StageInfoImage.texture = m_StageTextures[(int)(m_CurretSelectedStage - 1)];
        m_StageInfoText.text = m_CurretSelectedStage.ToString();
    }

    public void SelectStage()
    {

    }

    public void BackButton()
    {

    }
}
