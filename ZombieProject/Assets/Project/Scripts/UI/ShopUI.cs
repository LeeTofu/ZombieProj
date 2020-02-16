using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private Vector3 m_OriginPosition;

    [SerializeField]
    private Vector3 m_DestinationPos;

    Vector3 m_CurDestinationPos;


    public void SetUI(bool _isopen)
    {
        if (_isopen)
        {
            gameObject.SetActive(_isopen);
            m_CurDestinationPos = m_DestinationPos;
        }
        else
        {
            m_CurDestinationPos = m_OriginPosition;
        }

        StartCoroutine(StartMoveUI_C(0.5f, _isopen));

    }

    IEnumerator StartMoveUI_C(float _time, bool _isOpen)
    {
        float time = 0.0f;
        while(time < _time)
        {
            transform.position = Vector3.Slerp(transform.position, m_CurDestinationPos, time += (Time.deltaTime * (1 / _time)));

            yield return null;
        }

        if(!_isOpen)
        {
            gameObject.SetActive(_isOpen);
        }
    }

}
