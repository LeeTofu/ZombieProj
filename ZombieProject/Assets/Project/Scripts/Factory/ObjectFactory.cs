using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour 
{
    protected GameObject[] m_ModelPrefabs;
    protected GameObject m_MovingObejctPrefabs;

    protected Dictionary<int,Queue<MovingObject>> m_ListSleepingMovingObject = new Dictionary<int,Queue<MovingObject>>();
    public List<MovingObject> m_ListAllMovingObject { get; protected set; }

    // 메모리에 푸시하고서 발생할 이벤트가 있다면 이 액션에 함수를 할당해서 쓰세요.
    // 안써도 무방.
    // 사용예 ) 좀비를 메모리 풀에 다시 넣을때 좀비의 Count를 줄이는 이벤트를 자동으로 실행하고 싶다. 그럼 여기에 등록하셈.
    public System.Action<MovingObject> m_PushMemoryAction;

     public void Initialize( string _prefabPath, GameObject[] _models)
    {
        if(m_ListAllMovingObject == null)
            m_ListAllMovingObject = new List<MovingObject>();

        m_MovingObejctPrefabs = Resources.Load<GameObject>(_prefabPath);
        m_ModelPrefabs = _models;
    }

    private Queue<MovingObject> GetObjectQ(int _keyType)
    {
        Queue<MovingObject> newQ;

        if (m_ListSleepingMovingObject.TryGetValue(_keyType, out newQ))
        {
            return newQ;
        }

        return null;
    }

    private Queue<MovingObject> CreateObjectQ(int _keyType)
    {
        if (m_ListSleepingMovingObject.ContainsKey(_keyType) == true)
        {
            Debug.LogError("풀이 있는데 또 만들라 한다? Back");
            return null;
        }

        Queue<MovingObject> newQ = new Queue<MovingObject>();
        m_ListSleepingMovingObject.Add(_keyType, newQ);

        return newQ;
    }

    public MovingObject PopObject(Vector3 _pos, Quaternion _quat, int _typeKey)
    {
        MovingObject newObject = null;
        Queue<MovingObject> objectQ = GetObjectQ(_typeKey);

        if (objectQ != null)
        {
            newObject = PopObjectFromPooling(_pos, _quat, objectQ);

            if (newObject != null)
            {
                return newObject;
            }
            
        }
        else
        {
            objectQ = CreateObjectQ(_typeKey);
        }

        // 팝을 했는데 이제 더이상 풀에서 활성화할 오브젝트가 없다...? 그럼 새로 오브젝트 만들자.

        newObject = CreateObject();
        newObject.transform.position = _pos;
        newObject.transform.rotation = _quat;

        return newObject;
    }

    private MovingObject CreateObject()
    {
        MovingObject newObject = null;

        GameObject Model = Instantiate(
            m_ModelPrefabs[Random.Range(0, m_ModelPrefabs.Length)]);

        GameObject newGameObject = Instantiate(
            m_MovingObejctPrefabs,
            Vector3.zero,
            Quaternion.identity);

        Model.transform.SetParent(newGameObject.transform);
        Model.transform.localPosition = Vector3.zero;
        Model.transform.localScale = Vector3.one;
        Model.transform.localRotation = Quaternion.identity;

        newObject = newGameObject.GetComponent<MovingObject>();
        newObject.Initialize(Model, null);
        newObject.SetFactory(this);
        newObject.gameObject.SetActive(true);

        newObject.transform.SetParent(transform);

        m_ListAllMovingObject.Add(newObject);

        return newObject;
    }

    public void AllPushToMemoryPool(int _type)
    {
        foreach( MovingObject mobj  in m_ListAllMovingObject)
        {
            mobj.pushToMemory(this, _type);
        }
    }

   // 처음 메모리 풀링 만들때만 실행시켜주세요. 런타임중에 다시 실행할 일 없습니다.
   public void CreateObjectPool(int _typeKey, int _maxCount)
    {
        if (m_ModelPrefabs == null) return;
        if (m_ModelPrefabs.Length == 0) return;
       
        if (m_ListSleepingMovingObject.ContainsKey(_typeKey))
        {
            Debug.LogError("Q가 이미 있는데 만들라 한다? 그러지마세요.");
            return;
        }

        var Q = CreateObjectQ(_typeKey);

        for (int i = 0; i < _maxCount; i++)
        {
            var mobject = CreateObject();
            PushObjectToPooling(mobject, _typeKey);
            mobject.transform.SetParent(transform);
        }
    }

    public void PushObjectToPooling(MovingObject _object, Queue<MovingObject> _Q)
    {
        if (_Q == null) return;

        _object.gameObject.SetActive(false);
        _Q.Enqueue(_object);
    }


    // 다 쓰고 풀링에 넣어줌.
    public void PushObjectToPooling(MovingObject _object, int _typeKey)
    {
        var Q = GetObjectQ(_typeKey);

        if (Q == null)
        {
            Debug.LogError("Q가 없는데 넣을려고 한다..? 확인.. Q를 만들어 오세요.");
            Destroy(_object);
            return;
        }

        _object.gameObject.SetActive(false);
        Q.Enqueue(_object);
    }


    // 풀링에서 오브젝트를 꺼내서 쓴다.
    private MovingObject PopObjectFromPooling(Vector3 _pos, Quaternion _quat, int _typeKey)
    {
        var Q = GetObjectQ(_typeKey);

        if (Q == null)
        {
            Debug.LogError("Q가 없는데 넣을려고 한다..? 확인.. Q를 만들어 오세요.");
            return null;
        }

        if(Q.Count == 0 )
        {
            Debug.LogError("요상하다... 큐에 오브젝트가 없는데.. 여기서 또 꺼내 쓴다고?");
            return null;
        }

        MovingObject obj = Q.Dequeue();
        if (obj == null) return null;
            
        obj.transform.position = _pos;
        obj.transform.rotation = _quat;

        obj.SetFactory(this);
        obj.gameObject.SetActive(true);

        return obj;
    }

    // 풀링에서 오브젝트를 꺼내서 쓴다.
    private MovingObject PopObjectFromPooling(Vector3 _pos, Quaternion _quat, Queue<MovingObject> _Queue)
    {
        if (_Queue == null)
        {
            Debug.LogError("Q가 없는데 넣을려고 한다..? 확인.. Q를 만들어 오세요.");
            return null;
        }

        if (_Queue.Count == 0)
        {
            return null;
        }

        MovingObject obj = _Queue.Dequeue();
        if (obj == null) return null;

        obj.transform.position = _pos;
        obj.transform.rotation = _quat;

        obj.SetFactory(this);
        obj.gameObject.SetActive(true);

        return obj;
    }

}
