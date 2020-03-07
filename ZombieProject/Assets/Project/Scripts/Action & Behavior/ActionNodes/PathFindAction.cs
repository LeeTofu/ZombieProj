using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePathFindCondition : DecoratorNode
{

    public override NODE_STATE Tick()
    {
        //생성 이후부터 플레이어와의 거리가 주어진 값 이내가 될 때까지 쫓아감
        float distance = GetAttackObjectDistance();

        if (distance < m_Character.m_Stat.alertRange && distance > m_Character.m_Stat.Range || 
            m_Character.m_zombieState != ZOMBIE_STATE.PATHFIND)
        {
            MovingObject mobject = GetAttackObject();

            if (!m_Character.m_NavAgent.enabled)
                m_Character.m_NavAgent.enabled = true;

            m_Character.m_NavAgent.destination = mobject.transform.position;
            m_Character.m_NavAgent.isStopped = false;

            return NODE_STATE.SUCCESS;
        }

        return NODE_STATE.FAIL;
    }
}

public class ZombiePathFindAction : ActionNode
{
    Ray checkingRay;
    float originMoveSpd;

    private bool CheckObstacle()    //좀비와 플레이어 사이에 장애물이 없는지 체크
    {
        RaycastHit hit;

        checkingRay.origin = m_Character.gameObject.transform.position;
        checkingRay.direction = (GetAttackObject().transform.position -
            m_Character.gameObject.transform.position).normalized;

        if (Physics.Raycast(checkingRay, out hit, m_Character.m_Stat.Range))
        {
            if (hit.transform.tag.Equals("Player"))
            {
                return true;    //장애물 없다
            }
        }

        return false;   //장애물 있다
    }

    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;

        RuntimeAnimatorController ac = m_Character.m_Animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == "Zombie_Walk_F_1_Full_Loop_IPC")
                m_totalActionTime = ac.animationClips[i].length;
    }

    public override NODE_STATE Tick()
    {
        //플레이 부분
        if(GetAttackObjectDistance() <= m_Character.m_Stat.Range && !CheckObstacle()) //길찾기 탈출조건 : 공격 사거리 내에 있어야 하며, 공격 방향에 장애물이 없을때
        {
            m_Character.m_NavAgent.isStopped = true;

            return NODE_STATE.FAIL;
        }
        else
        {
            MovingObject mobject = GetAttackObject();

            m_Character.m_NavAgent.destination = mobject.transform.position;

            if (m_Character.m_zombieState != ZOMBIE_STATE.PATHFIND)
            {
                originMoveSpd = m_Character.m_Stat.MoveSpeed;

                if (originMoveSpd >= 2f) //애초에 빠른놈이면
                    m_Character.m_Animator.CrossFade("Chase", 0.1f);
                else
                    m_Character.m_Animator.CrossFade("Walk", 0.1f);
                m_Character.m_Animator.SetFloat("WalkSpeed", originMoveSpd * 1.5f);
                m_Character.m_zombieState = ZOMBIE_STATE.PATHFIND;
            }

            if (m_Character.m_Stat.MoveSpeed != originMoveSpd)  //collision trigger등으로 이동속도가 변경되었다면
            {
                if(m_Character.m_Stat.MoveSpeed >= 2f)
                    m_Character.m_Animator.CrossFade("Chase", 0.1f);
                else
                    m_Character.m_Animator.CrossFade("Walk", 0.1f);

                m_Character.m_Animator.SetFloat("WalkSpeed", m_Character.m_Stat.MoveSpeed * 1.5f);
                originMoveSpd = m_Character.m_Stat.MoveSpeed;
            }

            return NODE_STATE.RUNNING;
        }
    }
}