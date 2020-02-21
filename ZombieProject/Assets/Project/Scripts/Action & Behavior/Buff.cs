using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : STAT
{
    protected EffectObject m_EffectObject;

    public float m_DurationTime; // 버프가 전체 몇초간 지속되는가
    public float m_TickTime; // 이 버프가 지속적으로 무언가를 하는 버프라면 몇 초 간격으로 할것인가.

    protected float m_CurTimeDuration = 0.0f;// 현재 버프 시간

    public int m_Level;

    public BUFF_TYPE m_BuffType;
    
    public string m_Text;

    public STAT m_CharacterStat;

    public MovingObject m_MovingObject;

    public string m_ImageName;
    public Buff(STAT _stat)
    {
        m_CharacterStat = _stat;
    }

    public void SetStat(STAT _stat)
    {
        m_CharacterStat = _stat;
    }

    // 버프 종료 후 실행하는 액션 함수 
    public System.Action<Buff> m_BuffExitAction;

    // 버프 액션
    protected abstract void BuffAction();

    // 버프 종료시 처리.
    public abstract void BuffExitAction();

    // 버프 처음 적용시 호출할 함수입니다.
    // 이펙트 재생이나, 무슨 이벤트 플레이용.
    public abstract void PlayBuffEffect(MovingObject _object);

    protected abstract void ExitBuffEffect();

    protected void SetRimLight(MovingObject _object, Color _color)
    {
        m_MovingObject = _object;
        if (m_MovingObject.m_BuffRimLight != null)
        {
            m_MovingObject.m_BuffRimLight.SetRimLight();
            m_MovingObject.m_BuffRimLight.SetColor(_color);

        }
    }

    protected void SetStandard()
    {
        if (m_MovingObject == null) return;
        if (m_MovingObject.m_BuffRimLight != null)
        {
            m_MovingObject.m_BuffRimLight.SetStandard();
        }
    }

    // 그냥 단일 버프
    public IEnumerator OnceCoroutine()
    {
        if (m_CharacterStat == null) yield break;

         BuffAction();
        m_CurTimeDuration -= 1f;
        for (int i=0; i< m_DurationTime; i++)
        {
            m_CurTimeDuration++;
            yield return new WaitForSeconds(1f);
        }
         BuffExitAction();
    }

    // 시간에 맞추어 여러번 버프 주는 액션을 하는 버프
    public IEnumerator TimeTickCorotine()
    {
        while (m_CurTimeDuration < m_DurationTime)
        {
            BuffAction();

            if (m_CharacterStat.isDead)
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

    public float GetTimeRemaining()
    {
        return m_DurationTime - m_CurTimeDuration;
    }
}
public class Adrenaline : Buff
{
     public Adrenaline(STAT _stat) : base(_stat) 
    {
        m_BuffType = BUFF_TYPE.ADRENALINE;
        m_Text = "속도 증가";
        m_ImageName = "Ach45";
        BuffCoroutine = OnceCoroutine();
    }

    public override void PlayBuffEffect(MovingObject _object)
    {
        if (_object == null) return;
        m_EffectObject = EffectManager.Instance.AttachEffect(PARTICLE_TYPE.ADRENALIN, _object, Vector3.up * 1.0f, Quaternion.identity, Vector3.one * 0.6f, true, m_DurationTime - 0.2f);

        SetRimLight(_object, new Color(1f, 1f, 0f));
    }

    protected override void ExitBuffEffect()
    {
        if(m_EffectObject != null && m_EffectObject.gameObject.activeSelf)
            m_EffectObject.pushToMemory();

        m_EffectObject = null;

        SetStandard();
    }

    protected override void BuffAction()
    {
        if (m_CharacterStat == null) return;

        m_CharacterStat.MoveSpeed *= MoveSpeed;
    }

    public override void BuffExitAction()
    {
        ExitBuffEffect();

        m_CharacterStat.MoveSpeed /= MoveSpeed;
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
        m_ImageName = "Ach346";
        BuffCoroutine = TimeTickCorotine();
    }

    public override void PlayBuffEffect(MovingObject _object)
    {
        if (_object == null) return;

        EffectManager.Instance.PlayEffect(PARTICLE_TYPE.BUFF, _object.transform.position, Quaternion.Euler(270, 0, 0),
            Vector3.one * 1.2f, true, 10.0f);

        m_EffectObject = EffectManager.Instance.AttachEffect(PARTICLE_TYPE.HEAL, _object, Vector3.up * 1.0f, Quaternion.identity, Vector3.one * 2.0f, true, m_DurationTime - 0.2f);

        SetRimLight(_object, new Color(0f, 1f, 0f));
    }

    protected override void ExitBuffEffect()
    {
        if (m_EffectObject != null && m_EffectObject.gameObject.activeSelf)
            m_EffectObject.pushToMemory();

        m_EffectObject = null;

        SetStandard();
    }

    protected override void BuffAction()
    {
        if (m_CharacterStat == null) return;
        if (m_CharacterStat.isDead) return;

        m_CharacterStat.CurHP += Attack;
    }
    public override void BuffExitAction()
    {
        ExitBuffEffect();
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
        m_ImageName = "Ach123";
        BuffCoroutine = TimeTickCorotine();
    }
    public override void PlayBuffEffect(MovingObject _object)
    {
        if (_object == null) return;

        m_EffectObject = EffectManager.Instance.AttachEffect(PARTICLE_TYPE.HEAL, _object, Vector3.up * 0.2f, Quaternion.identity, Vector3.one, true, m_DurationTime - 0.2f);
        m_CharacterStat.MoveSpeed *= MoveSpeed;


        SetRimLight(_object, new Color(1f, 0f, 1f));
    }

    protected override void BuffAction()
    {
        if (m_CharacterStat == null) return;
        if (m_CharacterStat.isDead) return;

        m_CharacterStat.CurHP -= Attack;

        Debug.Log("PoisonDamage:" + Attack);
    }

    protected override void ExitBuffEffect()
    {
        if (m_EffectObject != null && m_EffectObject.gameObject.activeSelf)
            m_EffectObject.pushToMemory();

        m_EffectObject = null;

        SetStandard();
    }

    public override void BuffExitAction()
    {
        ExitBuffEffect();

        m_CharacterStat.MoveSpeed /= MoveSpeed;
        m_BuffExitAction(this);
        return;
    }
}

public class Fire : Buff
{
    public Fire(STAT _stat) : base(_stat)
    {
        m_BuffType = BUFF_TYPE.FIRE;
        m_Text = "화상";
        m_ImageName = "Ach85";
        BuffCoroutine = TimeTickCorotine();
    }
    public override void PlayBuffEffect(MovingObject _object)
    {
        if (_object == null) return;

        m_EffectObject = EffectManager.Instance.AttachEffect(PARTICLE_TYPE.FIRE, _object, Vector3.up * 0.3f, Quaternion.identity, Vector3.one * 1.5f, true, m_DurationTime - 0.2f);

        SetRimLight(_object, new Color(1f, 0f, 0f));
    }

    protected override void BuffAction()
    {
        if (m_CharacterStat == null) return;
        if (m_CharacterStat.isDead) return;

        m_CharacterStat.CurHP -= Attack;
    }

    protected override void ExitBuffEffect()
    {
        if (m_EffectObject != null && m_EffectObject.gameObject.activeSelf)
            m_EffectObject.pushToMemory();

        m_EffectObject = null;

        SetStandard();
    }

    public override void BuffExitAction()
    {
        ExitBuffEffect();

        m_BuffExitAction(this);
        return;
    }
}

