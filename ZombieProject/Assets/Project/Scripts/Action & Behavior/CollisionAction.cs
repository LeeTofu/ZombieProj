using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool CheckCollisionCondition(GameObject _collider);

public abstract class CollisionAction : MonoBehaviour
{
    System.Action<GameObject> m_CollisionAction;
    System.Action<GameObject> m_CollisionExitAction;

    CheckCollisionCondition m_CheckCollisionCondition;

    public MovingObject m_Character;

    protected CapsuleCollider m_CapsuleCollider;

    // ------------- 충돌 테스트용으로 만든 임시 함수들임 ----------------- 
    // 충돌 이벤트에 함수 등록해서 쓰는거임.

    // 충돌에 대한 조건을 등록할 때 쓰는 함수입니다.
    // 여기에 bool 반환하는 조건 함수를 만드셔서 등록하세요.
    // 여러개 함수를 넣으면 그 조건 다 따져요. And 연산.

    protected void Awake()
    {
        m_CapsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        if (m_CapsuleCollider == null)
            m_CapsuleCollider = gameObject.AddComponent<CapsuleCollider>();

        if (m_Character == null)
            m_Character = GetComponent<MovingObject>();

        AddCollisionFunction(CollisionEvent);
        AddCollisionCondtion(CollisionCondition);
    }


    protected abstract void CollisionEvent(GameObject _object);
    protected abstract bool CollisionCondition(GameObject _defender);

    public void AddCollisionCondtion(CheckCollisionCondition _collisionCondition)
    {
        m_CheckCollisionCondition += _collisionCondition;
    }

    // 충돌 후에 일어날 이벤트에 대한 함수를 등록하는 함수입니다.
    // 여기에 void 형 함수에 매개변수는 GameObject ( 충돌한 오브젝트 )한 함수를 만들어서 쓰세요.
    // 매개변수가 더 필요하다면 따로 하나 이런 함수 만드셈.
    public void AddCollisionFunction(System.Action<GameObject> _collisionAction)
    {
        m_CollisionAction += _collisionAction;
    }


    public void AddCollisionExitFunction(System.Action<GameObject> _collisionExitAction)
    {
        if (m_CollisionExitAction == null)
        {
            m_CollisionExitAction = _collisionExitAction;
            return;
        }

        m_CollisionExitAction += _collisionExitAction;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (m_CollisionAction == null) return;

        if (m_CheckCollisionCondition == null)
            m_CollisionAction(collision.gameObject);
        else if (m_CheckCollisionCondition(collision.gameObject))
            m_CollisionAction(collision.gameObject);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (m_CollisionAction == null) return;

        if (m_CheckCollisionCondition == null)
            m_CollisionAction(other.gameObject);
        else if (m_CheckCollisionCondition(other.gameObject))
            m_CollisionAction(other.gameObject);
    }

    protected void OnTriggerExit(Collider other)
    {
        if (m_CollisionExitAction == null) return;

        m_CollisionExitAction(other.gameObject);
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (m_CollisionExitAction == null) return;

        m_CollisionExitAction(collision.gameObject);
    }

    public void SetCollisionActive(bool _active)
    {
        m_CapsuleCollider.enabled = _active;
    }

}
