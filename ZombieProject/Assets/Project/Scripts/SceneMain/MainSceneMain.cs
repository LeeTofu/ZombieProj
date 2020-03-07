using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneMain : SceneMain
{


    public override bool DeleteScene()
    {
       
        return true;
    }

    public override bool InitializeScene()
    {
#if !UNITY_EDITOR
        DBManager.Instance.OnFirstReadDataRead();
#endif
        //   InvenManager.Instance.LoadItemInvenFromXML();
        return true;
    }
}
