using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAction : MonoBehaviour
{
    public delegate void FunctionPointer();
    // Start is called before the first frame update
    protected bool m_isPlaying = false;

    protected MovingObject m_Character;
    protected Animator m_Animation;

    private FunctionPointer m_playAction;
    private FunctionPointer m_stopAction;

    virtual public void PlayAction()
    {
        m_isPlaying = true;
       //
        //Debug.Log("Test");
        // 오버라이드해서 행동 구현 //
    }

    virtual public void StopAction()
    {
        m_isPlaying = false;
       //
      //  Debug.Log("Stop");
        // 오버라이드해서 행동 구현 //
    }

    public void Initialize(MovingObject _character)
    {
        m_Character = _character;
        m_Animation = m_Character.GetComponentInChildren<Animator>();
        if(m_Animation == null)
        {
            Debug.Log("Chracter not have a Animation!");
        }


    }

}

public class CompositeAction : ObjectAction
{
    List<ObjectAction> m_ListActionComposite = new List<ObjectAction>();

    public int m_CurActionIndex;


    public void AddAction(ObjectAction _action)
    {
        m_ListActionComposite.Add(_action);
    }

    public void DeleteAction(ObjectAction _action)
    {
        m_ListActionComposite.Remove(_action);
    }

    public override void PlayAction()
    {
        m_isPlaying = true;
        for (int i = 0; i < m_ListActionComposite.Count; i++)
        {
            m_ListActionComposite[i].PlayAction();
        }
    }

    public override void StopAction()
    {
        m_isPlaying = false;
        for (int i = 0; i < m_ListActionComposite.Count; i++)
        {
            m_ListActionComposite[i].StopAction();
        }
    }

}
