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

    public MovingObject CreatePlayer()
    {
        // 설정 //

        return m_Player;
    }

    protected override void Initialize()
    {
        throw new System.NotImplementedException();
    }
}
