using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;
using UnityEngine.AI;

// 오브젝트 종류
public enum OBJECT_TYPE
{
    PLAYER = 1, // 플레이어
    PLAYER_MERCENARY, // 플레이어 용병
    PLAYER_OBJECT, // 플레이어가 설치한 오브적트

    ZOMBIE, // 좀비
    DASH_ZOMBIE,
    RANGE_ZOMBIE,
    BOMB_ZOMBIE, // 네임드 좀비


    BULLET, // (불릿)
    BUFF_OBJECT, // 버프주는 오브젝트 ( 체력 회복, 아드레날린, 버프 걸어주는 떨어진 아이템 등등)
    EFFECT, // 이펙트

    NONE
}

public enum ZOMBIE_STATE
{
    IDLE,
    WALK,
    ATTACK,
    RANGE_ATK,
    PATROL,
    CHASE,
    PATHFIND,
    DEAD,
    KNOCK_BACK,
    STUN,
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

    public bool isStunned = false;

    public bool isCheckDeadAction = false;

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
                curHP = 0;
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
        if(curHP <= 0.01f)
        {
            return true;
        }
        return false;
    }

    public STAT Clone()
    {
        STAT newStat = new STAT();
        newStat.attack = attack;
        newStat.def = def;
        newStat.attackSpeed = attackSpeed;
        newStat.curHP = curHP;
        newStat.maxHP = maxHP;
        newStat.OnPropertyChange = OnPropertyChange;
        newStat.alertRange = alertRange;
        newStat.range = range;
        newStat.moveSpeed = moveSpeed;
        newStat.isStunned = isStunned;
        newStat.isKnockBack = isKnockBack;
        newStat.isDead = isDead;
        newStat.rotSpeed = rotSpeed;

        return newStat;
    }

}



public abstract class MovingObject : MonoBehaviour
{
    private ObjectFactory m_Factory;
    protected Rigidbody m_RigidBody;

    protected GameObject m_Model;

    public STAT m_Stat { protected set; get; }
    public OBJECT_TYPE m_Type;

    // 이거 중요!!! 오브젝트 풀에 다시 집어넣을때 m_TypeKey로 집어넣는거임
    public int m_TypeKey;

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

    public Coroutine m_KnocoBackCoroutine;

    [HideInInspector]
    public const float m_InjuredHP = 30f;

    protected Renderer[] m_Renderers;

    protected System.Action<List<Buff> > m_BuffAction;
    protected System.Action<List<Buff> > m_DeBuffAction;

    // 처음 오브젝트가 팩토리에서 만들어질 때 단 한번만 실행합니다.
    public abstract void Initialize(GameObject _model, MoveController _Controller, int _TypeKey);

    // 팩토리에서 오브젝트를 꺼내올때 마다 실행합니다.
    public abstract void InGame_Initialize();

    // 충돌 액션이나 조건 처리하는 컴포넌트
    protected CollisionAction m_CollisionAction;

    // 어그 끌리는 오브젝트
    public MovingObject m_TargetingObject { protected set; get; }

    //좀비들 길찾기용 NavMeshAgent
    public NavMeshAgent m_NavAgent;

    public HpBarUI m_HpBarUI;

    public BuffRimLight m_BuffRimLight;

    protected Coroutine m_DeadCoroutine;

    public DrawRange m_DrawRender { get; set; }

    public virtual void SetStat(STAT _stat)
    {
        m_Stat = _stat;
    }

    public void SetTarget(MovingObject _object)
    {
        m_TargetingObject = _object;
    }

    public Transform FindChildObject(GameObject _baseObject, string _str)
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

