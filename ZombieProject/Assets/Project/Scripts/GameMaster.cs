using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : Singleton<GameMaster>
{
    public GameMaster m_Test;
    private void Awake()
    {
        base.Awake();
    }
    public override bool Initialize()
    {
        EnemyManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();


        return true;
    }
}
