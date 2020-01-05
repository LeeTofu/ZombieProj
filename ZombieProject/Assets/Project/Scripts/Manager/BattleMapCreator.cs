using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 맵종류
public enum E_MAP
{
    NONE, // 아무것도 없다
    HOSPITAL, // 병원
    SEWER, // 하수도
    SUBURB, // 교외
    CITY1, // 도시2
    BANK, // 은행
    BUNKER, // 벙커
}


public class BattleMapCreator : MonoBehaviour
{
    public Transform m_PlayerCreateZone { get; private set; }
    public Transform ZombieCreateZones { get; private set; }

    NavMeshData m_NavMeshData;

    public bool CreateMap(GAME_STAGE _Stage, E_MAP _map)
    {
        Resources.Load("");

        GameObject bg = Instantiate(Resources.Load<GameObject>("Prefabs/BackGround/BackGround_" + _map.ToString()));

        SceneMaster.Instance.SetBattleMap(bg);

        if (!bg)
        {
            Debug.LogError(_Stage.ToString() + "의 " + _map.ToString() + " 맵을 만들다가 실패함. ");
            return false;
        }

        bg.transform.position = Vector3.zero;
        bg.transform.rotation = Quaternion.identity;
        
        m_PlayerCreateZone = bg.transform.Find("PlayerCreateZone");

        if (m_PlayerCreateZone == null)
            Debug.LogError("x");


        return true;

    }

}
