using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public abstract class BaseSubject : MonoBehaviour
{
    protected Dictionary<BUTTON_ACTION, List<System.Action>> m_buttonActionTable = new Dictionary<BUTTON_ACTION, List<System.Action>>();

    protected BUTTON_ACTION m_CurrentButtonAction = BUTTON_ACTION.NONE;
    protected List<System.Action> m_ListCurrentUpdateObserver;

    // 액션 추가하는 함수입니다.
    public void RegisterEvent(BUTTON_ACTION _action, System.Action _event)
    {
        if (_event == null) return;

        List<System.Action> list;
        if (m_buttonActionTable.TryGetValue(_action, out list))
        {
            list.Add(_event);
        }
        else
        {
            list = new List<System.Action>();
            list.Add(_event);
            m_buttonActionTable.Add(_action, list);
        }
    }

    // 액션이 담긴 리스트를 제거합니다.
    public void DetachEvent(BUTTON_ACTION _action, System.Action _event)
    {
        List<System.Action> list;
        if (m_buttonActionTable.TryGetValue(_action, out list))
        {
            list.Remove(_event);
        }
        else
        {
            Debug.LogError("그런 액션없는데 ?>");
        }
    }

    // 액션이 담긴 리스트를 돌리면서 액션을 실행 시킵니다.
    // 이거 private 풀지 마세요. 
    private void UpdateObserverList()
    {
        if (m_ListCurrentUpdateObserver == null)
        {
            Debug.LogError("List를 없는데 옵저버 돌린다? 퍽!");
            return;
        }

        foreach (System.Action o in m_ListCurrentUpdateObserver)
        {
            o?.Invoke();
        }
    }

    // 업데이트를 돌리는 실질적인 함수입니다. 
    protected void UpdateObserver(BUTTON_ACTION _action)
    {
        // 이상한 액션을 업데이트 하려는가?
        if (_action == BUTTON_ACTION.END || _action == BUTTON_ACTION.NONE)
        {
            Debug.LogError("그런 버튼 액션 없다. 확인");
            return;
        }

        // 액션이 지금이랑 같을때는 굳이 액션 리스트를 찾을 필요가 있나.
        if (m_CurrentButtonAction == _action)
        {
            UpdateObserverList();
            return;
        }

        // 새로운 액션이니 테이블에서 액션이 담긴 리스트를 찾자.
        m_CurrentButtonAction = _action;
        List<System.Action> list;
        if (m_buttonActionTable.TryGetValue(_action, out list))
        {
            m_ListCurrentUpdateObserver = list;
            UpdateObserverList();
        }
        
    }
}

// UI 버튼 누르면 실행하는 이벤트 실행 시킬 때
public abstract class UIPressSubject : BaseSubject, IPointerDownHandler, IPointerUpHandler
{
    // ============================ 상속해서 구현한 다음 쓰세요. ========================
    public abstract void OnPointerDown(PointerEventData eventData);
    public abstract void OnPointerUp(PointerEventData eventData);
    public abstract void OnPressed();

    // ================================================================================
}

// UI 드래그 한 것을 감지하고 이벤트를 실행시키고 싶을때, 이벤트를 등록해서 쓰시면 됩니다.
public abstract class UIDragSubject : BaseSubject, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // ============================ 상속해서 구현한 다음 쓰세요. ========================
    // 끌기를 시작할 때.
    public abstract void OnBeginDrag(PointerEventData eventData);
    // 드래그를 마쳤을때
    public abstract void OnEndDrag(PointerEventData eventData);
    // 오직 끌고서 움직여야만 발동되는 함수
    public abstract void OnDrag(PointerEventData eventData);

    // ================================================================================
}