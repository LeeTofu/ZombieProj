using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : STAT
{
    public float m_DurationTime; // 버프가 전체 몇초간 지속되는가
    public float m_TickTime; // 이 버프가 지속적으로 무언가를 하는 버프라면 몇 초 간격으로 할것인가.

    protected float m_CurTimeDuration = 0.0f;// 현재 버프 시간

    public int m_Level;

    public BUFF_TYPE m_BuffType;
    
    public string m_Text;

    public STAT m_Stat;
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

    // 버프 처음 적용시 호출할 함수입니다.
    // 이펙트 재생이나, 무슨 이벤트 플레이용.
    public abstract void PlayBuffEffect(MovingObject _object);

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
        m_Text = "속도 증가";
        BuffCoroutine = OnceCoroutine();
    }

    public override void PlayBuffEffect(MovingObject _object)
    {
        if (_object == null) return;
        EffectManager.Instance.AttachEffect(PARTICLE_TYPE.ADRENALIN, _object, Vector3.up * 1.0f, Quaternion.identity, Vector3.one * 0.6f, true, m_DurationTime - 0.2f);

    }


    protected override void BuffAction()
     {
        m_Stat.MoveSpeed *= MoveSpeed;
     }

    protected override void BuffExitAction()
    {
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
        m_Text = "치료중";
        BuffCoroutine = TimeTickCorotine();
    }

    public override void PlayBuffEffect(MovingObject _object)
    {
        if (_object == null) return;

        EffectManager.Instance.PlayEffect(PARTICLE_TYPE.BUFF, _object.transform.position, Quaternion.Euler(270, 0, 0),
            Vector3.one * 1.2f, true, 10.0f);

        EffectManager.Instance.AttachEffect(PARTICLE_TYPE.HEAL, _object, Vector3.up * 1.0f, Quaternion.identity, Vector3.one * 2.0f, true, m_DurationTime - 0.2f);
        
    }

    protected override void BuffAction()
    {
        m_Stat.CurHP += Attack;
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
        m_Text = "독";
        BuffCoroutine = TimeTickCorotine();
    }
    public override void PlayBuffEffect(MovingObject _object)
    {
        if (_object == null) return;

        EffectManager.Instance.AttachEffect(PARTICLE_TYPE.HEAL, _object, Vector3.up * 0.2f, Quaternion.identity, Vector3.one, true, m_DurationTime - 0.2f);
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

