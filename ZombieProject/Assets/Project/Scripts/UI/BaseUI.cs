using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{

    public GAME_SCENE m_Scene;

   
    protected void OKClick()
    {

    }

    protected void CancleClick()
    {

    }


    protected void MoveTo(GameObject _go, Vector3 _ToPos)
    {
        iTween.MoveTo(_go, _ToPos, 1.0f);
    }


    public abstract void InitializeUI();

    public abstract void DeleteUI();
}
