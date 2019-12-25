using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class PlayerObject : MovingObject
{
    public StateController m_StateController { private set; get; }
    MoveController m_Controller;
   // Dictionary<E_PLAYABLE_STATE, PlayerState> m_StateTable;

    //public PlayerState m_CurrentState { private set; get; } 

    public override void Initialize(GameObject _model, MoveController _Controller)
    {
        if (m_Animator == null)
        {
            m_Animator = gameObject.GetComponentInChildren<Animator>();
            m_Animator.applyRootMotion = false;
            //m_CurrentState.Start();
        }

        m_Controller = gameObject.AddComponent<MoveController>();
        m_Controller.Initialize(this);

        m_StateController = gameObject.AddComponent<StateController>();
        m_StateController.Initialize(this);

        

        AddCollisionCondtion(CollisionCondition);
        AddCollisionFunction(CollisionEvent);
        AddCollisionExitFunction(CollisionExitEvent);

        //m_StateTable = new Dictionary<E_PLAYABLE_STATE, PlayerState>();
        //m_StateTable.Add(E_PLAYABLE_STATE.IDLE, new IdleState(this));
        //m_StateTable.Add(E_PLAYABLE_STATE.WALKING, new WalkState(this));
        //m_StateTable.Add(E_PLAYABLE_STATE.WALKING, new AttackState(this));

        //m_CurrentState = m_StateTable[E_PLAYABLE_STATE.IDLE];

        //AddCollisionCondtion(CollisionCondition);
        //AddCollisionFunction(CollisionEvent);


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

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Wall"))
    //    {
    //        Vector3 center = this.transform.position + this.transform.TransformDirection(this.transform.GetComponentInChildren<CapsuleCollider>().center);
    //        Ray ray = new Ray(center, this.transform.TransformDirection(Vector3.forward));

    //        Debug.DrawRay(ray.origin, ray.direction * 2, Color.red);

    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit, 1.0f))
    //        {
    //            m_Controller.SetIsStop(true);
    //            Debug.DrawRay(hit.point, hit.normal, Color.white);

    //            Vector3 DirectionToHit = hit.point - center;

    //            Vector3 ProjectionResult = Vector3.Project(DirectionToHit, hit.normal);

    //            //Debug.DrawRay(hit.point, p - pnp, Color.green);
    //            //Vector3 dir = (p - pnp) - transform.position;
    //            Vector3 dir = (DirectionToHit - ProjectionResult);
    //            dir.y = 0;
    //            Debug.DrawRay(hit.point, dir, Color.yellow);

    //            if (BattleUI.m_InputController != null)
    //            {
    //                if (BattleUI.m_InputController.m_isHit) 
    //                    transform.position += dir * Time.deltaTime * 10f;
    //            }
    //        }
    //        else 
    //            m_Controller.SetIsStop(false);
    //    }
    //}

        //벽 충돌
    void CollisionEvent(GameObject _object)
    {
        //Vector3 center = this.transform.position + this.transform.TransformDirection(this.transform.GetComponentInChildren<CapsuleCollider>().center);
        //Ray ray = new Ray(center, this.transform.TransformDirection(Vector3.forward));

        //Debug.DrawRay(ray.origin, ray.direction * 2, Color.red);

        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, 1.0f))
        //{
        //    //m_Controller.SetIsStop(true);
        //    Debug.DrawRay(hit.point, hit.normal, Color.white);

        //    Vector3 directionToHit = hit.point - center;

        //    Vector3 ProjectionResult = Vector3.Project(directionToHit, hit.normal);
        //    Vector3 dir = (directionToHit - ProjectionResult);

        //    dir.y = 0;
        //    Debug.DrawRay(hit.point, dir, Color.yellow);

        //    if (BattleUI.m_InputController != null)
        //    {
        //        if (BattleUI.m_InputController.m_isHit)
        //            transform.position += dir * Time.deltaTime * 10f;
        //    }
        //}
        //else
        //{
        //    //m_Controller.SetIsStop(false);
        //}
    }

    bool CollisionCondition(GameObject _defender)
    {
         // if (_defender.tag == "Wall") return true;
            return false;
    }

    void CollisionExitEvent(GameObject _collision)
    {
      //  if (_collision.CompareTag("Wall"))
      //  {
            // m_Controller.SetIsStop(false);
       // }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Wall"))
    //    {
    //       // m_Controller.SetIsStop(false);
    //    }
    //}

    //private void Update()
    //{
    //    if(m_Controller.GetInputContoller() != null)
    //    {
    //        m_CurrentState.Update();
    //        if (m_Controller.GetInputContoller().GetisHit()) 
    //            ChangeState(E_PLAYABLE_STATE.WALKING);
    //        else if(!m_Controller.GetInputContoller().GetisHit() && !m_StateTable[E_PLAYABLE_STATE.IDLE].Equals(m_CurrentState)) 
    //            ChangeState(E_PLAYABLE_STATE.IDLE);
    //    }
    //}

}
