using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    protected GameObject[] m_ModelPrefabs;
    protected GameObject m_MovingObejctPrefabs;

    bool m_Initialize = false;

    protected int m_MaxCount;
    protected Queue<MovingObject> m_ListSleepingMovingObject = new Queue<MovingObject>();

     public void Initialize(int _maxCount, string _prefabPath, string _modelsPath)
    {
        m_MaxCount = _maxCount;
        m_MovingObejctPrefabs = Resources.Load<GameObject>(_prefabPath);
        m_ModelPrefabs = Resources.LoadAll<GameObject>(_modelsPath);

        if (m_MovingObejctPrefabs)
        {
            Debug.Log(" Object Load Success");
        }

        if (m_ModelPrefabs.Length != 0)
        {
            Debug.Log(" Model Load Success");
        }

        CreateObjectPool();
    }

    public virtual MovingObject CreateObject(Vector3 _pos, Quaternion _quat)
    {
        MovingObject newObject = null;

        if (m_ListSleepingMovingObject.Count > 0 && m_Initialize)
        {
            newObject = PopObjectFromPooling(_pos, _quat);
        }

        if (newObject != null)
        {
            newObject.gameObject.SetActive(true);
            return newObject;
        }

        GameObject Model = Instantiate(
            m_ModelPrefabs[Random.Range(0, m_ModelPrefabs.Length)],
            _pos,
            _quat);

        GameObject newGameObject = Instantiate(
            m_MovingObejctPrefabs,
            _pos,
            /*m_ZombieCreateZone.transform.position + Vector3.forward * Random.Range(-5.5f, 5.5f)*/
            _quat);

        Model.transform.SetParent(newGameObject.transform);
        Model.transform.localPosition = Vector3.zero;
        Model.transform.localRotation = Quaternion.identity;
        Model.transform.localScale = Vector3.one;

        newObject = newGameObject.GetComponent<MovingObject>();
        newObject.Initialize(Model, null);
        newObject.SetFactory(this);
        newObject.gameObject.SetActive(true);

        PushObjectToPooling(newObject);

        return newObject;
    }


   // 처음에만 실행시켜주세요.
   protected void CreateObjectPool()
    {
        if (m_ListSleepingMovingObject.Count >= m_MaxCount) return;

        for(int i = 0; i < m_MaxCount; i++)
        {
            var mobject = CreateObject(Vector3.zero, Quaternion.identity);
            mobject.transform.SetParent(transform);
            mobject.gameObject.SetActive(false);
        }
        m_Initialize = true;
    }


    // 다 쓰고 풀링에 넣어줌.
    public void PushObjectToPooling(MovingObject _object)
    {
        m_ListSleepingMovingObject.Enqueue(_object);
    }


    // 풀링에서 오브젝트를 꺼내서 쓴다.
    public MovingObject PopObjectFromPooling(Vector3 _pos, Quaternion _quat)
    {
        if (m_ListSleepingMovingObject.Count == 0) return null;

        MovingObject obj = m_ListSleepingMovingObject.Dequeue();
        if (obj == null) return null;
            
        obj.transform.position = _pos;
        obj.transform.rotation = _quat;

        obj.SetFactory(this);
        obj.gameObject.SetActive(true);

        return obj;
    }

}
