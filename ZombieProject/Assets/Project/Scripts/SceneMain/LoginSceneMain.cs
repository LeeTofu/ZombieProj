using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSceneMain : SceneMain
{

    private void Start()
    {
        UIManager.Instance.LoadUI(m_Scene);
        SoundManager.Instance.PlayBGM(SOUND_BG_LOOP.BATTLE1);
    }

    public override bool InitializeScene()
    {
       
        return true;
    }
    public override bool DeleteScene()
    {
        return true;
    }
}
