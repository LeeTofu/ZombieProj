using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 오브젝트 종류
public enum OBEJCT_SORT
{
    PLAYER = 1, // 플레이어
    PLAYER_MERCENARY, // 플레이어 용병
    PLAYER_OBJECT, // 플레이어가 설치한 오브적트

    ZOMBIE, // 좀비
    ELITE_ZOMBIE, // 네임드 좀비
    BOSS_ZOMBIE, // 보스 좀비
    ZOMBIE_OBJECT // 좀비들 오브젝트
}

public abstract class MovingObject : MonoBehaviour
{
    private GameObject m_Model;

    public OBEJCT_SORT m_Sort;

    [SerializeField]
    protected MoveController m_Controller;

    public abstract void Initialize(GameObject _model, MoveController _Controller);

    public void Walk()
    {
        m_Controller.WalkAction();
    }

    public void RayHit()
    {
        m_Controller.RayHitAction();
    }

    public void CollisionHit()
    {
        m_Controller.CollisionAction();
    }
}
