using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputContoller : MonoBehaviour
{
    private Camera m_camera;
    private Collider2D m_collider2D;

    private bool m_isMouseOver;
    private bool m_isMousePushed;
    private bool m_isHit;

    private Vector3 m_position;
    private Canvas m_canvas;
    private GraphicRaycaster m_gr;
    private PointerEventData m_ped;
    private float m_lengthlimit = 100f;

    // Start is called before the first frame update
    void Start()
    {
        //m_camera = FindObjectOfType<Camera>();
        //m_collider2D = GetComponent<Collider2D>();

        //m_canvas = GameObject.Find("BattleUI").GetComponent<Canvas>();
        m_canvas = gameObject.GetComponentInParent<Canvas>();
        m_gr = m_canvas.GetComponent<GraphicRaycaster>();
        m_ped = new PointerEventData(null);
        m_position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        m_isHit = false;
    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) m_isHit = !m_isHit;
        MoveDrag(Input.mousePosition);
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

        //var ray = m_camera.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;
        //Physics.Raycast(ray, out hit);

        //if(hit.collider != null && hit.collider == m_collider2D && m_isMouseOver)
        //{
        //    m_isMouseOver = true;
        //}
        //else if(hit.collider != m_collider2D && m_isMouseOver)
        //{
        //    m_isMouseOver = false;
        //}
        //if(m_isMouseOver && Input.GetMouseButtonDown(0))
        //{
        //    m_isMousePushed = true;
        //}
        //if (m_isMousePushed && Input.GetMouseButton(0))
        //{
        //    Debug.Log("Drag");
        //    OnMouseDrag();
        //}
        //if(m_isMousePushed && Input.GetMouseButtonUp(0))
        //{
        //    m_isMousePushed = false;
        //}

    }
    public float GetLength()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        float length = Vector3.Magnitude(mousePosition - m_position);
        return length;
    }

    public float GetLimitedLength()
    {
        return m_lengthlimit;
    }

    private void OnMouseDrag()
    {
        m_isHit = true;
        Vector3 mousePosition = GetMousePosition();

        //Vector3 objPosition = Camera.main.ScreenToViewportPoint(mousePosition);
        float length = Vector3.Magnitude(mousePosition - m_position);

        if (length <= m_lengthlimit) transform.position = mousePosition;
        else transform.position = m_position + Vector3.Normalize(-GetDirectionVec3()) * m_lengthlimit;
        //else m_isHit = false;
       
    }

    public Vector3 GetMousePosition()
    {
        return new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
    }

    public Vector3 GetDirectionVec3()
    {
        return -(GetMousePosition() - m_position);
    }

    private void MoveDrag(Vector3 position)
    {
        m_ped.position = position;
        List<RaycastResult> results = new List<RaycastResult>();
        m_gr.Raycast(m_ped, results);

        if (results.Count != 0)
        {
            for (int i = 0; i < results.Count; i++)
            {
                GameObject obj = results[i].gameObject;
                if (obj.name.Equals("InputContoller"))
                {
                    Debug.Log(results.Count);
                    Debug.Log("hit");
                    OnMouseDrag();
                }
            }
        }
        else if (m_isHit && GetLength() >= m_lengthlimit)
        {
            Debug.Log("계속 드래그");
            transform.position = m_position + Vector3.Normalize(-GetDirectionVec3()) * m_lengthlimit;
        }
        else if (!m_isHit) transform.position = m_position;
    }

    public bool GetisHit()
    {
        return m_isHit;
    }

}
