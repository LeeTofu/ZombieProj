using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputContoller : UIDragSubject
{
   

    Touch m_CurrnetTouch;
    Dictionary<int, Touch> m_TouchhTable = new Dictionary<int, Touch>();

    // 컨트롤러 놓으면 원래 위치.
    public Vector3 m_defaultPosition;

    // 인풋 컨트롤러 조이스틱을 눌렀나.
    public int m_touchCount { get; private set; }

    private Vector3 m_InputControllerPosition;
    private Canvas m_canvas;
    private GraphicRaycaster m_gr;
    private PointerEventData m_ped;
    private float m_lengthlimit =  20.0f;

    // 제어하는 캐릭터
    private MovingObject m_Character;

    // 입력 컨트롤러 배경 반지름.
    static float s_ControllerBGRadius = 70.0f;

    float m_ControllerBGRadiusDivision;

    int m_LastFingerID = -1;


    Vector3 m_CurMousePosition;
    // Drag 한 벡터
    public Vector3 m_DragDirectionVector { private set; get; }
    // 현재 캐릭터 이동할 벡터
    public Vector3 m_MoveVector { private set; get; }

    bool m_init = false;

    IEnumerator Start()
    {
        if (m_init) yield break;

        m_touchCount = 0;

        m_defaultPosition = transform.position;
        m_ControllerBGRadiusDivision = 1.0f / s_ControllerBGRadius;
        this.enabled = false;
        m_canvas = gameObject.GetComponentInParent<Canvas>();
        m_gr = m_canvas.GetComponent<GraphicRaycaster>();
        m_ped = new PointerEventData(null);
        m_InputControllerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        while (m_Character == null)
        {
            m_Character = PlayerManager.Instance.m_Player;
            this.enabled = true;
            yield return null;
        }

        m_init = true;
    }

    public float GetCurrentMouseDragLength()
    {
        Vector3 mousePosition = Vector3.zero;
        mousePosition = m_CurMousePosition;

        //if (Input.touchCount > 0 && m_LastFingerID != -1)
        //{
        //    for (int i = 0; i < Input.touches.Length; i++)
        //    {
        //        if (Input.touches[i].fingerId == m_LastFingerID)
        //        {
        //            mousePosition = Input.touches[i].position;
        //            break;
        //        }
        //    }
        //}

        float length = Vector3.Magnitude(mousePosition - m_InputControllerPosition);
        return length;
    }

    public Vector3 GetMousePosition()
    {
#if UNITY_EDITOR
        Vector3 pos = Input.mousePosition;
        pos.z = m_canvas.planeDistance;
        Vector3 mousePosition = UIManager.Instance.m_UICamera.ScreenToWorldPoint(pos);
        return mousePosition;
#elif UNITY_ANDROID
        if (Input.touchCount > 0 && m_LastFingerID != -1) 
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].fingerId == m_LastFingerID)
                {
                     Vector3 pos = Input.touches[i].position;
                    pos.z = m_canvas.planeDistance;
                    Vector3 mousePosition = UIManager.Instance.m_UICamera.ScreenToWorldPoint(pos);
                    return mousePosition;

                    //return new Vector3(Input.touches[i].position.x, Input.touches[i].position.y, transform.position.z);
                }
            }
        }
        return Vector3.zero;
#endif
    }

    public Vector3 GetDirectionVec3()
    {
        return -(m_CurMousePosition - m_InputControllerPosition);
    }
    private Vector3 CalculateSliding(RaycastHit hitCast, Vector3 characterCenter)
    {
        Vector3 directionToHit = hitCast.point - characterCenter;

        Vector3 ProjectionResult = Vector3.Project(directionToHit, hitCast.normal);
        Vector3 dir = (directionToHit - ProjectionResult);

        dir.y = 0;

        return dir;
    }
    // 벽 슬라이딩 만드는 함수.
    public bool CheckWallSliding(Vector3 _forward)
    {
        if (m_Character == null) return false;

        RaycastHit hitCast;
        Vector3 characterCenter;

        if (m_Character.CheckForwardWall(_forward, out hitCast, out characterCenter))
        {
            Vector3 dir = CalculateSliding(hitCast, characterCenter);
            m_MoveVector = dir;
            if (m_Character.CheckForwardWall(dir, out hitCast, out characterCenter))
            {
                dir = CalculateSliding(hitCast, characterCenter);
                if (Vector3.SqrMagnitude(dir) < 0.001f)
                    m_MoveVector = Vector3.zero;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    // 여기서 무브 벡터 만들음.
    public void CalculateMoveVector(Vector3 _forward)
    {
        if (m_Character == null) return;
#if !UNITY_EDITOR
        if (Input.touchCount <= 0) return;
        if (m_LastFingerID == -1) return;
#endif

        Vector3 MoveControllerDir = GetDirectionVec3();

        if (MoveControllerDir.sqrMagnitude > 0.0f)
        {
            //Camera cam = UIManager.Instance.m_UICamera;
           // MoveControllerDir = cam.transform.InverseTransformVector(new Vector3(MoveControllerDir.x, 0, MoveControllerDir.y));

       //     float length = GetCurrentMouseDragLength();
       //     float limitlength = m_lengthlimit;
       ////     Debug.Log(length);
       // //    Debug.Log(limitlength);

       //     if (length >= limitlength) 
       //         length = limitlength;

            m_DragDirectionVector = new Vector3(MoveControllerDir.y, 0.0f, -MoveControllerDir.x).normalized;
           
            // 벽에 막혀 슬라이딩 벡터를 만들어야 하나 함수.
           if( !CheckWallSliding(_forward))
            {
                m_MoveVector = m_DragDirectionVector;
            }
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (m_Character == null) return;
        if (m_Character.m_Stat.isDead) return;
        if (PlayerManager.Instance.GetPlayerState() == E_PLAYABLE_STATE.DRINK)
            return;

#if !UNITY_EDITOR
        if (m_LastFingerID != -1) return;

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].phase == TouchPhase.Began || 
                    Input.touches[i].phase == TouchPhase.Stationary ||
                    Input.touches[i].phase == TouchPhase.Moved)
                {
                    m_LastFingerID = Input.touches[i].fingerId;
                    break;
                }
            }
        }
#endif

        UpdateObserver(BUTTON_ACTION.DRAG_ENTER);
        m_InputControllerPosition = m_defaultPosition;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (m_Character == null) return;
    //    if (m_Character.m_Stat.isDead) return;

        UpdateObserver(BUTTON_ACTION.DRAG_EXIT);

      //  Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(m_LastFingerIndex).position);
        m_InputControllerPosition = m_defaultPosition;
        transform.position = m_InputControllerPosition;

        m_DragDirectionVector = Vector3.zero;

        m_LastFingerID = -1;

       // Debug.Log("EndDrag");
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (m_Character == null) return;
        if (m_Character.m_Stat.isDead) return;
        if (PlayerManager.Instance.GetPlayerState() == E_PLAYABLE_STATE.DRINK)
            return;

        m_CurMousePosition = GetMousePosition();
#if !UNITY_EDITOR
        if (m_LastFingerID == -1) return;
#endif
        if (GetCurrentMouseDragLength() < m_lengthlimit)
        {
            this.transform.position = m_CurMousePosition;
        }
        else
        {
            transform.position = m_InputControllerPosition + Vector3.Normalize(-GetDirectionVec3()) * m_lengthlimit;
        }


        UpdateObserver(BUTTON_ACTION.DRAG);
     //   Debug.Log("OnDrag");
    }

}
