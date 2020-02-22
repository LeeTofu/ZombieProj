using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BombZombie : Zombie
{
    protected override void DeadActionCallback()
    {
        base.DeadActionCallback();
        if (SceneMaster.Instance.m_CurrentScene == GAME_SCENE.IN_GAME)
        {
            BattleSceneMain.CreateFireArea(transform.position, transform.rotation);


        }
    }

}
