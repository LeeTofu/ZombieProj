using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 영래가 짜던거 업그레이드
// UI 드래그 한 것을 감지하고 이벤트를 실행시키고 싶을때, 이벤트를 등록해서 쓰시면 됩니다.
// AttachObserver 함수로 등록하시면 됩니다.
public abstract class UIDragSubject : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    static Dictionary<BUTTON_ACTION, List<System.Action>> m_buttonActionTable = new Dictionary<BUTTON_ACTION, List<System.Action>>();

    BUTTON_ACTION m_CurrentButtonAction =  BUTTON_ACTION.NONE;
    List<System.Action> m_ListCurrentUpdateObserver;

    // 액션 추가하는 함수입니다.
    static public void AttachObserver(BUTTON_ACTION _action, System.Action _event)
    {
        List<System.Action> list;
       if(m_buttonActionTable.TryGetValue(_action, out list))
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
    static public void DetachObserver(BUTTON_ACTION _action, System.Action _event)
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
        if(m_ListCurrentUpdateObserver == null)
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
        if(_action == BUTTON_ACTION.END || _action == BUTTON_ACTION.NONE)
        {
            Debug.LogError("그런 버튼 액션 없다. 확인");
            return;
        }

        // 액션이 지금이랑 같을때는 굳이 액션 리스트를 찾을 필요가 있나.
        if(m_CurrentButtonAction == _action)
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
        else
        {
            Debug.LogError("그런 액션은 테이블에 없습니다. 다시 보셈");
        }
    }


    // ============================ 상속해서 구현한 다음 쓰세요. ========================
    // 끌기를 시작할 때.
    public abstract void OnBeginDrag(PointerEventData eventData);
    // 드래그를 마쳤을때
    public abstract void OnEndDrag(PointerEventData eventData);
    // 오직 끌고서 움직여야만 발동되는 함수
    public abstract void OnDrag(PointerEventData eventData);
    // ================================================================================
}

public class InputContoller : UIDragSubject
{
    // 컨트롤러 놓으면 원래 위치.
    public Vector3 m_defaultPosition;

    // 인풋 컨트롤러 조이스틱을 눌렀나.
    public bool m_isHit { get; private set; }

    private Vector3 m_InputControllerPosition;
    private Canvas m_canvas;
    private GraphicRaycaster m_gr;
    private PointerEventData m_ped;
    private float m_lengthlimit;

    // 제어하는 캐릭터
    private MovingObject m_Character;

    // 입력 컨트롤러 배경 반지름.
    static float s_ControllerBGRadius = 10.0f;

    // Drag 한 벡터
    public Vector3 m_DragDirectionVector { private set; get; }
    // 현재 캐릭터 이동할 벡터
    public Vector3 m_MoveVector { private set; get; }

    IEnumerator Start()
    {
        m_defaultPosition = transform.position;

        this.enabled = false;
        m_canvas = gameObject.GetComponentInParent<Canvas>();
        m_gr = m_canvas.GetComponent<GraphicRaycaster>();
        m_ped = new PointerEventData(null);
        m_InputControllerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        //m_isHit = false;

        while (m_Character == null)
        {
            m_Character = PlayerManager.Instance.m_Player;
            this.enabled = true;
            yield return null;
        }
    }

    void Update()
    {
        m_lengthlimit = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height) / s_ControllerBGRadius;
    }

    public float GetCurrentMouseDragLength()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        float length = Vector3.Magnitude(mousePosition - m_InputControllerPosition);
        return length;
    }

    public Vector3 GetMousePosition()
    {
        return new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
    }

    public Vector3 GetDirectionVec3()
    {
        return -(GetMousePosition() - m_InputControllerPosition);
    }

    // 벽 슬라이딩 만드는 함수.
    public bool CheckWallSliding(Vector3 _forward)
    {
        if (m_Character == null) return false;

        RaycastHit hitCast;
        Vector3 characterCenter;

        if (m_Character.CheckForwardWall(_forward, out hitCast, out characterCenter))
        {
            Debug.DrawRay(hitCast.point, hitCast.normal, Color.white);

            Vector3 directionToHit = hitCast.point - characterCenter;

            Vector3 ProjectionResult = Vector3.Project(directionToHit, hitCast.normal);
            Vector3 dir = (directionToHit - ProjectionResult);

            dir.y = 0;
            Debug.DrawRay(hitCast.point, dir, Color.yellow);

            m_MoveVector = dir;
            return true;
        }
        else
        {
            return false;
        }
    }

    // 여기서 무브 벡터 만들음.
    public void CalculateMoveVector()
    {
        //if (m_isHit == false) return;
        if (m_Character == null) return;
        
        Vector3 MoveControllerDir = GetDirectionVec3();

        if (MoveControllerDir.sqrMagnitude > 0.0f)
        {
            Camera cam = CameraManager.Instance.m_Camera;
            MoveControllerDir = cam.transform.InverseTransformVector(new Vector3(MoveControllerDir.x, 0, MoveControllerDir.y));

            float length = GetCurrentMouseDragLength();
            float limitlength = m_lengthlimit;
            Debug.Log(length);
            Debug.Log(limitlength);

            if (length >= limitlength) 
                length = limitlength;

            m_DragDirectionVector = new Vector3(MoveControllerDir.x, 0.0f, MoveControllerDir.y).normalized;
           
            // 벽에 막혀 슬라이딩 벡터를 만들어야 하나 함수.
           if( !CheckWallSliding(m_Character.transform.forward))
            {
                m_MoveVector = m_DragDirectionVector;
            }
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        UpdateObserver(BUTTON_ACTION.DRAG_ENTER);

        m_defaultPosition = this.transform.position;
        m_InputControllerPosition = m_defaultPosition;

        Debug.Log("BeginDrag");
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        UpdateObserver(BUTTON_ACTION.DRAG_EXIT);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_InputControllerPosition = m_defaultPosition;
        transform.position = m_InputControllerPosition;

        m_DragDirectionVector = Vector3.zero;

        Debug.Log("EndDrag");
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (GetCurrentMouseDragLength() < m_lengthlimit)
        {
            Vector2 currentPos = Input.mousePosition;
            this.transform.position = currentPos;
        }
        else
        {
            transform.position = m_InputControllerPosition + Vector3.Normalize(-GetDirectionVec3()) * m_lengthlimit;

        }

        UpdateObserver(BUTTON_ACTION.DRAG);
        Debug.Log("OnDrag");
    }



}
