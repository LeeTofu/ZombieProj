﻿using System.Collections;
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
            m_PlayerObject.m_Animator.CrossFade("Idle", 0.3f, i);
        }
    }
    public override void Update()
    {
        PlayerManager.Instance.UpdateWeaponRange();
        if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
        {
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_IDLE);
        }
        else if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
        {
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERIDLE);
        }
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

        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG,
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
        if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
        {
            m_PlayerObject.m_Animator.CrossFade("InjuredWalking", 0.3f, 0);
            m_PlayerObject.m_Animator.CrossFade("Attack", 0.3f, 1);
        }
        else
        {
            m_PlayerObject.m_Animator.CrossFade("Walking", 0.3f, 0);
            m_PlayerObject.m_Animator.CrossFade("Attack", 0.3f, 1);
        }
    }

    Vector3 TargetingZombieOffset()
    {
        Vector3 dir = (PlayerManager.Instance.m_TargetingZombie.transform.position - m_PlayerObject.transform.position);
        dir.y = 0.0f;
        dir = dir.normalized * 4.0f;

        return dir;
    }

    void ZombieTargeting()
    {
        if (!PlayerManager.Instance.m_TargetingZombie)
        {
            CameraManager.Instance.AddOffsetVector(BattleUI.m_InputController.m_DragDirectionVector * 4.0f);
            m_PlayerObject.m_Animator.SetFloat("WalkSpeed", 0.5f);
        }
        else
        {
            Vector3 forwardDir = TargetingZombieOffset();
            CameraManager.Instance.AddOffsetVector(forwardDir);

            float d = Vector3.Dot(forwardDir, BattleUI.m_InputController.m_DragDirectionVector);

            if (d <= 0.0f)
            {
                m_PlayerObject.m_Animator.SetFloat("WalkSpeed", -0.5f);
            }
        }
    }

    public override void Update()
    {
        BattleUI.m_InputController.CalculateMoveVector(BattleUI.m_InputController.m_DragDirectionVector);

        if (PlayerManager.Instance.m_TargetingZombie == null)
            m_PlayerObject.transform.rotation = Quaternion.LookRotation(BattleUI.m_InputController.m_DragDirectionVector);

        ZombieTargeting();

        PlayerManager.Instance.UpdateWeaponRange();

        //if (PlayerManager.Instance.m_CurrentEquipedItemObject)
        //    m_PlayerObject.DrawCircle(PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_Range);
        if (PlayerManager.Instance.m_TargetingZombie != null)
            BattleUI.m_InputController.CheckWallSliding(BattleUI.m_InputController.m_MoveVector);

        m_PlayerObject.transform.position += BattleUI.m_InputController.m_MoveVector * Time.deltaTime * m_PlayerObject.m_Stat.MoveSpeed * 0.5f;
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
            {
                if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
                {
                    m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_WALKING);
                }
                else
                {
                    m_StateContoller.ChangeState(E_PLAYABLE_STATE.WALKING);
                }
            }
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
            {
                m_StateContoller.ReStart();
            }
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
        if(m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
        {
            m_PlayerObject.m_Animator.CrossFade("InjuredIdle", 0.3f, 0);
            m_PlayerObject.m_Animator.CrossFade("Attack", 0.3f, 1);
        }
        else
        {
            for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
            {
                m_PlayerObject.m_Animator.CrossFade("Attack", 0.3f, i);
            }
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
        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_RELEASE,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.ATTACK)
            {
                if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
                {
                    m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_IDLE);
                }
                else
                {
                    m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
                }
            }
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
            {
                m_StateContoller.ReStart();
            }
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
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
        {
            m_PlayerObject.m_Animator.CrossFade("Walking", 0.3f, i);
        }

        m_PlayerObject.m_Animator.SetFloat("WalkSpeed", 1.0f);
    }

    public override void Update()
    {
        CameraManager.Instance.AddOffsetVector(BattleUI.m_InputController.m_DragDirectionVector * 4.0f);

        BattleUI.m_InputController.CalculateMoveVector(BattleUI.m_InputController.m_DragDirectionVector);
        m_PlayerObject.transform.rotation = Quaternion.LookRotation(BattleUI.m_InputController.m_DragDirectionVector);
        m_PlayerObject.transform.position += BattleUI.m_InputController.m_MoveVector * Time.deltaTime * m_PlayerObject.m_Stat.MoveSpeed; //* 1.0f;

        PlayerManager.Instance.UpdateWeaponRange();
        if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
        {
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_WALKING);
        }
        //if(PlayerManager.Instance.m_CurrentEquipedItemObject)
        //    m_PlayerObject.DrawCircle(PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_Range);
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

