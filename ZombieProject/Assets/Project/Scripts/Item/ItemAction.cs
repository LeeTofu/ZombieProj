using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BUTTON_ACTION // 버튼에 행해지는 액션 타입.
{
    NONE = 12,
    PRESS_ENTER, // 아이템 버튼을 누를때
    PRESS_DOWN, // 아이템 버튼을 누르고 있는 중일때
    PRESS_RELEASE, // 아이템 버튼을 뗏을때

    DRAG_ENTER, // 드래그 시작
    DRAG, // 드래그중
    DRAG_EXIT, // 드래그 놓을때
    END
}


public class ItemAction : MonoBehaviour
{
    Item m_item;

    // 현재 아이템 쿨다운
    public float m_CurrentCoolTime { private set; get; }
    // 현재 맥스 아이템 쿨다운 
    public float m_CurrentMaxCoolTime { private set; get; }
    // 최대 맥스 쿨다운
    public float m_OriginalMaxCoolTime { private set; get; }
    // 쿨다운 퍼센테이지
    public float m_CoolTimePercentage { private set; get; }


    // 공격속도를 위해 만든 간격? 시간을 체크하기 위해 만들음.
    public float m_AttackSpeedTime { private set; get; }
    // 원래 이 아이템의 공격속도 오리지날값(공속 증가 라든가 만들 수 있으므로 오리지날 값 필요.)
    public float m_OriginalAttackSpeed { private set; get; }
    // 현재 공격속도 시간. 
    public float m_CurrentAttackSpeed { private set; get; }


    Dictionary<BUTTON_ACTION, System.Action<MovingObject, Item>> m_ActionTable = new Dictionary<BUTTON_ACTION, System.Action<MovingObject, Item>>();

    System.Action<MovingObject, Item> m_CurrentPlayAction;
    BUTTON_ACTION m_PreButtonActionType;

    bool m_isDownHover = false;

 

    public void Initialized( Item _item, BattleItemSlotButton _slotButton)
    {
        m_item = _item;

        m_CurrentCoolTime = 0.0f;
        m_CurrentMaxCoolTime = m_item.m_ItemStat.m_CoolTime;
        m_OriginalMaxCoolTime =  m_item.m_ItemStat.m_CoolTime;

        m_CurrentAttackSpeed = 0.5f;
        m_AttackSpeedTime = 0.0f;
        m_OriginalAttackSpeed = 0.5f;

        m_CurrentPlayAction = null;

    }

    private void PlayAction(MovingObject _object  ,BUTTON_ACTION _action)
    {
        System.Action<MovingObject, Item> action;

        // 전에 했던 액션과 동일하면 동일하게 실행.
        if(m_CurrentPlayAction != null && m_PreButtonActionType == _action)
        {
            m_CurrentPlayAction?.Invoke(_object, m_item);
            return;
        }

        if(m_ActionTable.TryGetValue(_action, out action))
        {
            m_CurrentPlayAction = action;
            action?.Invoke(_object, m_item);

            m_PreButtonActionType = _action;
            Debug.Log(_action + "실행");
            return;
        }

        m_PreButtonActionType = BUTTON_ACTION.END;
        Debug.Log(_action + "없는데요?");
        m_CurrentPlayAction = null;
    }

    // 기존 액션에 액션을 더하는 함수입니다. 
    public void AddPlayAction(BUTTON_ACTION _actionType, System.Action<MovingObject, Item> _action)
    {
        if (_action == null) return;

        System.Action<MovingObject, Item> action;
        if (m_ActionTable.TryGetValue(_actionType, out action))
        {
            action += _action;
        }
        else
        {
            m_ActionTable.Add(_actionType,_action);
        }
    }

    // 액션을 설정하는 함수입니다.
    public void SetPlayAction(BUTTON_ACTION _actionType, System.Action<MovingObject, Item> _action)
    {
        if (_action == null) return;

        System.Action<MovingObject, Item> action;
        if (m_ActionTable.TryGetValue(_actionType, out action))
        {
            m_ActionTable[_actionType] = _action;
        }
        else
        {
            m_ActionTable.Add(_actionType, _action);
        }
    }


