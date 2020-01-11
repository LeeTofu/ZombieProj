using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_PLAYABLE_STATE
{
    IDLE,
    WALKING,
    ATTACK,
    MOVING_ATTACK,
    INJURED_IDLE,
    INJURED_WALKING,
    KNOCKBACK,
    DRINK,
    NONE
}

public class StateController : MonoBehaviour
{
    MovingObject m_Character;
    Dictionary<E_PLAYABLE_STATE, PlayerState> m_StateTable;
    public E_PLAYABLE_STATE m_eCurrentState { private set; get; }

    public PlayerState m_CurrentState { private set; get; }

    public static bool m_isInitialize = false;


    // 게임 로그인 화면서 처음 생성시 초기화 ( 두번 이상 호출 ㄴㄴ )
    public void Initialize(MovingObject _object)
    {
        m_eCurrentState = E_PLAYABLE_STATE.NONE;
        m_Character = _object;

        m_StateTable = new Dictionary<E_PLAYABLE_STATE, PlayerState>();

        InsertState(E_PLAYABLE_STATE.IDLE, new IdleState(m_Character, this));
        InsertState(E_PLAYABLE_STATE.WALKING, new WalkState(m_Character, this));
        InsertState(E_PLAYABLE_STATE.ATTACK, new AttackState(m_Character, this));
        InsertState(E_PLAYABLE_STATE.MOVING_ATTACK, new MovingAttackState(m_Character, this));
        InsertState(E_PLAYABLE_STATE.INJURED_IDLE, new InjuredIdleState(m_Character, this));
        InsertState(E_PLAYABLE_STATE.INJURED_WALKING, new InjuredWalkState(m_Character, this));
        InsertState(E_PLAYABLE_STATE.KNOCKBACK, new KnockBackState(m_Character, this));
        InsertState(E_PLAYABLE_STATE.DRINK, new DrinkState(m_Character, this));

    }

    // 인 게임에서 만들어진 후 초기화 임. Initialize과 헷갈 ㄴㄴ
    public void InGame_Initialize()
    {
        ChangeState(E_PLAYABLE_STATE.IDLE);

        if (m_isInitialize == false)
        {
            for (E_PLAYABLE_STATE i = E_PLAYABLE_STATE.IDLE; i != E_PLAYABLE_STATE.NONE; i++)
            {
                m_StateTable[i].AddAction();
            }
        }

        m_isInitialize = true;
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

    


}
