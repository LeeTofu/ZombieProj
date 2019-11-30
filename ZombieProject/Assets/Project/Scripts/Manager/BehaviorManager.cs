using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorManger : Singleton<BehaviorManger>
{
    private Dictionary<string, BehaviorNode> m_BehaviorTable = new Dictionary<string, BehaviorNode>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AddActionNode(string key, BehaviorNode head)
    {
        if (m_BehaviorTable.ContainsKey(key)) return;

        m_BehaviorTable.Add(key, head);
    }

    public BehaviorNode GetNode(string key)
    {
        if(m_BehaviorTable.ContainsKey(key))
        {
            return m_BehaviorTable[key];
        }

        return null;
    }

    public override bool Initialize()
    {
        return true;
    }
}
