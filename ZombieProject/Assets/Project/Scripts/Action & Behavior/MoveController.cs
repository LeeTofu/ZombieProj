using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    // 제어할 캐릭터가 뭔지
    private MovingObject m_Character;

    Ray m_Ray = new Ray();
    RaycastHit m_RayCastHit = new RaycastHit();
    GameObject m_FirePos;

    [SerializeField]
    private InputContoller m_InputContoller;

    virtual public void Initialize(MovingObject _Character)
    {
        m_Character = _Character;
        m_FirePos = m_Character.gameObject;
    }

    private void OnEnable()
    {
        if (GameObject.Find("InputContoller") != null) m_InputContoller = GameObject.Find("InputContoller").GetComponent<InputContoller>();
    }

    private void Update()
    {
        if (m_InputContoller != null) InputMove();
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
        {
         //   InputAttack();
        }
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {

           Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                InputAttack();
            }
        }
#endif
    }

    void InputAttack()
    {
        Camera cam = Camera.main;

        Vector3 point = new Vector3();
        Vector2 mousePos = new Vector2();
        mousePos = Input.mousePosition;

        m_Ray = cam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        if (Physics.Raycast(m_Ray, out m_RayCastHit, 50.0f, 1 << LayerMask.NameToLayer("Ground")))
        {
            Vector3 hitPoint = m_RayCastHit.point;
            hitPoint.y = m_Character.transform.position.y;

            Item currentEquipedItem = InvenManager.Instance.m_EquipedItemSlots[ITEM_SLOT_SORT.MAIN];

            if(currentEquipedItem != null)
            {
                currentEquipedItem.Attack(
                    m_FirePos.transform.position + Vector3.up * 0.1f, 
                    (hitPoint - m_Character.transform.position).normalized,
                    m_Character);
            }

        }
    }

    void InputMove()
    {

        Vector3 vec3 = m_InputContoller.GetDirectionVec3();
        if (vec3.x != 0 && vec3.y != 0)
        {
            //m_Character.m_Animator.Play("Walk");
            Camera cam = Camera.main;
            vec3 = cam.transform.InverseTransformVector(new Vector3(vec3.x, 0, vec3.y));

            transform.rotation = Quaternion.LookRotation(new Vector3(vec3.x, 0, vec3.y));
            transform.position += transform.forward * Time.deltaTime * m_InputContoller.GetLength() / 20f;
        }

    }
}
