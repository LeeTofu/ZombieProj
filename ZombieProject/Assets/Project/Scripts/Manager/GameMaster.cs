﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_SCENE
{
    NONE,
    MAIN, // 메인 화면
    LOADING, //로딩 씬
    SELECT_STAGE, // 셀렉트 스테이지 씬
    LOGIN, // 로그인 씬
    SHOP, // 샵
    INVENTORY, // 인벤
    IN_GAME, // 인게임 씬 (인게임은 GAME_STAGE로 세부항목 설정해야함.)
    ZombieTestScene, // 테스트씬
    END

}


// IN_GAME 씬을 제외한 다른 씬은 모두 NONE 이어야함.
public enum GAME_STAGE
{
    NONE, //IN_GAME 씬이 아니다.
    STAGE_1,
    STAGE_2,
    END
}

public class GameMaster : Singleton<GameMaster>
{
    private void Awake()
    {
      //  Debug.Log( "가로 : "  + Screen.width);
        Screen.SetResolution(1280, (int)(1280 * 9.0f / 16.0f), true);

        Initialize();
    }

    public override bool Initialize()
    {
        NavMeshBaker.Instance.CreateManager();

        TextureManager.Instance.CreateManager();
        EnemyManager.Instance.CreateManager();
        PlayerManager.Instance.CreateManager();
        UIManager.Instance.CreateManager();
        SoundManager.Instance.CreateManager();
        SceneMaster.Instance.CreateManager();
       
        ItemManager.Instance.CreateManager();
        InvenManager.Instance.CreateManager();

        CameraManager.Instance.CreateManager();
        BulletManager.Instance.CreateManager();
        EffectManager.Instance.CreateManager();

        BuffManager.Instance.CreateManager();
        RespawnManager.Instance.CreateManager();

        return true;
    }
    public override void DestroyManager()
    {
        NavMeshBaker.Instance.DestroyManager();

        TextureManager.Instance.DestroyManager();
        EnemyManager.Instance.DestroyManager();
        PlayerManager.Instance.DestroyManager();
        UIManager.Instance.DestroyManager();
        SoundManager.Instance.DestroyManager();
        SceneMaster.Instance.DestroyManager();

        ItemManager.Instance.DestroyManager();
        InvenManager.Instance.DestroyManager();

        CameraManager.Instance.DestroyManager();
        BulletManager.Instance.DestroyManager();
        EffectManager.Instance.DestroyManager();

        BuffManager.Instance.DestroyManager();
        RespawnManager.Instance.DestroyManager();
    }
}