public class InjuredIdleState : PlayerState
{
    public InjuredIdleState(MovingObject playerObject, StateController _stateController) : base(playerObject, _stateController) { }
    public override void Start()
    {
        CameraManager.Instance.ResetOffsetPosition();
        if(PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
        {
            for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
                m_PlayerObject.m_Animator.CrossFade("DaggerInjured", 0.3f, i);
        }
        else
        {
            for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
                m_PlayerObject.m_Animator.CrossFade("InjuredIdle", 0.3f, i);
        }
    }
    public override void End()
    {

    }
    public override void Update()
    {
        PlayerManager.Instance.UpdateWeaponRange();
        if (m_PlayerObject.m_Stat.CurHP > MovingObject.m_InjuredHP)
        {
            if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERIDLE);
            else
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
        }
    }
    public override void AddAction()
    {
        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG_ENTER,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.INJURED_IDLE)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_WALKING);
        });

        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.INJURED_IDLE)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_WALKING);
        });


        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_DOWN,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.INJURED_IDLE)
            {
                if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
                    m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERATTACK);
                else
                    m_StateContoller.ChangeState(E_PLAYABLE_STATE.ATTACK);
            }
        });

        BattleUI.GetItemSlot(ITEM_SLOT_SORT.SECOND).RegisterEvent(BUTTON_ACTION.PRESS_ENTER,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.INJURED_IDLE)
                m_StateContoller.ReStart();
        });
    }
}
public class InjuredWalkState : PlayerState
{
    public InjuredWalkState(MovingObject playerObject, StateController _stateController) : base(playerObject, _stateController) { }
    public override void Start()
    {
        if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
        {
            for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
                m_PlayerObject.m_Animator.CrossFade("DaggerInjuredWalking", 0.3f, i);
        }
        else
        {
            for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
                m_PlayerObject.m_Animator.CrossFade("InjuredWalking", 0.3f, i);
        }

        m_PlayerObject.m_Animator.SetFloat("WalkSpeed", 1.0f);
    }
    public override void End()
    {

    }
    public override void Update()
    {
        CameraManager.Instance.AddOffsetVector(BattleUI.m_InputController.m_DragDirectionVector * 4.0f);

        BattleUI.m_InputController.CalculateMoveVector(BattleUI.m_InputController.m_DragDirectionVector);
        m_PlayerObject.transform.rotation = Quaternion.LookRotation(BattleUI.m_InputController.m_DragDirectionVector);
        m_PlayerObject.transform.position += BattleUI.m_InputController.m_MoveVector * Time.deltaTime * m_PlayerObject.m_Stat.MoveSpeed; //* 1.0f;

        PlayerManager.Instance.UpdateWeaponRange();

        ////if (PlayerManager.Instance.m_CurrentEquipedItemObject)
        ////    m_PlayerObject.DrawCircle(PlayerManager.Instance.m_CurrentEquipedItemObject.m_CurrentStat.m_Range);

    }
    public override void AddAction()
    {
        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.INJURED_WALKING)
            {
                if (m_PlayerObject.m_Stat.CurHP > MovingObject.m_InjuredHP)
                {
                    if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
                        m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERWALKING);
                    else
                        m_StateContoller.ChangeState(E_PLAYABLE_STATE.WALKING);
                }
            }
        });

        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG_EXIT,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.INJURED_WALKING)
            {
                if (m_PlayerObject.m_Stat.CurHP > MovingObject.m_InjuredHP)
                {
                    if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
                        m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERIDLE);
                    else
                        m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
                }
                else
                {
                    m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_IDLE);
                }
            }
        });

        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_DOWN,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.INJURED_WALKING)
            {
                if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort != ITEM_SORT.DAGGER)
                    m_StateContoller.ChangeState(E_PLAYABLE_STATE.MOVING_ATTACK);
                else
                    m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERATTACK);
            }
        });
    }
}

