using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 맵종류
public enum E_MAP
{
    NONE, // 아무것도 없다
    HOSPITAL, // 병원
    BUNKER, // 벙커
    FOREST, //숲
    CITY1, // 도시2
    CITY2, // 도시1
    SUBURB, // 교외
    CONSTRUCTION_SITE // 공사장
}


public class BattleMapCreator : MonoBehaviour
{
    public Transform m_PlayerCreateZone { get; private set; }
    public Transform ZombieCreateZones { get; private set; }


    public bool CreateMap(GAME_STAGE _Stage, E_MAP _map)
    {

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
        ZombieCreateZones = bg.transform.Find("ZombieCreateZone");



        if (m_PlayerCreateZone == null)
            Debug.LogError("x");
        if (ZombieCreateZones == null)
            Debug.LogError("x2");

        return true;

    }

}
