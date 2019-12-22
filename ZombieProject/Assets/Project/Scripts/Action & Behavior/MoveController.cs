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
        if (GameObject.Find("InputContoller") != null)
        {
            m_InputContoller = GameObject.Find("InputContoller").GetComponent<InputContoller>();
            m_InputContoller.enabled = true;
        }
        if (m_Character != null) m_Character.m_Animator.applyRootMotion = false;
    }

    private void Update()
    {
        if (m_InputContoller != null) InputMove();
    }

    private void InputMove()
    {
        Vector3 vec3 = m_InputContoller.GetDirectionVec3();
        if(vec3.x != 0 && vec3.y != 0 && m_InputContoller.GetisHit())
        {
            Camera cam = Camera.main;
            vec3 = cam.transform.InverseTransformVector(new Vector3(vec3.x, 0, vec3.y));

            transform.rotation = Quaternion.LookRotation(new Vector3(vec3.x, 0, vec3.y));

            float length = m_InputContoller.GetLength();
            float limitlength = m_InputContoller.GetLimitedLength();
            Debug.Log(length);
            Debug.Log(limitlength);
            if (length >= limitlength) length = limitlength;
            transform.position += transform.forward * Time.deltaTime * length/limitlength * 10f;

        }
    }

    public InputContoller GetInputContoller()
    {
        return m_InputContoller;
    }

}
