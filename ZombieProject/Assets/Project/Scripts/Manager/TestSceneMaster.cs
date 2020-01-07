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
        //EnemyManager.Instance.CreateManager();
        //PlayerManager.Instance.CreateManager();

        //PlayerManager.Instance.CreatePlayer(Vector3.forward * 3f, Quaternion.identity);
        //EnemyManager.Instance.CreateZombie(Vector3.zero, Quaternion.identity, OBJECT_TYPE.ZOMBIE);

        //NavMeshMakingCam.Instance.CreateManager();
        NavMeshMakingCam.Instance.CreateManager();
        NavMeshMakingTool.Instance.CreateManager();

        return true;
    }
}
