using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : MonoBehaviour
{
    public string m_ActionName { get; private set; }
    public bool m_isActive { get; protected set; } // Action을 쓸 수 있는가 true = 사용가능, false = 불가능

    public delegate void FunctionPointer();

    protected MovingObject m_Character;
    protected Animator m_Animation;

    private FunctionPointer m_playAction;
    private FunctionPointer m_stopAction;

    virtual public bool OnUpdate()
    {
        return true;
        // 오버라이드해서 행동 구현 //
    }

    virtual public bool OnStart()
    {
        return true;
        // 오버라이드해서 행동 구현 //
    }

    virtual public bool OnEnd()
    {
        return true;
        // 오버라이드해서 행동 구현 //
    }

    public void Initialize(MovingObject _character, string  _action)
    {
        m_isActive = true;
        m_ActionName = _action;
        m_Character = _character;
        m_Animation = m_Character.GetComponentInChildren<Animator>();

        if(m_Animation == null)
        {
            Debug.Log("Chracter not have a Animation!");
        }
    }

    public virtual bool CheckStartCondition()
    { return true; }

    public virtual bool CheckUpdateCondition()
    { return true; }

    public virtual bool CheckStopCondition()
    { return true; }

}

public abstract class CompositeActionNode : ActionNode
{
    protected CompositeActionNode m_CurrentUpdateAction;
    protected List<ActionNode> m_ActionList = new List<ActionNode>();

    public void InsertAction(ActionNode _action)
    {
        m_ActionList.Add(_action);
    }
}

public class SelectorNode : CompositeActionNode
{
    public override bool OnUpdate()
    {
        for (int i = 0; i < m_ActionList.Count; i++ )
        {
            if (m_ActionList[i].OnUpdate())
            {
                return true;
            }
        }

        return false;
    }
}

public class SequencerNode : CompositeActionNode
{
    public override bool OnUpdate()
    {
        for (int i = 0; i < m_ActionList.Count; i++)
        {
            if (!m_ActionList[i].OnUpdate())
            {
                return false;
            }
        }

        return true;
    }
}

