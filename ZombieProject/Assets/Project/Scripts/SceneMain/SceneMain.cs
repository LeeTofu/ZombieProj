using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 이 놈 용도는 처음 씬을 초기화 하거나 릴리즈 할때 쓰는 클래스임.
// 그 외는 안씀
public abstract class SceneMain : MonoBehaviour
{
    public GAME_STAGE m_Stage { get; private set; }

    [SerializeField]
    public GAME_SCENE m_Scene;
    public abstract bool InitializeScene();
    public abstract bool DeleteScene();
}
