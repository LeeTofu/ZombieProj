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
        if (m_Character != null) m_Character.m_Animator.applyRootMotion = false;
    }



    public InputContoller GetInputContoller()
    {
        return m_InputContoller;
    }

}
