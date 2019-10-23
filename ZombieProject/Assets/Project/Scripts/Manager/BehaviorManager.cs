using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorManger : Singleton<BehaviorManger>
{
    private Dictionary<string, ActionNode> m_ActionTable = new Dictionary<string, ActionNode>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AddActionNode(string key, ActionNode head)
    {
        if (m_ActionTable.ContainsKey(key)) return;

        m_ActionTable.Add(key, head);
    }

    public ActionNode GetNode(string key)
    {
        if(m_ActionTable.ContainsKey(key))
        {
            return m_ActionTable[key];
        }

        return null;
    }

    public override bool Initialize()
    {
        return true;
    }
}
