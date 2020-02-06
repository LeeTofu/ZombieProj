using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcCollisionAction : CollisionAction
{
    public new void Awake()
    {
        base.Awake();
        AddCollisionExitFunction((GameObject _gameObject)=>
        {
            (UIManager.Instance.m_CurrentUI as BattleUI).NpcCollision(false);
        });
    }
    protected override void CollisionEvent(GameObject _object)
    {
        (UIManager.Instance.m_CurrentUI as BattleUI).NpcCollision(true);
    }

    protected override bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (_defender.tag != "Player") return false;
        if (!RespawnManager.Instance.m_isRest) return false;

        return true;
    }

}
