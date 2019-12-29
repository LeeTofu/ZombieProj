using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : STAT
{
    protected STAT m_Stat;
    public Buff(STAT _stat)
    {
        m_Stat = _stat;
    }
    public override void Action() { }
}

public class Adrenaline : Buff
{
    public Adrenaline(STAT _stat) : base(_stat) { }
    public override void Action()
    {
        m_Stat.Action();
        MoveSpeedUp();
    }
    public void MoveSpeedUp()
    {
        m_Stat.MoveSpeed += m_Stat.MoveSpeed;
    }
}

public class Blessing : Buff
{
    public Blessing(STAT _stat) : base(_stat)
    {
        m_Coroutine = Healing();
    }
    public override void Action()
    {
        m_Stat.Action();
        //StartCoroutine("Healing");
    }
    IEnumerator Healing()
    {
        int count = 0;
        while (count<10)
        {
            m_Stat.CurHP += 2.0f;
            ++count;
            Debug.Log("Healing:"+m_Stat.CurHP);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

public class Poison : Buff
{
    public Poison(STAT _stat) : base(_stat)
    {
        m_Coroutine = PoisonDamage();
    }
    public override void Action()
    {
        m_Stat.Action();
        //StartCoroutine("PoisonDamage");
    }
    IEnumerator PoisonDamage()
    {
        int count = 0;
        while(count < 10)
        {
            m_Stat.CurHP -= 2.0f;
            ++count;
            Debug.Log("PoisonDamage:" + m_Stat.CurHP);
            yield return new WaitForSeconds(0.1f);
        }
    }
}