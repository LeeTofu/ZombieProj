using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUI : BaseUI
{
    private void Start()
    {
        Debug.Log("Login UI 불러옴");
    }

    public override void InitializeUI()
    {

    }

    public override void DeleteUI()
    {

    }

    public void PressStartButton()
    {
        SceneMaster.Instance.LoadScene(GAME_SCENE.MAIN);
    }
}
