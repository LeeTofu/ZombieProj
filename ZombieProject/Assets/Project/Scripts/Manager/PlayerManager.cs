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
    public List<MovingObject> m_PlayableObject = new List<MovingObject>();

    private ObjectFactory m_PlayerFactory;

    [HideInInspector]
    public GameObject m_PlayerCreateZone;

    void Start()
    {
        m_PlayerCreateZone = GameObject.Find("PlayerCreateZone");
        m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        CreatePlayer(m_PlayerCreateZone.transform.position, m_PlayerCreateZone.transform.rotation);
    }


    public MovingObject CreatePlayer(Vector3 _pos, Quaternion _quat)
    {
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
