using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSceneMain : SceneMain
{

    public override bool InitializeScene()
    {
        Debug.Log("SelectSceneMain Init ");
        return true;
    }

    public override bool DeleteScene()
    {
        return true;
    }
}
