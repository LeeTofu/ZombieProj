using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_PLAYABLE_STATE
{
    IDLE,
    WALKING,
    ATTACK,
    NONE
}

public abstract class PlayerState
{
    public StateController m_StateContoller { get; private set; }
    public MovingObject m_PlayerObject { get; private set; }

    protected PlayerState(MovingObject playerObject ,StateController _stateContoller)
    {
        m_StateContoller = _stateContoller;
        m_PlayerObject = playerObject;
    }
    public abstract void Start();
    public abstract void Update();
    public abstract void End();
    public abstract void BeginDrag();
    public abstract void EndDrag();
    public abstract void Drag();
}

public class IdleState : PlayerState
{
    public IdleState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller) { }
    public override void Start()
    {
        CameraManager.Instance.ResetOffsetPosition();
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
        {
            m_PlayerObject.m_Animator.Play("Idle", i);
        }
    }
    public override void Update()
    {

    }
    public override void End()
    {

    }
    public override void BeginDrag()
    {
        m_StateContoller.ChangeState(E_PLAYABLE_STATE.WALKING);
    }
    public override void EndDrag()
    {

    }
    public override void Drag()
    {

    }
}

public class AttackState : PlayerState
{
    public AttackState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller) { }
    public override void Start()
    {
        // Layer1 은 오직 상체를 위한 애니메이션을 담당합니다. , Layer 0은 전체적인 상하체 기존 쓰던 애니메이션.
        // 무빙샷 때문에 이렇게 만들음.
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
        {
            m_PlayerObject.m_Animator.Play("Walking", i);
        }
    }
    public override void Update()
    {

    }
    public override void End()
    {

    }
    public override void BeginDrag()
    {

    }
    public override void EndDrag()
    {

    }
    public override void Drag()
    {

    }
}

public class WalkState : PlayerState
{
    public WalkState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller) { }
    public override void Start()
    {
        // Layer1 은 오직 상체를 위한 애니메이션을 담당합니다. Layer 0은 전체적인 상하체 기존 쓰던 애니메이션.
        // 무빙샷 때문에 이렇게 만들음.
         m_PlayerObject.m_Animator.Play("Walking", 0);
    }

    public override void Update()
    {
        CameraManager.Instance.AddOffsetVector(BattleUI.m_InputController.m_DragDirectionVector * 4.0f);

        BattleUI.m_InputController.CalculateMoveVector();
        m_PlayerObject.transform.rotation = Quaternion.LookRotation(BattleUI.m_InputController.m_DragDirectionVector);
        m_PlayerObject.transform.position += BattleUI.m_InputController.m_MoveVector * Time.deltaTime * 3.0f; //* 1.0f;
    }
    public override void End()
    {

    }
    public override void BeginDrag()
    {

    }
    public override void EndDrag()
    {
        m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
    }
    public override void Drag()
    {
        
    }
}

public class StateController : MonoBehaviour
{
    MovingObject m_Character;
    Dictionary<E_PLAYABLE_STATE, PlayerState> m_StateTable;
    public E_PLAYABLE_STATE m_eCurrentState { private set; get; }

    public PlayerState m_CurrentState { private set; get; }

    public void Initialize(MovingObject _object)
    {
        m_eCurrentState = E_PLAYABLE_STATE.NONE;
        m_Character = _object;

        m_StateTable = new Dictionary<E_PLAYABLE_STATE, PlayerState>();

        InsertState(E_PLAYABLE_STATE.IDLE, new IdleState(m_Character, this));
        InsertState(E_PLAYABLE_STATE.WALKING, new WalkState(m_Character, this));
        InsertState(E_PLAYABLE_STATE.ATTACK, new AttackState(m_Character, this));

        ChangeState(E_PLAYABLE_STATE.IDLE);

        InputContoller.AttachObserver(BUTTON_ACTION.DRAG_ENTER, BeginDragUpdate);
        InputContoller.AttachObserver(BUTTON_ACTION.DRAG_EXIT, EndDragUpdate);
        InputContoller.AttachObserver(BUTTON_ACTION.DRAG, DragUpdate);
    }

    void  InsertState(E_PLAYABLE_STATE _state , PlayerState _playerState)
    {
        if (m_StateTable.ContainsKey(_state) == true) return;
        m_StateTable.Add(_state, _playerState);
    }

    // 바깥에서 ChangeState 하면 바로 변화하게 만들자~
    // 예외처리 안했길래 예외 처리함.
    public void ChangeState(E_PLAYABLE_STATE _STATE)
    {
        // 또 같은 스테이트로 변화. 예외 처리
        if (m_eCurrentState == _STATE) return;

        // 만약 m_CurrentState가 없는 경우가 생길경우 예외처리.
        if (m_CurrentState != null)
            m_CurrentState.End();

        PlayerState state = null;

        // 만약 테이블에 상태가 없으면? 예외처리.
        if (m_StateTable.TryGetValue(_STATE,  out state))
        {
            m_CurrentState = state;
            m_CurrentState.Start();
            m_eCurrentState = _STATE;
        }
        else
        {
            Debug.LogError(_STATE +  " 이런 상태 테이블에 없는디. ");
        }
    }

    private void Update()
    {
        if(m_CurrentState != null)
            m_CurrentState.Update();
    }

    public void BeginDragUpdate()
    {
        if (m_CurrentState != null)
            m_CurrentState.BeginDrag();
    }
    public void EndDragUpdate()
    {
        if (m_CurrentState != null)
            m_CurrentState.EndDrag();
    }
    public void DragUpdate()
    {
        if (m_CurrentState != null)
            m_CurrentState.Drag();
    }
}
