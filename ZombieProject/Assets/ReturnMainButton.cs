using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReturnMainButton : UIPressSubject
{
    public override void OnPointerDown(PointerEventData eventData)
    {
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
    }
    public override void OnPressed()
    {
        SceneMaster.Instance.LoadScene(GAME_SCENE.MAIN);
        GameMaster.Instance.DestroyManager();
        BattleUI.SetDeathPanelActive(false);
    }
}
