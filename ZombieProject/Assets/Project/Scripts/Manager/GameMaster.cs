using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_SCENE
{
    NONE,
    MAIN,
    LOADING,
    SELECT_STAGE,
    LOGIN,
    SHOP,
    IN_GAME,
    END

}

public class GameMaster : Singleton<GameMaster>
{
    private void Awake()
    {
        Initialize();
    }

    public override bool Initialize()
    {
        EnemyManager.Instance.CreateManager();
        PlayerManager.Instance.CreateManager();
        SoundManager.Instance.CreateManager();
        SceneMaster.Instance.CreateManager();
        UIManager.Instance.CreateManager();

        return true;
    }
}
