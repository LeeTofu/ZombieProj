using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : Singleton<GameMaster>
{
    public GameMaster m_Test;

    private void Awake()
    {
        Initialize();
    }

    public override bool Initialize()
    {
        EnemyManager.Instance.CreateManager();
        PlayerManager.Instance.CreateManager();
        SoundManager.Instance.CreateManager();

        return true;
    }
}
