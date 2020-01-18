using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

// 오브젝트 종류
public enum OBJECT_TYPE
{
    PLAYER = 1, // 플레이어
    PLAYER_MERCENARY, // 플레이어 용병
    PLAYER_OBJECT, // 플레이어가 설치한 오브적트

    ZOMBIE, // 좀비
    ELITE_ZOMBIE, // 네임드 좀비
    BOSS_ZOMBIE, // 보스 좀비
    ZOMBIE_OBJECT, // 좀비들 오브젝트

    BULLET, // (불릿)
    DROPITEM, // 드롭된 아이템 ( 체력 회복, 아드레날린, 버프 걸어주는 떨어진 아이템 등등)
    EFFECT // 이펙트
}

public enum ZOMBIE_STATE
{
    IDLE,
    WALK,
    ATTACK,
    RANGE_ATK,
    PATROL,
    CHASE,
    DEAD,
    KNOCK_BACK,
    NONE,
}

public class STAT
{
    private float moveSpeed;
    private float rotSpeed;

    private float attack;
    private float range;
    private float AlertRange;
    private float def;

    private float curHP;
    private float maxHP;

    private float attackSpeed;

    public IEnumerator BuffCoroutine;

    public bool isDead { get; private set; }

    public bool isKnockBack = false;

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

    public float alertRange
    {
        get => AlertRange;
        set
        {
            AlertRange = value;
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

            if(curHP > maxHP )
            {
                curHP = maxHP;
            }

            if (CheckIsDead())
            {
                isDead = true;
            }
            else isDead = false;
            OnPropertyChange?.Invoke();
        }
    }
    public float MaxHP
    {
        get => maxHP;
        set
        {
            curHP = value;
            maxHP = value;
            OnPropertyChange?.Invoke();
        }
    }

    public float AttackSpeed
    {
        get => attackSpeed;
        set
        {
            attackSpeed = value;
            OnPropertyChange?.Invoke();
        }
    }

    // 스탯의 변수에 변화가 생기면 실행한 함수가 있다면 여기에 넣고 쓰자.
    // 예 ) ui 스탯창 변화가 생길때 스탯을 갱신하는 함수를 여따 넣으면 알아서 갱신함.
    public void AddPropertyChangeAction(System.Action _action)
    {
        OnPropertyChange += _action;
    }

    public bool CheckIsDead()
    {
        if(curHP <= 0)
        {
            return true;
        }
        return false;
    }

}



public abstract class MovingObject : MonoBehaviour
{
    private ObjectFactory m_Factory;
    protected Rigidbody m_RigidBody;

    protected GameObject m_Model;

    public STAT m_Stat { protected set; get; }
    public OBJECT_TYPE m_Type;

    [HideInInspector]
    public ZOMBIE_STATE m_zombieState;

    [HideInInspector]
    public Animator m_Animator;

   // [HideInInspector]
   // public ItemObject m_CurrentEquipedItem;

    // 오른팔
    protected Transform m_PropR;
    // 왼팔
    protected Transform m_PropL;

    protected System.Action<float> m_KnockBackAction;
    // 죽은 후 실행하는 함수. // 좀비는 아이템을 떨구고... 플레이어는 게임을 종료하고... 
    protected System.Action m_DeadActionCallBackFunc;

    protected List<Buff> m_ListBuff = new List<Buff>();
    protected List<Buff> m_ListDeBuff = new List<Buff>();

    public Coroutine m_BlinkCoroutine;
    public Coroutine m_KnocoBackCoroutine;

    [HideInInspector]
    public const float m_InjuredHP = 30f;

    protected Renderer[] m_Renderers;

    protected System.Action m_BuffAction;
    protected System.Action m_DeBuffAction;

    // 처음 오브젝트가 팩토리에서 만들어질 때 단 한번만 실행합니다.
    public abstract void Initialize(GameObject _model, MoveController _Controller);

    // 팩토리에서 오브젝트를 꺼내올때 마다 실행합니다.
    public abstract void InGame_Initialize();

    // 충돌 액션이나 조건 처리하는 컴포넌트
    protected CollisionAction m_CollisionAction;

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
    public void pushToMemory(int _type)
    {
        if (!gameObject.activeSelf) return;
        if(m_Factory == null)
        {
            Debug.LogError(gameObject.name + " Factory 없습니다.");
            return;
        }

        gameObject.SetActive(false);
        m_Factory.PushObjectToPooling(this, _type);
    }

    public void pushToMemory(ObjectFactory _factory, int _type)
    {
        if (!gameObject.activeSelf) return;
        if (_factory == null)
        {
            Debug.LogError(gameObject.name + " Factory 없습니다.");
            return;
        }

        gameObject.SetActive(false);
        _factory.PushObjectToPooling(this, _type);
    }


    protected void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        if(m_RigidBody == null)
            m_RigidBody = gameObject.AddComponent<Rigidbody>();

