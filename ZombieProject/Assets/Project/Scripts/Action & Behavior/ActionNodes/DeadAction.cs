using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZombieDeadCondition : DecoratorNode
{
    public override NODE_STATE Tick()
    {
        if (m_Character.m_Stat.isDead)
        {
            return NODE_STATE.SUCCESS;
        }
        return NODE_STATE.FAIL;
    }

}
public class ZombieDeadAction : ActionNode
{
    public override void Initialize(MovingObject _character)
    {
        m_Character = _character;
    }

    public override NODE_STATE Tick()
    {
        //플레이 부분
        if (m_Character.m_zombieState != ZOMBIE_STATE.DEAD)
        {
            m_Character.m_Animator.CrossFade("Dead", 0.1f);
            m_Character.m_zombieState = ZOMBIE_STATE.DEAD;
        }

        return NODE_STATE.SUCCESS;
    }
}