using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    private void Start()
    {
        
        SceneMaster.Instance.LoadSceneStart();
    }
}
