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
            PlayerManager.Instance.SplashAttackToPlayer(transform.position, 5.0f, m_Stat.Attack, true, 5);
            BattleSceneMain.CreateFireArea_Zombie(transform.position + Vector3.up * 0.2f, transform.rotation);

            EffectManager.Instance.PlayEffect(
                PARTICLE_TYPE.EXPLOSION_HUGE,
                transform.position + Vector3.up * 0.3f,
                Quaternion.LookRotation(-transform.forward),
                Vector3.one * 1.4f,
                true, 1.0f);



        }
    }

}
