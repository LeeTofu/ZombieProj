using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

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

public enum ZOMBIE_STATE
{
    IDLE,
    WALK,
    ATTACK,
    RANGE_ATK,
    PATROL,
    CHASE,
    NONE,
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
    protected AimIK m_AimIK;

    private ObjectFactory m_Factory;
    protected Rigidbody m_RigidBody;
    protected CapsuleCollider m_CapsuleCollider;
    
    protected GameObject m_Model;

    public STAT m_Stat;
    public OBEJCT_SORT m_Sort;
    public ZOMBIE_STATE m_zombieState;
    public Animator m_Animator;
    public ItemObject m_CurrentEquipedItem;

    // 오른팔
    protected Transform m_PropR;
    // 왼팔
    protected Transform m_PropL;

    System.Action<GameObject> m_CollisionAction;
    System.Action<GameObject> m_CollisionExitAction;

    protected List<Buff> m_ListBuff = new List<Buff>();
    protected List<Buff> m_ListDeBuff = new List<Buff>();

    CheckCollisionCondition m_CheckCollisionCondition;
    public abstract void Initialize(GameObject _model, MoveController _Controller);
    public virtual void SetStat(STAT _stat)
    {
        m_Stat = _stat;
    }


    Transform FindChildObject(GameObject _baseObject, string _str)
    {
        Transform[] objects = _baseObject.GetComponentsInChildren<Transform>();
        foreach (Transform obj in objects)
        {
            if (obj.name == _str)
                return obj;
        }

        return null;
    }

    // Factory에서 만들어진 MovingObject는 자동으로 오브젝트 풀링해서 쓴당.
    // 오브젝트 다 쓰고 나면 pushToMemory 필수로 실행해주세요.
    public void pushToMemory()
    {
        if (!gameObject.activeSelf) return;
        if(m_Factory == null)
        {
            Debug.LogError(gameObject.name + " Factory 없습니다.");
            return;
        }

        gameObject.SetActive(false);
        m_Factory.PushObjectToPooling(this);
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
    // 충돌 이벤트에 함수 등록해서 쓰는거임.

    public void AddCollisionCondtion(CheckCollisionCondition _collisionCondition)
    {
        m_CheckCollisionCondition += _collisionCondition;
    }


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
    // --------------------------------------------------------------------
    // 내 앞에 벽이 있나? 
    public bool CheckForwardWall(Vector3 _forward, out RaycastHit _hit, out Vector3 _center)
    {
        _center = this.transform.position + this.transform.TransformDirection(this.transform.GetComponentInChildren<CapsuleCollider>().center);
        Ray ray = new Ray(_center, this.transform.TransformDirection(Vector3.forward));

        Debug.DrawRay(ray.origin, ray.direction * 2, Color.red);

        if (Physics.Raycast(ray, out _hit, 1.0f, 1 << LayerMask.NameToLayer("Wall")))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 무빙오브젝트의 무기를 셋팅하는 함수임.
    public void SetWeapon(Item _item, bool _isRightHand = true)
    {
        if(m_CurrentEquipedItem != null)
        {
            m_CurrentEquipedItem.gameObject.SetActive(false);
        }
        if (_item == null) return;

        GameObject itemObject = ItemManager.Instance.CreateItemObject(_item);

        if (itemObject == null) return;

        Transform PropHand;

        if (_isRightHand)
            PropHand = FindChildObject(gameObject, "Hand_R");
        else PropHand = FindChildObject(gameObject, "Hand_L");

        itemObject.transform.SetParent(PropHand);
        itemObject.transform.localPosition = new Vector3(15f, 2.4f, -3.6f);
        itemObject.transform.localRotation = Quaternion.Euler(-4.336f, 90.0f, -106f);
        itemObject.transform.localScale = new Vector3(75, 75, 75);

        m_CurrentEquipedItem = itemObject.GetComponent<ItemObject>();
        m_CurrentEquipedItem.Init(_item);

    }

    // ============= 버프는 영래 당담 ===================
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

    // 팩토리 설정
    public void SetFactory(ObjectFactory _factory)
    {
        m_Factory = _factory;
    }

    public void SetAimIK(GameObject _object)
    {
      //  if (m_AimIK == null)
      //     m_AimIK = GetComponentInChildren<AimIK>();

       // m_AimIK.solver.target = _object.transform;
    }






}
