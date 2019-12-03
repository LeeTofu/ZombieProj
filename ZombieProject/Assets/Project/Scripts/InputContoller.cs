using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputContoller : MonoBehaviour
{
    private Camera m_camera;
    private Collider2D m_collider2D;

    private bool m_isMouseOver;
    private bool m_isMousePushed;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = FindObjectOfType<Camera>();
        m_collider2D = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var ray = m_camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        
        if(hit.collider != null && hit.collider == m_collider2D && m_isMouseOver)
        {
            m_isMouseOver = true;
        }
        else if(hit.collider != m_collider2D && m_isMouseOver)
        {
            m_isMouseOver = false;
        }
        if(m_isMouseOver && Input.GetMouseButtonDown(0))
        {
            m_isMousePushed = true;
        }
        if (m_isMousePushed && Input.GetMouseButton(0))
        {
            Debug.Log("Drag");
            OnMouseDrag();
        }
        if(m_isMousePushed && Input.GetMouseButtonUp(0))
        {
            m_isMousePushed = false;
        }

    }

    void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);

        Vector3 objPosition = Camera.main.ScreenToViewportPoint(mousePosition);
        transform.position = objPosition;
    }

}
