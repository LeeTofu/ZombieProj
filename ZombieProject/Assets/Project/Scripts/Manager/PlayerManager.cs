using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : Singleton<PlayerManager>
{

    [SerializeField]
    private GameObject m_PlayerHeadModelPrefabs;

    [SerializeField]
    private GameObject m_PlayerBodyModelPrefabs;

    public static MovingObject m_Player;

    // 플레이어를 도울 용병등등... 드론..? 용병?
    public List<MovingObject> m_PlayableObject = new List<MovingObject>();

    private ObjectFactory m_PlayerFactory;

    [HideInInspector]
    public GameObject m_PlayerCreateZone;

    void Start()
    {
        m_PlayerCreateZone = GameObject.Find("PlayerCreateZone");

        if(m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

    }


    public MovingObject CreatePlayer(Vector3 _pos, Quaternion _quat)
    {
        if (m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        // 설정 //
        m_Player = m_PlayerFactory.CreateObject(_pos, _quat);
        return m_Player;
    }

    public override bool Initialize()
    {
        m_PlayerFactory = gameObject.AddComponent<PlayerFactory>();
        m_PlayerFactory.Initialize();
        return true;
    }
}
