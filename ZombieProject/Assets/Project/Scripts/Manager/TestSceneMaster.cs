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
        //PlayerManager.Instance.CreateManager();
        //EnemyManager.Instance.CreateManager();
        NavMeshBaker.Instance.CreateManager();


        //맵 로드(배틀맵 크리에이터에는 테스트용 맵이 아니어서 아직 적용 X)
        Resources.Load("");

        GameObject bg = Instantiate(Resources.Load<GameObject>("Prefabs/PathFinding/TestBackGround_HOSPITAL"));

        if (!bg)
        {
            Debug.LogError("맵을 만들다가 실패함. ");
            return false;
        }

        bg.transform.position = Vector3.zero;
        bg.transform.rotation = Quaternion.identity;


        //맵 navmesh 만드는 코드
        NavMeshSurface surface = bg.gameObject.GetComponentInChildren<NavMeshSurface>();

        NavMeshBaker.Instance.AddNavSurface(surface);

        NavMeshBaker.Instance.BakeNavMeshes();

        return true;
    }
    public override void DestroyManager()
    {
    }
}
