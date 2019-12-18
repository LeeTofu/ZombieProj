using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 오브젝트 종류
public enum OBEJCT_SORT
{
    PLAYER = 1, // 플레이어
    PLAYER_MERCENARY, // 플레이어 용병
    PLAYER_OBJECT, // 플레이어가 설치한 오브적트

    ZOMBIE, // 좀비
    ELITE_ZOMBIE, // 네임드 좀비
    BOSS_ZOMBIE, // 보스 좀비
    ZOMBIE_OBJECT // 좀비들 오브젝트
}

public class STAT
{
    private float moveSpeed;
    private float rotSpeed;

    private float attack;
    private float range;
    private float def;

    private float curHP;
    private float maxHP;

    System.Action OnPropertyChange;

    public float MoveSpeed
    {
        get => moveSpeed;
        set
        {
            moveSpeed = value;
            OnPropertyChange?.Invoke();
        }
    }
    public float RotSpeed
    {
        get => rotSpeed;
        set
        {
            rotSpeed = value;
            OnPropertyChange?.Invoke();
        }
    }
    public float Attack
    {
        get => attack;
        set
        {
            attack = value;
            OnPropertyChange?.Invoke();
        }
    }
    public float Range
    {
        get => range;
        set
        {
            range = value;
            OnPropertyChange?.Invoke();
        }
    }
    public float Defend
    {
        get => def;
        set
        {
            def = value;
            OnPropertyChange?.Invoke();
        }
    }
    public float CurHP
    {
        get => curHP;
        set
        {
            curHP = value;
            OnPropertyChange?.Invoke();
        }
    }
    public float MaxHP
    {
        get => maxHP;
        set
        {
            maxHP = value;
            OnPropertyChange?.Invoke();
        }
    }

    public void AddPropertyChangeAction(System.Action _action)
    {
        OnPropertyChange += _action;
    }
}


public delegate bool CheckCollisionCondition(GameObject _collider);

public abstract class MovingObject : MonoBehaviour
{
    private ObjectFactory m_Factory;
    protected Rigidbody m_RigidBody;
    protected CapsuleCollider m_CapsuleCollider;
    
    protected GameObject m_Model;

    public STAT m_Stat;
    public OBEJCT_SORT m_Sort;
    public Animator m_Animator;
    public ItemObject m_CurrentItemObject;

    // 오른팔
    protected Transform m_PropR;
    // 왼팔
    protected Transform m_PropL;

    System.Action<GameObject> m_CollisionAction;

    protected List<Buff> m_ListBuff = new List<Buff>();
    protected List<Buff> m_ListDeBuff = new List<Buff>();

    CheckCollisionCondition m_CheckCollisionCondition;
    public abstract void Initialize(GameObject _model, MoveController _Controller);
    public virtual void SetStat(STAT _stat)
    {
        m_Stat = _stat;
    }

    public void pushToMemory()
    {
        if (!gameObject.activeSelf) return;
        if(m_Factory == null)
        {
            Debug.LogError(gameObject.name + " Factory 없습니다.");
            return;
        }

        gameObject.SetActive(false);
        m_Factory.PushObject(this);
    }


    protected void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        if(m_RigidBody == null)
            m_RigidBody = gameObject.AddComponent<Rigidbody>();

        m_CapsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        if (m_CapsuleCollider == null)
            m_CapsuleCollider = gameObject.AddComponent<CapsuleCollider>();


        SetRigidBodyState(false);
        StopRigidbody();

        m_CollisionAction += (GameObject obj) => { };
    }

    // ------------- 충돌 테스트용으로 만든 임시 함수들임 ----------------- 
    public void AddCollisionCondtion(CheckCollisionCondition _collisionCondition)
    {
        m_CheckCollisionCondition += _collisionCondition;
    }


    public void AddCollisionFunction(System.Action<GameObject> _collisionAction)
    {
        m_CollisionAction += _collisionAction;
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
    // --------------------------------------------------------------------

    public void SetWeapon(Item _item, bool _isRightHand = true)
    {
        if(m_CurrentItemObject != null)
        {
            m_CurrentItemObject.gameObject.SetActive(false);
        }
        if (_item == null) return;

        GameObject itemObject = ItemManager.Instance.CreateItemObject(_item);

        itemObject.transform.SetParent(transform);
        itemObject.transform.localPosition = Vector3.zero;
        itemObject.transform.localRotation = Quaternion.identity;

        m_CurrentItemObject = itemObject.GetComponent<ItemObject>();

    }

    void AddBuff(Buff _buff)
    {
        if (!_buff) return;
        m_ListBuff.Add(_buff);
    }

    void AddDeBuff(Buff _buff)
    {
        if (!_buff) return;
        m_ListDeBuff.Add(_buff);
    }

    public void SetRigidBodyState(bool _value)
    {
        if (m_RigidBody == null)
        {
            Debug.LogError("RigidBody 읍다.");
            return;
        }
        m_RigidBody.isKinematic = !_value;
        m_RigidBody.useGravity = _value;

    }


    public void AddForce(Vector3 _velocity, Vector3 _angularVelocity )
    {
        m_RigidBody.velocity = _velocity;
        m_RigidBody.angularVelocity = _angularVelocity;
    }

    public void StopRigidbody()
    {
        AddForce(Vector3.zero, Vector3.zero);
    }

    public void SetFactory(ObjectFactory _factory)
    {
        m_Factory = _factory;
    }


}