public class KnockBackState : PlayerState
{
    private Coroutine m_Coroutine;
    public KnockBackState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller)
    {
    }
    public override void Start()
    {
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
            m_PlayerObject.m_Animator.CrossFade("KnockBack", 0.3f, i);
    }
    public override void End()
    {

    }
    public override void Update()
    {
        
    }
    public override void AddAction()
    {
        m_PlayerObject.AddKnockBackAction(0.1f);
        m_PlayerObject.AddKnockBackFunction((float time) =>
        {
            if (m_PlayerObject.m_Stat.CurHP > 0)
            {
                if (m_Coroutine != null)
                    m_StateContoller.StopCoroutine(m_Coroutine);
                m_Coroutine = m_StateContoller.StartCoroutine(KnockBackChange(time, m_PlayerObject));
            }
        });
    }
    private IEnumerator KnockBackChange(float _time, MovingObject _movingObject)
    {
        m_StateContoller.ChangeState(E_PLAYABLE_STATE.KNOCKBACK);
        while (!_movingObject.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        {
            yield return null;
        }
        while (_movingObject.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            yield return null;
        }
        if (_movingObject.m_Stat.CurHP > MovingObject.m_InjuredHP)
        {
            if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERIDLE);
            else
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
        }
        else
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_IDLE);

    }
}


public class DrinkState : PlayerState
{
    private Coroutine m_Coroutine;

    private float m_Time = 0.0f;

    public DrinkState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller)
    {
    }
    public override void Start()
    {
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
            m_PlayerObject.m_Animator.CrossFade("Drink", 0.3f, i);

        SoundManager.Instance.OneShotPlay(UI_SOUND.DRINK);

        m_Time = 0.0f;
    }
    public override void End()
    {
        m_Time = 0.0f;
    }

    public override void Update()
    {
        if(m_Time < 1.0f)
        {
            m_Time += Time.deltaTime;
            return;
        }

        if (m_PlayerObject.m_Stat != null)
        {
            if((m_PlayerObject as PlayerObject).m_ReservedBuff != null)
                BuffManager.Instance.ApplyBuff((m_PlayerObject as PlayerObject).m_ReservedBuff, m_PlayerObject);
        }

        if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_IDLE);
        else
        {
            if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERIDLE);
            else
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
        }
    }
    public override void AddAction()
    {

    }

}
public class DeathState : PlayerState
{
    public DeathState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller) { }

    public override void Start()
    {
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
        {
            m_PlayerObject.m_Animator.CrossFade("Death", 0.3f, i);
        }

        SoundManager.Instance.OneShotPlay(UI_SOUND.DRINK);
    }

    public override void End()
    {
    }

    public override void Update()
    {
        while (!m_PlayerObject.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Death"))
        {
            return;
        }
        while (m_PlayerObject.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            return;
        }

        RespawnManager.Instance.GameOver();
        BattleUI.SetDeathPanelActive(true);
    }

    public override void AddAction()
    {
    }
}

public class UseQuickState : PlayerState
{
    public UseQuickState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller) { }

    private float m_Time = 0.0f;

    public override void Start()
    {
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
        {
            m_PlayerObject.m_Animator.CrossFade("UseQuick", 0.1f, i);
        }

        m_Time = 0.0f;
    }

    public override void End()
    {
        m_Time = 0.0f;
    }

    public override void Update()
    {
        if (m_Time < 0.4f)
        {
            m_Time += Time.deltaTime;
            return;
        }

        PlayerManager.Instance.PlayReservedAction();

        if (m_Time < 1.0f)
        {
            m_Time += Time.deltaTime;
            return;
        }

        if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_IDLE);
        else
        {
            if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERIDLE);
            else
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
        }
    }

    public override void AddAction()
    {
    }
}

public class PickUpState : PlayerState
{
    public PickUpState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller) { }

    private float m_Time = 0.0f;

    public override void Start()
    {
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
        {
            m_PlayerObject.m_Animator.CrossFade("Pickup", 0.1f, i);
        }

        m_Time = 0.0f;
    }

    public override void End()
    {
        m_Time = 0.0f;
    }

    public override void Update()
    {
        if (m_Time < 0.5f)
        {
            m_Time += Time.deltaTime;
            return;
        }

        PlayerManager.Instance.PlayReservedAction();

        if (m_Time < 1.1f)
        {
            m_Time += Time.deltaTime;
            return;
        }

        if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_IDLE);
        else
        {
            if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort == ITEM_SORT.DAGGER)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERIDLE);
            else
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
        }

    }

    public override void AddAction()
    {
    }
}

public class DaggerState : PlayerState
{
    public DaggerState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller) { }

    public override void Start()
    {
        CameraManager.Instance.ResetOffsetPosition();
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
        {
            m_PlayerObject.m_Animator.CrossFade("DaggerIdle", 0.3f, i);
        }
    }

    public override void End()
    {
    }

    public override void Update()
    {
        PlayerManager.Instance.UpdateWeaponRange();
        if (PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Sort != ITEM_SORT.DAGGER)
        {
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.IDLE);
        }
        if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_IDLE);
    }

    public override void AddAction()
    {
        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG_ENTER,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.DAGGERIDLE)
             m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERWALKING);
        });

        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.DAGGERIDLE)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERWALKING);
        });
        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_DOWN,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.DAGGERIDLE)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERATTACK);
        });
    }
}

