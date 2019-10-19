using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectFactory : MonoBehaviour
{
    abstract public void Initialize();

    abstract public MovingObject CreateObject(Vector3 _pos, Quaternion _quat);
    
}