        m_TargetingObject = null;
        gameObject.SetActive(false);
        m_Factory.PushObjectToPooling(this, m_TypeKey);
    }

    public void pushToMemory(ObjectFactory _factory)
    {
        if (!gameObject.activeSelf) return;
        if (_factory == null)
        {
            Debug.LogError(gameObject.name + " Factory 없습니다.");
            return;
        }

        m_TargetingObject = null;
        gameObject.SetActive(false);
        _factory.PushObjectToPooling(this, m_TypeKey);
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
        if (m_Stat.isDead) return;

        m_ListBuff.Add(_buff);

        m_BuffAction?.Invoke(m_ListBuff);

        _buff.m_BuffExitAction = (Buff buff) => { DeleteBuff(buff); };
        _buff.PlayBuffEffect(this);

        StartCoroutine(_buff.BuffCoroutine);
        if (m_BuffRimLight != null && m_ListBuff.Count == 1)
            StartCoroutine(m_BuffRimLight.m_Coroutine);
    }

    public void DeleteBuff(Buff _buff)
    {
        if (_buff == null) return;

        if (_buff.BuffCoroutine != null)
            StopCoroutine(_buff.BuffCoroutine);
        if ((m_BuffRimLight != null && m_BuffRimLight.m_Coroutine != null && m_ListBuff.Count == 1) || m_Stat.isDead)
            StopCoroutine(m_BuffRimLight.m_Coroutine);


        // _buff.BuffExitAction();
        // _buff.BuffExitAction();

        m_ListBuff.Remove(_buff);
        m_BuffAction?.Invoke(m_ListBuff);
    }

    public void DeleteDeBuff(Buff _buff)
    {
        if (_buff != null) return;

        if(_buff.BuffCoroutine != null)
            StopCoroutine(_buff.BuffCoroutine);

       // _buff.BuffExitAction();

        m_ListDeBuff.Remove(_buff);
        m_DeBuffAction?.Invoke(m_ListDeBuff);
    }

    public void AddDeBuff(Buff _buff)
    {
        if (_buff != null) return;
        m_ListDeBuff.Add(_buff);
        m_DeBuffAction?.Invoke(m_ListDeBuff);
        StartCoroutine(_buff.BuffCoroutine);
    }

    public void AllDeleteBuff()
    {
        for (int i = 0; i< m_ListBuff.Count; i++)
        {
            m_ListBuff[i].BuffExitAction();
            //DeleteBuff(m_ListBuff[i]);
        }

        for (int i = 0; i < m_ListDeBuff.Count; i++)
        {
            m_ListDeBuff[i].BuffExitAction();
            //DeleteDeBuff(m_ListDeBuff[i]);
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
            if (CompareTag("Zombie"))
            {
                if (m_DeadCoroutine != null)
                    StopCoroutine(m_DeadCoroutine);
                m_DeadCoroutine = StartCoroutine(DeadCoroutine());
            }
            m_DeadActionCallBackFunc?.Invoke();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("emfdjsmsrksek");
            AllDeleteBuff();
        }
    }

    //데미지를 입다.
    public virtual void HitDamage(float _damage, bool _isKnockBack = false, float _knockBackTime = 0.0f)
    {
        m_Stat.CurHP -= _damage;

        if (_isKnockBack)
            m_KnockBackAction?.Invoke(_knockBackTime);
    }
    public void AddKnockBackFunction(System.Action<float> _knockBackAction)
    {
        m_KnockBackAction += _knockBackAction;
    }
    public void AddBuffFunction(System.Action<List<Buff> > _buffAction)
    {
        m_BuffAction += _buffAction;
    }
    public void AddDeBuffFunction(System.Action<List<Buff> > _deBuffAction)
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
            if (m_KnocoBackCoroutine != null)
                StopCoroutine(m_KnocoBackCoroutine);
            m_KnocoBackCoroutine = StartCoroutine(KnockBackRelease(time));
        });
    }
    //_time 받은 만큼 m_Stated의 isKnockBack을 true해준다
    public IEnumerator KnockBackRelease(float _time)
    {
        m_Stat.isKnockBack = true;
        yield return new WaitForSeconds(_time);
        m_Stat.isKnockBack = false;
    }

    public GameObject GetModel()
    {
        return m_Model;
    }

    public IEnumerator DeadCoroutine()
    {
        m_BuffRimLight.SetDissolve();
        m_BuffRimLight.SetDissolveColor(new Color(1f,0,0));
        m_BuffRimLight.SetDissolveEmission(0f);

        float DissolveAmount = 0.0f;

        while(DissolveAmount <= 1.0f)
        {
            m_BuffRimLight.SetDissolveAmount(DissolveAmount);
            DissolveAmount += Time.deltaTime * 10.0f;
            yield return null;

        }

        m_BuffRimLight.SetStandard();
    }

    public void DrawCircle(float _range)
    {

        if(m_DrawRender)
            m_DrawRender.SetRangeCircle(_range);
    }



}
