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

    public void InsertActionToTable(ObjectAction _action)
    {
        // Data 
        if (m_ActionTable.ContainsKey((_action.GetType()).Name)) return;

        _action.Initialize(m_Chracter);
        m_ActionTable.Add(_action.GetType().Name, _action);

        return;
    }

    public void InsertActionToTable(string _actionName, ObjectAction _action)
    {
        // Data 
        if (m_ActionTable.ContainsKey(_actionName)) return;

        _action.Initialize(m_Chracter);
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

    virtual public void Initialize(MovingObject _Character)
    {
        m_Chracter = _Character;



    }

    private void Update()
    {
        if (m_CurrentAction == null) return;

        m_CurrentAction.PlayAction();
    }

    public void WalkAction()
    {
        ObjectAction action = FindActionFromTable("Walk");

        if (action != null)
        {
            m_CurrentAction = action;
            action.PlayAction();
        }
        else
        {
            Debug.Log("can't Find a Walk Action");
        }
    }

    // RayHit후 행동 처리
    public void RayHitAction()
    {
        ObjectAction action = FindActionFromTable("RayHit");
        if(action != null)
        {
            m_CurrentAction = action;
            action.PlayAction();
        }
        else
        {
            Debug.Log("can't Find a RayHit Action");
        }
    }

    // 실제 충돌 행동의 행동 처리
    public void CollisionAction()
    {
        ObjectAction action = FindActionFromTable("CollisionHit");
        if (action != null)
        {
            m_CurrentAction = action;
            action.PlayAction();
        }
        else
        {
            Debug.Log("can't Find a Collision Action");
        }
    }

}
