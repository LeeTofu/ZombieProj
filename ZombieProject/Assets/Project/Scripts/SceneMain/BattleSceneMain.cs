using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneMain : SceneMain
{
    GameObject m_PlayerCreateZone;
    GameObject m_ZombieCreateZone;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);

        m_PlayerCreateZone = GameObject.Find("PlayerCreateZone");

        if (m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        PlayerManager.Instance.CreatePlayer(m_PlayerCreateZone.transform.position, m_PlayerCreateZone.transform.rotation);

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
