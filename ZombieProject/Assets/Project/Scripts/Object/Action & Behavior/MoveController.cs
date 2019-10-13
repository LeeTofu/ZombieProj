using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    // 제어할 캐릭터가 뭔지
    private MovingObject m_Chracter;

    // 현재 실행중인 액션 
    private ObjectAction m_CurrentAction;

    // Action을 table에 저장하기용. 
    private Dictionary<string, ObjectAction> m_ActionTable = new Dictionary<string, ObjectAction>();

    private Queue<ObjectAction> m_ActionQueue = new Queue<ObjectAction>();

    virtual public void Initialize(MovingObject _Character)
    {
        m_Chracter = _Character;

        InsertActionToTable("Idle", gameObject.AddComponent<Idle_ObjectAction>(), false, true);
    }

    public void InsertActionToTable(ObjectAction _action, bool _isOneShot, bool _isPreemptive)
    {
        // Data 
        if (m_ActionTable.ContainsKey((_action.GetType()).Name)) return;

        if (m_CurrentAction == null) m_CurrentAction = _action;

        _action.Initialize(m_Chracter, _action.GetType().Name, _isOneShot, _isPreemptive);
        m_ActionTable.Add(_action.GetType().Name, _action);

        return;
    }

    public void InsertActionToTable(string _actionName, ObjectAction _action, bool _isOneShot, bool _isPreemptive)
    {
        // Data 
        if (m_ActionTable.ContainsKey(_actionName)) return;

        if (m_CurrentAction == null) m_CurrentAction = _action;


        _action.Initialize(m_Chracter, _actionName, _isOneShot, _isPreemptive);
        m_ActionTable.Add(_actionName, _action);

        return;
    }

    public bool IsHaveActionFromTable(string _name)
    {
        return m_ActionTable.ContainsKey(_name);
    }

    public bool IsHaveActionFromTable(ObjectAction _action)
    {
        return m_ActionTable.ContainsKey((_action.GetType()).Name);
    }

    public ObjectAction FindActionFromTable(string _name)
    {
        return m_ActionTable[_name];
    }


    private void Update()
    {
        if (m_CurrentAction == null) return;
        if (!m_CurrentAction.m_isActive) return;
        if (m_CurrentAction.m_isFinish) return;
        if (m_CurrentAction.m_isOneShotPlay) return;

        m_CurrentAction.PlayAction();
    }

    public void PlayAction(string _actionName)
    {
        if (m_CurrentAction.m_ActionName == (_actionName)) return;
        if (!m_CurrentAction.m_isCanPreemptive && !m_CurrentAction.m_isFinish) return;

        ObjectAction action = FindActionFromTable(_actionName);

        if (action != null)
        {
            if (!action.m_isActive) return;

            m_CurrentAction.StopAction();
            m_CurrentAction = action;
            action.PlayAction();
        }
        else
        {
            Debug.Log("can't Find a _actionName Action");
        }
    }

}
