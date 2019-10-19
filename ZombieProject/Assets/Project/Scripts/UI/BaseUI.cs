using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{

    public GAME_SCENE m_Scene;
    public abstract void InitializeUI();

    public abstract void DeleteUI();
}
