using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SOUND_BG_LOOP
{
    MAIN,
    SHOP,
    BATTLE1,
    BATTLE2,
    BATTLE3,
    BATTLE4
}

public class SoundManager : Singleton<SoundManager>
{

    SOUND_BG_LOOP m_CurrentSound;

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
      //  PlayBGM(SOUND_BG_LOOP.BATTLE1);

        return true;
    }

    public void StopCurrentBGM()
    {
        m_BGMAudio.Stop();
    }

    public void PlayBGM(SOUND_BG_LOOP _BG)
    {
        m_BGMAudio.Stop();

        int random = (Random.Range(0, m_BGAudioClipTable.Count));
        AudioClip audio =  m_BGAudioClipTable[_BG.ToString()];

        if(!audio)
        {
            Debug.LogError("그런 브금 없는데?");
            return;
        }

        m_CurrentSound = _BG;
        m_BGMAudio.clip = audio;
        m_BGMAudio.Play();
    }

}
