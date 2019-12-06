using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFactory : ObjectFactory
{
  

    public override void Initialize(int _maxCount)
    {
        m_MaxCount = _maxCount;
        m_MovingObejctPrefabs = Resources.Load<GameObject>("Prefabs/Players/Player");
        m_ModelPrefabs = Resources.LoadAll<GameObject>("Prefabs/Players/Models/Normal");

        if (m_MovingObejctPrefabs)
        {
            Debug.Log("Player Object Load Success");
        }

        if (m_ModelPrefabs.Length != 0)
        {
            Debug.Log("Player Model Load Success");
        }

       CreateObjectPool();
    }
}
