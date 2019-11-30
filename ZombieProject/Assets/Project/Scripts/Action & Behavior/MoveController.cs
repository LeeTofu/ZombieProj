using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    // 제어할 캐릭터가 뭔지
    private MovingObject m_Chracter;

    // 현재 실행중인 액션 
    private BehaviorNode m_CurrentBehavior;

    // Action을 table에 저장하기용. -> BehaviorManager로 이동
    /*
    private Dictionary<string, ActionNode> m_ActionTable = new Dictionary<string, ActionNode>();

    private Queue<ActionNode> m_ActionQueue = new Queue<ActionNode>();
    */

    virtual public void Initialize(MovingObject _Character)
    {
        m_Chracter = _Character;

        //InsertActionToTable("Idle", gameObject.AddComponent<Idle_ObjectAction>());
    }

    /*
    public void InsertActionToTable(ActionNode _action)
    {
        // Data 
        if (m_ActionTable.ContainsKey((_action.GetType()).Name)) return;

        if (m_CurrentAction == null) m_CurrentAction = _action;

        _action.Initialize(m_Chracter, _action.GetType().Name);
        m_ActionTable.Add(_action.GetType().Name, _action);

        return;
    }

    public void InsertActionToTable(string _actionName, ActionNode _action)
    {
        // Data 
        if (m_ActionTable.ContainsKey(_actionName)) return;

        if (m_CurrentAction == null) m_CurrentAction = _action;


        _action.Initialize(m_Chracter, _actionName);
        m_ActionTable.Add(_actionName, _action);

        return;
    }

    public bool IsHaveActionFromTable(string _name)
    {
        return m_ActionTable.ContainsKey(_name);
    }

    public bool IsHaveActionFromTable(ActionNode _action)
    {
        return m_ActionTable.ContainsKey((_action.GetType()).Name);
    }

    public ActionNode FindActionFromTable(string _name)
    {
        return m_ActionTable[_name];
    }

    public void PlayAction(string _actionName)
    {
        if (!m_ActionTable.ContainsKey(_actionName)) return;

        if (m_CurrentAction != null)
            m_CurrentAction.OnEnd();

        Debug.Log(_actionName + "실행");
        m_CurrentAction = m_ActionTable[_actionName];
    }
    */

    private void Update()
    {
        if (m_CurrentBehavior == null) return;

        m_CurrentBehavior.Tick();
    }

}
