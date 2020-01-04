using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 페이즈 발생 오브젝트
public class PhaseTriggerObject : MonoBehaviour
{
    [SerializeField]
    int m_IWillOccurPhase;

    AudioSource m_Source;

    private void Start()
    {
        m_Source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        m_Source.Play();
        EnemyManager.Instance.OccurZombiePhase(m_IWillOccurPhase);

        gameObject.SetActive(false);
    }
}
