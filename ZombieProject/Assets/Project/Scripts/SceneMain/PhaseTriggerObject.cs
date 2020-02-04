using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 페이즈 발생 오브젝트
public class DebuffAreaCollisionArea : CollisionAction
{
    [SerializeField]
    int m_IWillOccurPhase;

    AudioSource m_Source;

    BoxCollider m_Collidier;

    protected override void CollisionEvent(GameObject _object)
    {
        (m_Character as BuffArea).ApplyBuff(_object.GetComponent<MovingObject>());
    }

    protected override bool CollisionCondition(GameObject _defender)
    {
        if (_defender.GetComponent<MovingObject>() == null) return false;
        if (_defender.tag != "Zombie" && _defender.tag != "Player") return false;

        return true;
    }
}
