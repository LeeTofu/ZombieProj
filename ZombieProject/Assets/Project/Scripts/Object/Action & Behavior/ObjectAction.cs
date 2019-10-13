using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAction : MonoBehaviour
{
    public string m_ActionName { get; private set; }
    public bool m_isOneShotPlay { get; private set; }
    public bool m_isActive { get; private set; } // Action을 쓸 수 있는가 true = 사용가능, false = 불가능
    public bool m_isFinish { get; protected set; } // Action이 끝났는가? true = 끝, false = 아직 안끝남.
    public bool m_isCanPreemptive { get; private set; } // Action이 끝나지도 않았는데 뺏어서 실행 가능?

    public delegate void FunctionPointer();

    protected MovingObject m_Character;
    protected Animator m_Animation;

    private FunctionPointer m_playAction;
    private FunctionPointer m_stopAction;

    public void SetActive(bool _active)
    {
        m_isActive = _active;
    }

    virtual public void PlayAction()
    {
        // 오버라이드해서 행동 구현 //
    }

    virtual public void StopAction()
    {
        // 오버라이드해서 행동 구현 //
    }

    public void Initialize(MovingObject _character, string  _action, bool _isOneShot, bool _isPreemptive)
    {
        m_isActive = true;
        m_ActionName = _action;
        m_isOneShotPlay = _isOneShot;
        m_isCanPreemptive = _isPreemptive;
        m_Character = _character;
        m_Animation = m_Character.GetComponentInChildren<Animator>();

        if(m_Animation == null)
        {
            Debug.Log("Chracter not have a Animation!");
        }
    }

    public virtual bool CheckFinishCondition()
    {
        return true;
    }

}

