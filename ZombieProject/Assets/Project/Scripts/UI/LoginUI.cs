using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUI : BaseUI
{
    [SerializeField]
    TMPro.TMP_InputField m_IDInput;

    [SerializeField]
    TMPro.TMP_InputField m_PWInput;

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
        //  SceneMaster.Instance.LoadScene(GAME_SCENE.MAIN);
#if UNITY_EDITOR
        SceneMaster.Instance.LoadScene(GAME_SCENE.MAIN);
#else
        LoginManager.Instance.LoginToGoogle();
        //JoinToTheFireBase();
#endif
    }

    public void JoinToTheFireBase()
    {
       LoginManager.Instance.LoginToFireBase("qnpfrehdtk32s@gmail.com", "tlgja-312!");
    }


}
