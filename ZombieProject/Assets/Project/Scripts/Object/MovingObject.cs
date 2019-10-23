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

public struct STAT
{
    public float MoveSpeed;
    public float RotSpeed;
    
    public float Attack;
    public float Range;
    public float Def;

    public float Cur_HP;
    public float Max_HP;
}

public abstract class MovingObject : MonoBehaviour
{
    private GameObject m_Model;

    public STAT m_Stat;
    public OBEJCT_SORT m_Sort;

    [SerializeField]
    protected MoveController m_Controller;

    public abstract void Initialize(GameObject _model, MoveController _Controller);
    public virtual void SetStat(STAT _stat)
    {
        m_Stat = _stat;
    }
}
