using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NODE_STATE
{
    SUCCESS,
    FAIL,
    RUNNING
}

public abstract class BehaviorNode
{
    public string m_NodeName { get; private set; }
    protected NODE_STATE m_NodeState { get; set; }
    //protected MovingObject m_Character { get; set; }

    protected MovingObject m_Character;

    protected BehaviorNode m_HeadNode;

    virtual public void Initialize(MovingObject _character)
    {
        m_Character = _character;
    }

    virtual public NODE_STATE Tick()
    {
        return NODE_STATE.SUCCESS;
    }

    protected void SetHeadNode(BehaviorNode _node)
    {
        m_HeadNode = _node;
    }

    protected float GetAttackObjectDistance()
    {
        float distance = 0.0f;

        MovingObject mobject = GetAttackObject();

        if (mobject == null) return 10000.0f;
        if (mobject.m_Stat == null) return 10000.0f;
        if (mobject.m_Stat.isDead) return 10000.0f;
        if (!mobject.gameObject.activeSelf) return 10000.0f;

        distance = Vector3.Distance(
            m_Character.transform.position,
           mobject.transform.position);

        return distance;
    }

    protected MovingObject GetAttackObject()
    {
        if (m_Character.m_TargetingObject != null )
        {
            if(m_Character.m_TargetingObject.gameObject.activeSelf)
                return m_Character.m_TargetingObject;
        }
        if (EnemyManager.Instance.m_ZombieAttackObject)
        {
            if (EnemyManager.Instance.m_ZombieAttackObject.gameObject.activeSelf)
                return EnemyManager.Instance.m_ZombieAttackObject;
        }

        return PlayerManager.Instance.m_Player;
        
    }
}

public abstract class CompositeNode : BehaviorNode
{
    protected BehaviorNode m_CurrentUpdateAction;
    protected List<BehaviorNode> m_NodeList = new List<BehaviorNode>();

    public void InsertAction(BehaviorNode _behaviorNode)
    {
        m_NodeList.Add(_behaviorNode);
    }

    public override void Initialize(MovingObject _character)
    {
        for (int i = 0; i < m_NodeList.Count; i++)
        {
            m_NodeList[i].Initialize(_character);
        }
    }
}

public class SelectorNode : CompositeNode
{
    public override NODE_STATE Tick()
    {
        for (int i = 0; i < m_NodeList.Count; i++)
        {
            switch (m_NodeList[i].Tick())
            {
                case NODE_STATE.SUCCESS:
                    m_NodeState = NODE_STATE.SUCCESS;
                    return m_NodeState;
                case NODE_STATE.RUNNING:
                    m_NodeState = NODE_STATE.SUCCESS;
                    return m_NodeState;
                case NODE_STATE.FAIL:
                    continue;
            }
        }
        m_NodeState = NODE_STATE.FAIL;
        return m_NodeState;
    }

}

public class SequenceNode : CompositeNode
{
    public override NODE_STATE Tick()
    {
        bool isAnyChildRunning = false;

        for (int i = 0; i < m_NodeList.Count; i++)
        {
            switch (m_NodeList[i].Tick())
            {
                case NODE_STATE.SUCCESS:
                    continue;
                case NODE_STATE.RUNNING:
                    isAnyChildRunning = true;
                    continue;
                case NODE_STATE.FAIL:
                    m_NodeState = NODE_STATE.FAIL;
                    return m_NodeState;
            }
        }

        m_NodeState = isAnyChildRunning ? NODE_STATE.RUNNING : NODE_STATE.SUCCESS;
        return m_NodeState;
    }
}

public abstract class DecoratorNode : BehaviorNode
{
 



    //반복, 실행조건(확률 등)추가
}

public abstract class ActionNode : BehaviorNode
{
    //public delegate void FunctionPointer();
    //protected FunctionPointer m_playAction;
    protected float m_totalActionTime = 0f;
    protected float m_nowActionTime = 0f;

}