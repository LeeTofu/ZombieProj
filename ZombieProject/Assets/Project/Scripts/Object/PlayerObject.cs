using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePLAYER_STATE
{
    IDLE,
    WALKING
}

public abstract class PlayerState
{
    public PlayerObject m_PlayerObject;
    protected PlayerState(PlayerObject playerObject)
    {
        m_PlayerObject = playerObject;
    }
    public abstract void Start();
    public abstract void Update();
    public abstract void End();
}

public class IdleState : PlayerState
{
    public IdleState(PlayerObject playerObject) : base(playerObject) { }
    public override void Start()
    {
        m_PlayerObject.m_Animator.Play("Idle");
    }
    public override void Update()
    {

    }
    public override void End()
    {
        
    }
}

public class WalkState : PlayerState
{
    public WalkState(PlayerObject playerObject) : base(playerObject) { }
    public override void Start()
    {
        m_PlayerObject.m_Animator.Play("Walking");
    }
    public override void Update()
    {

    }
    public override void End()
    {

    }
}

public class PlayerObject : MovingObject
{
    MoveController m_Controller;
    Dictionary<ePLAYER_STATE, PlayerState> m_StateTable;
    PlayerState m_CurrentState;

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        m_Controller = gameObject.AddComponent<MoveController>();
        m_Controller.Initialize(this);

        m_StateTable = new Dictionary<ePLAYER_STATE, PlayerState>();
        m_StateTable.Add(ePLAYER_STATE.IDLE, new IdleState(this));
        m_StateTable.Add(ePLAYER_STATE.WALKING, new WalkState(this));

        m_CurrentState = m_StateTable[ePLAYER_STATE.IDLE];

        //AddCollisionCondtion(CollisionCondition);
        //AddCollisionFunction(CollisionEvent);

        if (m_Animator == null)
        {
            m_Animator = gameObject.GetComponentInChildren<Animator>();
            m_CurrentState.Start();
        }
        return;
    }

    //void CollisionEvent(GameObject _object)
    //{
    //    m_Controller.SetIsStop(true);
    //    Vector3 c = this.transform.position + this.transform.TransformDirection(this.transform.GetComponentInChildren<CapsuleCollider>().center);
    //    Ray ray = new Ray(c, this.transform.TransformDirection(Vector3.forward));
    //    RaycastHit hit;
    //    if(Physics.Raycast(ray, out hit))
    //    {
    //        Vector3 a = c - hit.point;
    //        Vector3 b = (c + this.transform.up * -1) - hit.point;

    //        Vector3 p = hit.point - c;
    //        Vector3 pnp = Vector3.Project(p, hit.normal);
    //        Vector3 dir = (p - pnp) - transform.position;
    //        dir.y = 0;
    //        Quaternion rot = Quaternion.LookRotation(dir);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
    //    }
    //    transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime;
    //}
    //bool CollisionCondition(GameObject _object)
    //{
    //    if (_object.tag == "Wall") return true;
    //    return false;
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Vector3 c = this.transform.position + this.transform.TransformDirection(this.transform.GetComponentInChildren<CapsuleCollider>().center);
            Ray ray = new Ray(c, this.transform.TransformDirection(Vector3.forward));
            Debug.DrawRay(ray.origin, ray.direction * 2, Color.red);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1.0f))
            {
                m_Controller.SetIsStop(true);
                Debug.DrawRay(hit.point, hit.normal, Color.white);
                Vector3 p = hit.point - c;
                Vector3 pnp = Vector3.Project(p, hit.normal);
                //Debug.DrawRay(hit.point, p - pnp, Color.green);
                //Vector3 dir = (p - pnp) - transform.position;
                Vector3 dir = (p - pnp);
                dir.y = 0;
                Debug.DrawRay(hit.point, dir, Color.yellow);
                if(m_Controller.GetInputContoller().GetisHit()) transform.position += dir * Time.deltaTime * 10f;
            }
            else m_Controller.SetIsStop(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            m_Controller.SetIsStop(false);
        }
    }

    private void Update()
    {
        if(m_Controller.GetInputContoller() != null)
        {
            m_CurrentState.Update();
            if (m_Controller.GetInputContoller().GetisHit()) ChangeState(ePLAYER_STATE.WALKING);
            else if(!m_Controller.GetInputContoller().GetisHit() && !m_StateTable[ePLAYER_STATE.IDLE].Equals(m_CurrentState)) ChangeState(ePLAYER_STATE.IDLE);
        }
    }

    public void ChangeState(ePLAYER_STATE _STATE)
    {
        m_CurrentState.End();
        m_CurrentState = m_StateTable[_STATE];
        m_CurrentState.Start();
    }
}
