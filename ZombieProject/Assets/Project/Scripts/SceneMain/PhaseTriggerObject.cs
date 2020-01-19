using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 페이즈 발생 오브젝트
public class PhaseTriggerObject : MonoBehaviour
{
    [SerializeField]
    int m_IWillOccurPhase;

    AudioSource m_Source;

    BoxCollider m_Collidier;

    BattleUI m_UI;


    private void Start()
    {
        m_Collidier = GetComponent<BoxCollider>();
        m_Source = GetComponent<AudioSource>();
        m_UI = UIManager.Instance.GetUIObject(GAME_SCENE.IN_GAME).GetComponent<BattleUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        m_Source.Play();
        EnemyManager.Instance.OccurZombiePhase(m_IWillOccurPhase);
        m_UI.PlayWaringText();

        m_Collidier.enabled = false;
    }
}
