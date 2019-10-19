using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneMain : SceneMain
{

    GameObject m_ZombieCreateZone;

    public IEnumerator Start()
    {
       
        SoundManager.Instance.PlayBGM(SOUND_BG_LOOP.BATTLE2);

        yield return new WaitForSeconds(1.0f);

        m_ZombieCreateZone = GameObject.Find("ZombieCreateZone");
        EnemyManager.Instance.CreateZombie(m_ZombieCreateZone.transform.position, m_ZombieCreateZone.transform.rotation);
    }

    public override bool DeleteScene()
    {
        return true;
    }

    public override bool InitializeScene()
    {
        Debug.Log("Battle init 불러옴");
        return true;
    }
}
