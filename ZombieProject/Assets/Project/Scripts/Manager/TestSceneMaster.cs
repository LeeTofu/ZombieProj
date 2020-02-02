using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestSceneMaster : Singleton<TestSceneMaster>
{
    private void Awake()
    {
        Initialize();
    }

    public override bool Initialize()
    {
        Resources.Load("");

        GameObject bg = Instantiate(Resources.Load<GameObject>("Prefabs/BackGround/BackGround_HOSPITAL"));

        if (!bg)
        {
            Debug.LogError("맵을 만들다가 실패함. ");
            return false;
        }

        bg.transform.position = Vector3.zero;
        bg.transform.rotation = Quaternion.identity;

        //맵 navmesh 만드는 코드
        

        return true;
    }
    public override void DestroyManager()
    {
    }
}
