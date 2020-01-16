using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : STAT
{
    public float m_DurationTime; // 버프가 전체 몇초간 지속되는가
    public int m_TickTime; // 이 버프가 지속적으로 무언가를 하는 버프라면 몇 초 간격으로 할것인가.

    protected float m_CurTimeDuration = 0.0f;// 현재 버프 시간

    protected STAT m_Stat;
    public Buff(STAT _stat)
    { 
        m_Stat = _stat;
    }
    
    public void SetStat(STAT _stat)
    {
        m_Stat = _stat;
    }

    // 버프 종료 후 실행하는 액션 함수 
    public System.Action<Buff> m_BuffExitAction;

    // 버프 액션
    protected abstract void BuffAction();

    // 버프 종료시 처리.
    protected abstract void BuffExitAction();

    // 그냥 단일 버프
    public IEnumerator OnceCoroutine()
    {
         BuffAction();
         yield return new WaitForSeconds(m_DurationTime);
         BuffExitAction();
    }


    // 시간에 맞추어 여러번 버프 주는 액션을 하는 버프
    public IEnumerator TimeTickCorotine()
    {
        while (m_CurTimeDuration < m_DurationTime)
        {
            BuffAction();

            if (isDead)
                yield break;

            m_CurTimeDuration += m_TickTime;

            if (m_CurTimeDuration >= m_DurationTime)
            {
                yield return new WaitForSeconds(m_CurTimeDuration - m_DurationTime);
            }
            else
                yield return new WaitForSeconds(m_TickTime);
        }

        BuffExitAction();
    }
}
public class Adrenaline : Buff
{
     public Adrenaline(STAT _stat) : base(_stat) 
    {
        Debug.Log("아드레날린 분비");
        BuffCoroutine = OnceCoroutine();
    }
    protected override void BuffAction()
     {
        m_Stat.MoveSpeed = m_Stat.MoveSpeed * 2.0f;
     }

    protected override void BuffExitAction()
    {
        Debug.Log("아드레날린 분비 끝");
        m_Stat.MoveSpeed = m_Stat.MoveSpeed / 2.5f;
        m_BuffExitAction(this);
        return;
    }
}

public class Blessing : Buff
{
    public Blessing(STAT _stat) : base(_stat)
    {
        BuffCoroutine = TimeTickCorotine();
    }
    protected override void BuffAction()
    {
        m_Stat.CurHP += 2.0f;
        Debug.Log("Healing:" + m_Stat.CurHP);
    }
    protected override void BuffExitAction()
    {
        m_BuffExitAction(this);
        return;
    }
}

public class Poison : Buff
{
    public Poison(STAT _stat) : base(_stat)
    {
         BuffCoroutine = TimeTickCorotine();
    }
    protected override void BuffAction()
    {
        m_Stat.CurHP -= 2.0f;
        Debug.Log("PoisonDamage:" + m_Stat.CurHP);
    }

    protected override void BuffExitAction()
    {
        m_BuffExitAction(this);
        return;
    }
}

