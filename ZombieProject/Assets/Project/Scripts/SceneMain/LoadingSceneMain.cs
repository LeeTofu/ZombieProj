using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneMain : SceneMain
{
    public override bool InitializeScene()
    {
        Debug.Log("Loading Init ");
        return true;
    }
    public override bool DeleteScene()
    {
        return true;
    }
}
