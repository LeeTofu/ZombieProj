using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 전역 접근 가능한 매니저에 쓸 때 상속받아서 쓰시면 됨
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static private T instance = null;

    static public T Instance
    {
        get
        {

            GameObject go;
            if(instance == null)
            {
                 go = GameObject.Find(typeof(T).Name);
                Debug.Log(typeof(T).Name + " Finding...");

                if (go == null)
                {
                    go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();

                    SetUp();

                    return instance;
                }
                else
                {
                    instance = go.AddComponent<T>();
                    SetUp();
                    return instance;
                }
            }
             return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    abstract protected void Initialize();

    private static bool SetUp()
    {
        if (instance == null)
        {
            Debug.Log(typeof(T).Name + "Load Fail");
            return false;
        }

        Debug.Log(typeof(T).Name + "Load Success");


        return true;
    }


}