    // 쿨다운 틱당 도는 함수입ㄴ다.
    public void TickItemCoolTime()
    {
        if (m_item.m_ItemStat.m_isHaveCoolTime == false) return;
        if (m_CurrentCoolTime <= 0.0f)
        {
            m_CoolTimePercentage = 0.0f;
            return;
        }

        m_CurrentCoolTime -= Time.deltaTime;

        if (m_CurrentCoolTime < 0.0f)
        {
            m_CurrentCoolTime = 0.0f;
        }

        m_CoolTimePercentage = m_CurrentCoolTime / m_CurrentMaxCoolTime;
    }

    public void TickItemAttackSpeed()
    {
        if (m_AttackSpeedTime == 0.0f) return;

        m_AttackSpeedTime -= Time.deltaTime;

        if (m_AttackSpeedTime < 0.0f)
        {
            m_AttackSpeedTime = 0.0f;
        }
    }

    bool CheckCoolDown()
    {
        if (m_item == null) return false;
        if (m_item.m_ItemStat.m_isHaveCoolTime == false) return true;

        if(m_CurrentCoolTime <= 0.0f)
        {
            return true;
        }

        return false;

    }

    bool CheckAttackSpeed()
    {
        if (m_item == null) return false;
        if (m_item.m_ItemStat.m_AttackSpeed <= 0.0f) return true;

        if (m_AttackSpeedTime <= 0.0f)
        {
            return true;
        }

        return false;
    }

    bool CheckCanActionPlay()
    {
        if (!CheckCoolDown()) return false;
        if (!CheckAttackSpeed()) return false;

        return true;
    }

    void AfterSkillActive()
    {
        m_CurrentCoolTime = m_CurrentMaxCoolTime;
        m_AttackSpeedTime = m_CurrentAttackSpeed;
        m_CurrentPlayAction = null;
    }


    // 곱연산임.
    // ==========================================
    // 쿨다운을 깍는 함수입니다.
    // -0.4f -> 쿨다운을 40프로 증가 시킨다.
    // 0.2f -> 쿨다운을 20프로 깍는다.
    // 1.0f -> 쿨다운을 100프로 깍는다. -> 쿨타임 0초됨.
    // ==========================================
    public void SetCoolDown(float _percentage)
    {
        _percentage = Mathf.Min(_percentage, 1.0f);

        m_CurrentCoolTime =  m_CurrentCoolTime * (1.0f - _percentage);
        m_CurrentMaxCoolTime = m_CurrentMaxCoolTime * (1.0f - _percentage);

    }

    // 곱연산임.
    // ==========================================
    // 공격속도를 깍는 함수입니다.
    // 쿨다운과 동일
    // ==========================================
    public void SetAttackSpeed(float _percentage)
    {
        m_CurrentAttackSpeed = m_CurrentAttackSpeed * (1.0f - _percentage);
    }

    // 버튼 눌렀을 때
    public bool OnPointerDown()
    {
        m_isDownHover = true;
        Debug.Log("스킬 아이콘을 눌렀다");

        if (!CheckCanActionPlay()) return false;

        PlayAction(PlayerManager.Instance.m_Player,  BUTTON_ACTION.PRESS_ENTER);
        return true;
        
    }

    // 버튼 누르고서 드래그 할때
    public bool OnPointerPress()
    {
        if (!m_isDownHover) return false;
        if (!CheckCanActionPlay()) return false;

        PlayAction(PlayerManager.Instance.m_Player,BUTTON_ACTION.PRESS_DOWN);

        m_AttackSpeedTime = m_CurrentAttackSpeed;

        Debug.Log("스킬 아이콘 드래그중");

        return true;
    }

    // 버튼 
    public bool OnPointerUp()
    {
        m_isDownHover = false;
        Debug.Log("스킬 아이콘 놓았다.");


        PlayAction(PlayerManager.Instance.m_Player,BUTTON_ACTION.PRESS_RELEASE);
        AfterSkillActive();

        return true;
    }

}
