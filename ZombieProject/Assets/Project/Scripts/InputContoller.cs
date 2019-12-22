using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputContoller : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public static Vector2 defaultposition;
    private bool m_isMouseOver;
    private bool m_isMousePushed;
    private bool m_isHit;

    private Vector3 m_position;
    private Canvas m_canvas;
    private GraphicRaycaster m_gr;
    private PointerEventData m_ped;
    private float m_lengthlimit;

    // Start is called before the first frame update
    void Start()
    {
        //m_camera = FindObjectOfType<Camera>();
        //m_collider2D = GetComponent<Collider2D>();

        //m_canvas = GameObject.Find("BattleUI").GetComponent<Canvas>();
        this.enabled = false;
        m_canvas = gameObject.GetComponentInParent<Canvas>();
        m_gr = m_canvas.GetComponent<GraphicRaycaster>();
        m_ped = new PointerEventData(null);
        m_position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        m_isHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        m_lengthlimit = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height)/10f;

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
        
        if (results.Count != 0 && !m_isHit)
        {
            for (int i = 0; i < results.Count; i++)
            {
                GameObject obj = results[i].gameObject;
                if (obj.name.Equals("InputContoller"))
                {
                    Debug.Log(results.Count);
                    Debug.Log("hit");
                    m_isHit = true;
                    OnMouseDrag();
                }
            }
        }
        else if (m_isHit) OnMouseDrag();
        else if (m_isHit && GetLength() >= m_lengthlimit)
        {
            Debug.Log("계속 드래그");
            transform.position = m_position + Vector3.Normalize(-GetDirectionVec3()) * m_lengthlimit;
        }
    }

    public bool GetisHit()
    {
        return m_isHit;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        defaultposition = this.transform.position;
        m_position = defaultposition;
        m_isHit = true;
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = defaultposition;
        m_isHit = false;
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GetLength() < m_lengthlimit)
        {
            Vector2 currentPos = Input.mousePosition;
            this.transform.position = currentPos;
        }
        else
        {
            transform.position = m_position + Vector3.Normalize(-GetDirectionVec3()) * m_lengthlimit;
        }

        //throw new System.NotImplementedException();
    }
}
