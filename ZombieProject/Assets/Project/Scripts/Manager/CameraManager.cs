using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
     Camera m_Camera;

    public float m_CameraSensitivity = 3.0f;

    public Vector3 m_CameraOffset { private set; get; }

    Vector3 m_DefalutCameraOffset;

    GameObject m_TargetingObject;
    Vector3 m_TargetingPosition;

    float m_ZoomOutInRatio;



    public override bool Initialize()
    {
       
        return true;
    }

    public void CameraInitialize(Vector3 _pos)
    {
        m_DefalutCameraOffset = new Vector3(0.0f, 11.0f, 0.0f);
        m_CameraOffset = m_DefalutCameraOffset;

        if (Camera.main == null)
        {
            m_Camera = GameObject.Find("MainCamera").GetComponent<Camera>();
        }
        else
        {
            m_Camera = Camera.main;
        }

        if (m_Camera == null)
            Debug.LogError("Null이다...");

        m_Camera.transform.position = _pos;
        SetTargeting(_pos);
    }

    public void SetTargeting(Vector3 _pos)
    {
        m_TargetingObject = null;
        m_TargetingPosition = _pos;
    }

    public void SetTargeting(GameObject _object)
    {
        m_TargetingObject = _object;
        m_TargetingPosition = m_TargetingObject.transform.position;
    }

    public void ResetOffsetPosition()
    {
        m_CameraOffset = m_DefalutCameraOffset;
    }

    //public void ZoomInOut(float _value)
    //{
    //    m_ZoomOutInRatio -= _value;

    //    if (m_ZoomOutInRatio < 0.1f)
    //        m_ZoomOutInRatio = 0.1f;
    //    else if (m_ZoomOutInRatio > 1.0f)
    //        m_ZoomOutInRatio = 1.0f;

    //    Vector3 CameraDir = m_CameraOffset.normalized;
    //    float originalLen = m_CameraOffset.magnitude;

    //    m_CameraOffset = CameraDir * originalLen * m_ZoomOutInRatio ;
    //    m_Camera.transform.position += m_CameraOffset;
    //}

    public void AddOffsetVector(Vector3 _vector)
    {
        m_CameraOffset = m_DefalutCameraOffset + _vector;
    }

    void TargetingCamera(GameObject _object )
    {
        TargetingCamera(_object.transform.position);
        m_TargetingObject = _object;
    }

    void TargetingCamera(Vector3 _pos)
    {
        m_TargetingObject = null;
        m_Camera.transform.position = Vector3.Slerp(m_Camera.transform.position, _pos + m_CameraOffset, Time.deltaTime * m_CameraSensitivity);
    }

    public void LateUpdate()
    {
        if (m_Camera == null) return;
        if (SceneMaster.Instance.m_CurrentScene != GAME_SCENE.IN_GAME) return;

        if(m_TargetingObject != null)
        {
             TargetingCamera(m_TargetingObject);
        }
        else
        {
            TargetingCamera(m_TargetingPosition);
        }
        
    }

}
