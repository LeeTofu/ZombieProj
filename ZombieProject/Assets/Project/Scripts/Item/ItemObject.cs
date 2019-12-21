using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// InGame에 생성될 아이템 오브젝트임... 그냥 발사 위치나 이런거 알때 씀.
public class ItemObject : MonoBehaviour
{
    const string m_FirePosString = "FirePos";
    public Transform m_FireTransform { get; private set; }
    AudioSource m_audio;
    AudioClip[] m_auidoClip;

    private void Awake()
    {
        var trArr = transform.GetComponentsInChildren<Transform>();
        foreach (Transform tr in trArr)
        {
            if (tr.name == m_FirePosString)
            {
                m_FireTransform = tr;
                return;
            }
        }

        m_audio = GetComponent<AudioSource>();

        if (m_audio == null)
            m_audio = gameObject.AddComponent<AudioSource>();
    }

    public void Init(Item _item)
    {
        m_auidoClip = Resources.LoadAll<AudioClip>("Sound/WeaponSound/" + _item.m_ItemStat.m_Sort.ToString());

        m_audio = GetComponent<AudioSource>();

        if (m_audio == null)
            m_audio = gameObject.AddComponent<AudioSource>();
    }


    public void PlaySound()
    {
        if (m_audio)
            m_audio.PlayOneShot(m_auidoClip[Random.Range(0, m_auidoClip.Length)]);
        else Debug.LogError("무기에 오디오 없습니다.");
    }




}
