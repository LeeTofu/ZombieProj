using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk_ObjectAction : ActionNode
{
    public Vector3 m_DestinationPosition;
 

    public override bool OnUpdate()
    {
        if (!m_isActive) return true;
        if ((m_DestinationPosition - (transform.position)).magnitude < 0.1f)
        {
            transform.position = m_DestinationPosition;
            Debug.Log("end Walk");

            m_isActive = false;
            return true;
        }

        transform.LookAt(m_DestinationPosition);
        transform.position +=  ( (m_DestinationPosition - transform.position).normalized * Time.deltaTime * 0.25f );

      //  transform.rotation = Quaternion.LookRotation((m_DestinationPosition - transform.position));
        
        m_Animation.Play("Walk");

        return false;
    }
}
