using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneMaster : Singleton<TestSceneMaster>
{
    private void Awake()
    {
        Initialize();
    }

    public override bool Initialize()
    {
        EnemyManager.Instance.CreateManager();
        PlayerManager.Instance.CreateManager();

        PlayerManager.Instance.CreatePlayer(Vector3.forward * 9f + Vector3.right * 4f, Quaternion.identity);
        EnemyManager.Instance.CreateZombie();

        return true;
    }
}