public class DaggerAttackState : PlayerState
{
    public DaggerAttackState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller) { }

    private bool m_IsHit = false;
    private Vector3 m_Forward;
    private Vector3 m_Firetransform;
    private float m_Damage;
    private float m_Range;
    public override void Start()
    {
        m_IsHit = false;
        m_Firetransform = m_PlayerObject.transform.position;
        m_Damage = PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_AttackPoint;
        m_Range = PlayerManager.Instance.m_CurrentEquipedItemObject.m_Item.m_ItemStat.m_Range;
        m_Forward = m_PlayerObject.transform.forward.normalized;
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
        {
            m_PlayerObject.m_Animator.CrossFade("DaggerAttack", 0.3f, i);
        }
    }
    Vector3 TargetingZombieOffset()
    {
        Vector3 dir = (PlayerManager.Instance.m_TargetingZombie.transform.position - m_PlayerObject.transform.position);
        dir.y = 0.0f;
        dir = dir.normalized * 4.0f;

        return dir;
    }

    void ZombieTargeting()
    {
        if (!PlayerManager.Instance.m_TargetingZombie)
        {
            CameraManager.Instance.AddOffsetVector(BattleUI.m_InputController.m_DragDirectionVector * 4.0f);
            m_PlayerObject.m_Animator.SetFloat("WalkSpeed", 0.5f);
        }
        else
        {
            Vector3 forwardDir = TargetingZombieOffset();
            CameraManager.Instance.AddOffsetVector(forwardDir);

            float d = Vector3.Dot(forwardDir, BattleUI.m_InputController.m_DragDirectionVector);

            if (d <= 0.0f)
            {
                m_PlayerObject.m_Animator.SetFloat("WalkSpeed", -0.5f);
            }
        }
    }
    public override void End()
    {
    }

    public override void Update()
    {
        ZombieTargeting();
        while (!m_PlayerObject.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.DaggerAttack"))
        {
            return;
        }
        while (m_PlayerObject.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            if (m_PlayerObject.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.47f && !m_IsHit && (PlayerManager.Instance.m_TargetingZombie != null))
            {
                m_IsHit = true;
                EnemyManager.Instance.DaggerAttackZombies(m_Firetransform,m_Range,m_Damage,false, m_Forward);
            }
            return;
        }
        if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_IDLE);
        else
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERIDLE);
    }

    public override void AddAction()
    {
    }
}
public class DaggerWalkingState : PlayerState
{
    public DaggerWalkingState(MovingObject playerObject, StateController _stateContoller) : base(playerObject, _stateContoller) { }

    public override void Start()
    {
        for (int i = 0; i < m_PlayerObject.m_Animator.layerCount; i++)
        {
            m_PlayerObject.m_Animator.CrossFade("DaggerWalking", 0.3f, i);
        }
    }
    public override void End()
    {
    }

    public override void Update()
    {
        CameraManager.Instance.AddOffsetVector(BattleUI.m_InputController.m_DragDirectionVector * 4.0f);

        BattleUI.m_InputController.CalculateMoveVector(BattleUI.m_InputController.m_DragDirectionVector);
        m_PlayerObject.transform.rotation = Quaternion.LookRotation(BattleUI.m_InputController.m_DragDirectionVector);
        m_PlayerObject.transform.position += BattleUI.m_InputController.m_MoveVector * Time.deltaTime * m_PlayerObject.m_Stat.MoveSpeed; //* 1.0f;

        PlayerManager.Instance.UpdateWeaponRange();
        if (m_PlayerObject.m_Stat.CurHP <= MovingObject.m_InjuredHP)
        {
            m_StateContoller.ChangeState(E_PLAYABLE_STATE.INJURED_WALKING);
        }
    }

    public override void AddAction()
    {
        BattleUI.m_InputController.RegisterEvent(BUTTON_ACTION.DRAG_EXIT,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.DAGGERWALKING)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERIDLE);
        });

        BattleUI.GetItemSlot(ITEM_SLOT_SORT.MAIN).RegisterEvent(BUTTON_ACTION.PRESS_DOWN,
        () =>
        {
            if (m_StateContoller.m_eCurrentState == E_PLAYABLE_STATE.DAGGERWALKING)
                m_StateContoller.ChangeState(E_PLAYABLE_STATE.DAGGERATTACK);
        });
    }
}