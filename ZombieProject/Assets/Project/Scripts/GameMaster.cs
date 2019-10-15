using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : Singleton<GameMaster>
{
    public GameMaster m_Test;

    public override bool Initialize()
    {
        EnemyManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();
        SoundManager.Instance.Initialize();

        return true;
    }
}
