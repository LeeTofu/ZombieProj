using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public Dictionary<string, AudioClip> m_BGAudioClipTable = new Dictionary<string, AudioClip>();
    private AudioSource m_BGMAudio;

    public override bool Initialize()
    {
        m_BGMAudio = gameObject.AddComponent<AudioSource>();
        AudioClip[] AudioArray = Resources.LoadAll<AudioClip>("Sound/BGM");

        for(int i = 0; i < AudioArray.Length; i++)
        {
            if(m_BGAudioClipTable.ContainsKey(AudioArray[i].name))
            {
                Debug.Log("같은 키의 오디오 클립을 중복으로 집어넣었다 : " + AudioArray[i].name);
                continue;
            }
            m_BGAudioClipTable.Add(AudioArray[i].name, AudioArray[i]);
        }

        m_BGMAudio.loop = true;
        PlayRandomBGM();

        return true;
    }

    public void PlayRandomBGM()
    {
        m_BGMAudio.Stop();

        AudioClip audio =  m_BGAudioClipTable["BG" + (Random.Range(0, m_BGAudioClipTable.Count)).ToString()];
        m_BGMAudio.clip = audio;
        m_BGMAudio.Play();
    }

    public void PlayBGM(AudioClip _bgm)
    {
        m_BGMAudio.Stop();
        m_BGMAudio.clip = _bgm;
        m_BGMAudio.Play();
    }
}
