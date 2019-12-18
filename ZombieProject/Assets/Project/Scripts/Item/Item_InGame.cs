using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// InGame에 생성될 아이템 오브젝트임... 그냥 발사 위치나 이런거 알때 씀.
public class ItemObject : MonoBehaviour
{
    const string m_FirePosString = "FirePos";
    public Transform m_FireTransform { get; private set; }

    private void Awake()
    {
        var trArr = transform.GetComponentsInChildren<Transform>();
        foreach ( Transform tr in trArr)
        {
            if (tr.name == m_FirePosString)
            {
                m_FireTransform = tr;
                return;
            }
        }


    }




}
