using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshMakingCam : Singleton<NavMeshMakingCam>
{
    public Camera m_Camera;
    public float m_cameraYaw;
    public float m_cameraPitch;
    public float m_cameraMoveSpd;
    public float m_cameraYawRotSpd;
    public float m_cameraPitchRotSpd;

    public override bool Initialize()
    {
        if (!m_Camera)
        {
            m_Camera = Camera.main;
        }

        m_cameraYaw = 0f;
        m_cameraPitch = 0f;

        m_cameraMoveSpd = 25f;
        m_cameraYawRotSpd = 100f;
        m_cameraPitchRotSpd = 100f;

        return true;
    }   

    void LateUpdate()
    {
        //카메라 이동부분

        if(Input.GetKey(KeyCode.W))
        {
            m_Camera.transform.position += m_Camera.transform.forward * m_cameraMoveSpd * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.A))
        {
            m_Camera.transform.position -= m_Camera.transform.right * m_cameraMoveSpd * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            m_Camera.transform.position -= m_Camera.transform.forward * m_cameraMoveSpd * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            m_Camera.transform.position += m_Camera.transform.right * m_cameraMoveSpd * Time.deltaTime;
        }

        //카메라 회전부분
        if(Input.GetKey(KeyCode.Space))
        {
            m_cameraYaw += m_cameraYawRotSpd * Input.GetAxis("Mouse X") * Time.deltaTime;
            m_cameraPitch -= m_cameraPitchRotSpd * Input.GetAxis("Mouse Y") * Time.deltaTime;

            m_Camera.transform.rotation *= Quaternion.Euler(m_cameraPitch, 0f, 0f);
            m_Camera.transform.Rotate(Vector3.up, m_cameraYaw, Space.World);

            m_cameraYaw = m_cameraPitch = 0f;
        }
        
    }

}
