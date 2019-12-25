﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputContoller : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
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

    // 현재 캐릭터 이동할 벡터
    public Vector3 m_DragDirectionVector { private set; get; }
    public Vector3 m_MoveVector { private set; get; }
    // Start is called before the first frame update
    IEnumerator Start()
    {

        m_defaultPosition = transform.position;

        this.enabled = false;
        m_canvas = gameObject.GetComponentInParent<Canvas>();
        m_gr = m_canvas.GetComponent<GraphicRaycaster>();
        m_ped = new PointerEventData(null);
        m_InputControllerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        m_isHit = false;

        while(m_Character == null)
        {
            m_Character = PlayerManager.Instance.m_Player;
            this.enabled = true;
            yield return null;
        }


    }

    // Update is called once per frame
    void Update()
    {
        m_lengthlimit = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height) / s_ControllerBGRadius;

#if UNITY_EDITOR
        //if (Input.GetMouseButtonDown(0))
        //{
        //    transform.localPosition = Vector3.zero;
        //    m_position = transform.position;
        //    m_isHit = false;
        //}
        //else if (transform.localPosition == Vector3.zero) m_position = transform.position;
        //MoveDrag(Input.mousePosition);

#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            for(int i=0; i<Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                //if(touch.phase == TouchPhase.Began)
                //{
                    
                //}
                MoveDrag(touch.position);
            }
            //if (touch.phase == TouchPhase.Ended)
            //{
            //    MoveDrag();
            //}
        }
#endif
        CalculateMoveVector();
    }
    public float GetCurrentMouseDragLength()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        float length = Vector3.Magnitude(mousePosition - m_InputControllerPosition);
        return length;
    }

    private void MouseDrag()
    {
        ////Vector3 mousePosition = GetMousePosition();

        ////float length = Vector3.Magnitude(mousePosition - m_InputControllerPosition);

        ////if (length <= m_lengthlimit) transform.position = mousePosition;
        ////else transform.position = m_InputControllerPosition + Vector3.Normalize(-GetDirectionVec3()) * m_lengthlimit;

    }

    public Vector3 GetMousePosition()
    {
        return new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
    }

    public Vector3 GetDirectionVec3()
    {
        return -(GetMousePosition() - m_InputControllerPosition);
    }

    //private void OnEnable()
    //{
    //    if (GameObject.Find("InputContoller") != null)
    //    {
    //        m_InputContoller = GameObject.Find("InputContoller").GetComponent<InputContoller>();
    //        m_InputContoller.enabled = true;
    //    }
    //    if (m_Character != null) /*m_Character.m_Animator.applyRootMotion = false*/;
    //}




    
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

    private void CalculateMoveVector()
    {
        if (m_Character == null) return;
        
        Vector3 MoveControllerDir = GetDirectionVec3();

        if (MoveControllerDir.sqrMagnitude > 0.0f && m_isHit)
        {
            Camera cam = Camera.main;
            MoveControllerDir = cam.transform.InverseTransformVector(new Vector3(MoveControllerDir.x, 0, MoveControllerDir.y));

            float length = GetCurrentMouseDragLength();
            float limitlength = m_lengthlimit;
            Debug.Log(length);
            Debug.Log(limitlength);

            if (length >= limitlength) 
                length = limitlength;

            m_DragDirectionVector = new Vector3(MoveControllerDir.x, 0.0f, MoveControllerDir.y).normalized;
           
           if( CheckWallSliding(m_Character.transform.forward))
            {

            }
            else 
                m_MoveVector = m_DragDirectionVector;
        }
    }

    private void MoveDrag(Vector3 position)
    {
        //m_ped.position = position;
        //List<RaycastResult> results = new List<RaycastResult>();
        //m_gr.Raycast(m_ped, results);

        //if (results.Count != 0 && !m_isHit)
        //{
        //    for (int i = 0; i < results.Count; i++)
        //    {
        //        GameObject obj = results[i].gameObject;
        //        if (obj.name.Equals("InputContoller"))
        //        {
        //            Debug.Log(results.Count);
        //            Debug.Log("hit");
        //            m_isHit = true;
        //            MouseDrag();
        //        }
        //    }
        //}
        //else if (m_isHit) MouseDrag();
        //else if (m_isHit && GetCurrentMouseDragLength() >= m_lengthlimit)
        //{
        //    Debug.Log("계속 드래그");
        //    transform.position = m_InputControllerPosition + Vector3.Normalize(-GetDirectionVec3()) * m_lengthlimit;
        //}
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_defaultPosition = this.transform.position;
        m_InputControllerPosition = m_defaultPosition;
        m_isHit = true;

        Debug.Log("BeginDrag");
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_InputControllerPosition = m_defaultPosition;
        transform.position = m_InputControllerPosition;
        m_isHit = false;

        m_DragDirectionVector = Vector3.zero;

        Debug.Log("EndDrag");
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
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
        Debug.Log("OnDrag");
        //throw new System.NotImplementedException();
    }
}
