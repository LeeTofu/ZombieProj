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
        ItemManager.Instance.LoadItemData();
        return true;
    }
}