        SetRigidBodyState(false);
        StopRigidbody();

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
    public void SetWeapon(GameObject _itemObject, bool _isRightHand = true)
    {
        Transform PropHand;

        if (_isRightHand)
            PropHand = FindChildObject(gameObject, "Hand_R");
        else PropHand = FindChildObject(gameObject, "Hand_L");

        _itemObject.transform.SetParent(PropHand);
        _itemObject.transform.localPosition = new Vector3(15f, 2.4f, -3.6f);
        _itemObject.transform.localRotation = Quaternion.Euler(-4.336f, 90.0f, -106f);
        _itemObject.transform.localScale = new Vector3(75, 75, 75);

        _itemObject.SetActive(true);
    }

    // ============= 버프는 영래 당담 ===================
    public void AddBuff(Buff _buff)
    {
        if (_buff == null) return;
        m_ListBuff.Add(_buff);
        m_BuffAction?.Invoke();
        Debug.Log(" 버프 갯수 : " + m_ListBuff.Count + " , " + _buff.m_BuffType);

        _buff.m_BuffExitAction = (Buff buff) => { DeleteBuff(buff); };

        StartCoroutine(_buff.BuffCoroutine);
    }

    public void DeleteBuff(Buff _buff)
    {
        if (_buff == null) return;

        StopCoroutine(_buff.BuffCoroutine);

        m_ListBuff.Remove(_buff);
    }

    public void DeleteDeBuff(Buff _buff)
    {
        if (_buff != null) return;

        StopCoroutine(_buff.BuffCoroutine);

        m_ListDeBuff.Remove(_buff);
    }

    public void AddDeBuff(Buff _buff)
    {
        if (_buff != null) return;
        m_ListDeBuff.Add(_buff);
        m_DeBuffAction?.Invoke();
        StartCoroutine(_buff.BuffCoroutine);
    }

    public void AllDeleteBuff()
    {
        foreach(Buff buff in m_ListBuff)
        {
            StopCoroutine(buff.BuffCoroutine);
        }

        foreach (Buff buff in m_ListDeBuff)
        {
            StopCoroutine(buff.BuffCoroutine);
        }

        m_ListBuff.Clear();
        m_ListDeBuff.Clear();

    }
    //==============================================

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


    // 죽음시 처리할 액션이다
    public void DeadAction()
    {
        if (m_Stat == null) return;

        if(m_Stat.isDead)
        {

            m_CollisionAction.SetCollisionActive(false);
            //걸린 모든 버프 제거하고
            AllDeleteBuff();
            m_DeadActionCallBackFunc?.Invoke();
        }
    }

    //데미지를 입다.
    public void HitDamage(float _damage, bool _isKnockBack = false, float _knockBackTime = 0.0f)
    {
        m_Stat.CurHP -= _damage;

        if (_isKnockBack)
            m_KnockBackAction?.Invoke(_knockBackTime);
    }
    public void AddKnockBackFunction(System.Action<float> _knockBackAction)
    {
        m_KnockBackAction += _knockBackAction;
    }
    public void AddBuffFunction(System.Action _buffAction)
    {
        m_BuffAction += _buffAction;
    }
    public void AddDeBuffFunction(System.Action _deBuffAction)
    {
        m_DeBuffAction += _deBuffAction;
    }
    public List<Buff> GetListBuff()
    {
        return m_ListBuff;
    }
    public List<Buff> GetListDeBuff()
    {
        return m_ListDeBuff;
    }
    // 넉백액션함수에 깜빡이는 코루틴이랑 넉백당한 동안 isKnockBack을 true로 해주는 코루틴 추가하는 함수
    public void AddKnockBackAction(float _blinkterm)
    {
        AddKnockBackFunction((float time) =>
        {
            if (m_BlinkCoroutine != null)
                StopCoroutine(m_BlinkCoroutine);
            if (m_KnocoBackCoroutine != null)
                StopCoroutine(m_KnocoBackCoroutine);

            m_BlinkCoroutine = StartCoroutine(Blink(time, _blinkterm));
            m_KnocoBackCoroutine = StartCoroutine(KnockBackRelease(time));
        });
    }
    //Renderer가 있는 게임오브젝트들을 true,false 줘서 깜빡이게하는 코루틴
    public IEnumerator Blink(float _time, float _blinkterm)
    {
        yield return null;

        //for (float i = 0; i < _time; i += _blinkterm)
        //{
        //    for (int j = 0; j < m_Renderers.Length; ++j)
        //        m_Renderers[j].enabled = !m_Renderers[j].enabled;
        //    yield return new WaitForSeconds(_blinkterm);
        //}
        //for (int i = 0; i < m_Renderers.Length; ++i)
        //    m_Renderers[i].enabled = true;
    }
    //_time 받은 만큼 m_Stated의 isKnockBack을 true해준다
    public IEnumerator KnockBackRelease(float _time)
    {
        m_Stat.isKnockBack = true;
        yield return new WaitForSeconds(_time);
        m_Stat.isKnockBack = false;
    }
}
