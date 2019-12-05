using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectFactory : MonoBehaviour
{

    protected int m_MaxCount;
    protected List<MovingObject> m_ListSleepingMovingObject = new List<MovingObject>();

    abstract public void Initialize(int _maxCount);

    abstract public MovingObject CreateObject(Vector3 _pos, Quaternion _quat);

    public void PushObject(MovingObject _object)
    {
        m_ListSleepingMovingObject.Add(_object);
    }

    public MovingObject PopObject(Vector3 _pos, Quaternion _quat)
    {
        Debug.Log("Zombie ReCreate");

        for (int i = 0; i < m_ListSleepingMovingObject.Count; i++)
        {
            MovingObject obj = m_ListSleepingMovingObject[i];

            obj.Initialize(null, null);
            m_ListSleepingMovingObject.RemoveAt(i);
            return obj;
        }

        return null;
    }

}
