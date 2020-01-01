using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerState
{
    public StateController m_StateContoller { get; private set; }
    public MovingObject m_PlayerObject { get; private set; }

    protected PlayerState(MovingObject playerObject, StateController _stateContoller)
    {
        m_StateContoller = _stateContoller;
        m_PlayerObject = playerObject;
    }
    public abstract void Start();
    public abstract void Update();
    public abstract void End();
    public abstract void AddAction();
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

    public override void AddAction()
    {
        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG_ENTER,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.IDLE)
             m_StateContoller.ChangeState(E_PLAYABLE_STATE.WALKING);
        });


        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_DOWN,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.IDLE)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.ATTACK);
        });
    }
}


public class MovingAttackState : PlayerState
{
    public MovingAttackState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller)
    {

    }
    public override void Start()
    {
        // Layer1 은 오직 상체를 위한 애니메이션을 담당합니다. , Layer 0은 전체적인 상하체 기존 쓰던 애니메이션.
        // 무빙샷 때문에 이렇게 만들음.
        m_PlayerObject.m_Animator.Play("Attack", 1);

    }
    public override void Update()
    {
        CameraManager.Instance.AddOffsetVector(BattleUI.m_InputController.m_DragDirectionVector * 4.0f);

        BattleUI.m_InputController.CalculateMoveVector();
        m_PlayerObject.transform.rotation = Quaternion.LookRotation(BattleUI.m_InputController.m_DragDirectionVector);
        m_PlayerObject.transform.position += BattleUI.m_InputController.m_MoveVector * Time.deltaTime * m_PlayerObject.m_Stat.MoveSpeed * 0.4f; //* 1.0f;
    }
    public override void End()
    {

    }

    public override void AddAction()
    {
        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_RELEASE,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.MOVING_ATTACK)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.WALKING);
        });

        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG_EXIT,
            () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.MOVING_ATTACK)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.ATTACK);
        });

        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_DOWN,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.MOVING_ATTACK)
                m_PlayerObject.m_Animator.Play("Attack", 1);
        });
    }
}


public class AttackState : PlayerState
{
    public AttackState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller)
    {

    }
    public override void Start()
    {
        // Layer1 은 오직 상체를 위한 애니메이션을 담당합니다. , Layer 0은 전체적인 상하체 기존 쓰던 애니메이션.
        // 무빙샷 때문에 이렇게 만들음.
        m_PlayerObject.m_Animator.Play("Attack", 1);

    }
    public override void Update()
    {

    }
    public override void End()
    {

    }

    public override void AddAction()
    {
        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_RELEASE,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.ATTACK)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
        });

         BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG,
         () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.ATTACK)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.MOVING_ATTACK);
        });

        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_DOWN,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.ATTACK)
                m_PlayerObject.m_Animator.Play("Attack", 1);
        });
    }
}

public class WalkState : PlayerState
{
    public WalkState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller)
    {
    }
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
        m_PlayerObject.transform.position += BattleUI.m_InputController.m_MoveVector * Time.deltaTime * m_PlayerObject.m_Stat.MoveSpeed; //* 1.0f;
    }
    public override void End()
    {

    }

    public override void AddAction()
    {
        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG_EXIT,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.WALKING)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
        });

        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_DOWN,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.WALKING)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.MOVING_ATTACK);
        });


    }
}