using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : STAT
{
    public float m_DurationTime; // 버프가 전체 몇초간 지속되는가
    public int m_TickTime; // 이 버프가 지속적으로 무언가를 하는 버프라면 몇 초 간격으로 할것인가.

    protected float m_CurTimeDuration = 0.0f;// 현재 버프 시간

    public int m_Level;

    public BUFF_TYPE m_BuffType;
    
    public string m_ImagePath;

    public STAT m_Stat;
    public Buff(STAT _stat)
    { 
        m_Stat = _stat;
        m_ImagePath = "Image/UIIcon/";
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
        m_BuffType = BUFF_TYPE.ADRENALINE;
        m_ImagePath += "Speed";
        Debug.Log("아드레날린 분비");
        BuffCoroutine = OnceCoroutine();
    }
    protected override void BuffAction()
     {
        Debug.Log( MoveSpeed );
        m_Stat.MoveSpeed *= MoveSpeed;
     }

    protected override void BuffExitAction()
    {
        Debug.Log("아드레날린 분비 끝");
        m_Stat.MoveSpeed /= MoveSpeed;
        m_BuffExitAction(this);
        return;
    }
}

public class Blessing : Buff
{
    public Blessing(STAT _stat) : base(_stat)
    {
        m_BuffType = BUFF_TYPE.BLESSING;
        m_ImagePath += "Plus2";
        BuffCoroutine = TimeTickCorotine();
    }
    protected override void BuffAction()
    {
        m_Stat.CurHP += Attack;
        Debug.Log("Healing:" + m_Stat.CurHP);
        Debug.Log(m_DurationTime);
        Debug.Log(m_CurTimeDuration);
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
        m_BuffType = BUFF_TYPE.POISON;
        m_ImagePath += "";
        BuffCoroutine = TimeTickCorotine();
    }
    protected override void BuffAction()
    {
        m_Stat.CurHP -= Attack;
        m_Stat.MoveSpeed *= MoveSpeed;
        Debug.Log("PoisonDamage:" + Attack);
    }

    protected override void BuffExitAction()
    {
        m_Stat.MoveSpeed /= MoveSpeed;
        m_BuffExitAction(this);
        return;
    }
}

