using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SOUND_BG_LOOP
{
    NONE,
    MAIN,
    SHOP,
    BATTLE1,
    BATTLE2,
    BATTLE3,
    BATTLE4,
    END
}

public enum UI_SOUND
{
   OK_BUTTON,
   CANCLE_BUTTON,
   SUCCESS_MISSON,
   FAIL_MISSON,
   INCHANT_SUCCESS,
   INCHANT_FAIL,
    TOUCH_EFFECT,
    BATTLE_START,

   INSTALL_BOMB,
   WEAPON_CHANGE,
   HURT1,
    HURT2,
    HURT3,
}

public class SoundManager : Singleton<SoundManager>
{
    SOUND_BG_LOOP m_CurrentSound = SOUND_BG_LOOP.NONE;

    public Dictionary<string, AudioClip> m_BGAudioClipTable = new Dictionary<string, AudioClip>();
    public Dictionary<string, AudioClip> m_UISoundAudioClipTable = new Dictionary<string, AudioClip>();
    private AudioSource m_Audio;

    public override bool Initialize()
    {
        m_Audio = gameObject.AddComponent<AudioSource>();
        m_Audio.volume = 1.0f;
        AudioClip[] BGMAudioArray = Resources.LoadAll<AudioClip>("Sound/BGM");
        AudioClip[] UIAudioArray = Resources.LoadAll<AudioClip>("Sound/UI_Sound");

        for (int i = 0; i < BGMAudioArray.Length; i++)
        {
            if(m_BGAudioClipTable.ContainsKey(BGMAudioArray[i].name))
            {
                Debug.Log("같은 키의 오디오 클립을 중복으로 집어넣었다 : " + BGMAudioArray[i].name);
                continue;
            }
            m_BGAudioClipTable.Add(BGMAudioArray[i].name, BGMAudioArray[i]);
        }

        for (int i = 0; i < UIAudioArray.Length; i++)
        {
            if (m_BGAudioClipTable.ContainsKey(UIAudioArray[i].name))
            {
                Debug.Log("같은 키의 오디오 클립을 중복으로 집어넣었다 : " + UIAudioArray[i].name);
                continue;
            }
            m_UISoundAudioClipTable.Add(UIAudioArray[i].name, UIAudioArray[i]);
        }

        m_Audio.loop = true;
        
      //  PlayBGM(SOUND_BG_LOOP.BATTLE1);

        return true;
    }

    public override void DestroyManager()
    {
    }

    public void StopCurrentBGM()
    {
        m_Audio.Stop();
    }

    public void PauseCurrentBGM()
    {
        m_Audio.Pause();
    }

    public void OneShotPlay(UI_SOUND _uiSound)
    {
         if(m_UISoundAudioClipTable.ContainsKey(_uiSound.ToString()))
        {
            m_Audio.PlayOneShot(m_UISoundAudioClipTable[_uiSound.ToString()]);
        }
    }

    public void PlayBGM(GAME_SCENE _scene)
    {
        switch (_scene)
        {
            case GAME_SCENE.IN_GAME:
                PlayBGM(SOUND_BG_LOOP.BATTLE2);
                break;
            case GAME_SCENE.LOGIN:
            case GAME_SCENE.MAIN:
            case GAME_SCENE.INVENTORY:
                PlayBGM(SOUND_BG_LOOP.MAIN);
                break;
            case GAME_SCENE.SELECT_STAGE:
                PlayBGM(SOUND_BG_LOOP.BATTLE1);
                break;
            case GAME_SCENE.SHOP:
                PlayBGM(SOUND_BG_LOOP.SHOP);
                break;
             default:
                StopCurrentBGM();
                break;
        }
    }


    public void PlayBGM(SOUND_BG_LOOP _BG)
    {
        if (m_CurrentSound == _BG) return;

        m_Audio.Stop();

        int random = (Random.Range(0, m_BGAudioClipTable.Count));
        AudioClip audio =  m_BGAudioClipTable[_BG.ToString()];

        if(!audio)
        {
            Debug.LogError("그런 브금 없는데?");
            return;
        }

        m_CurrentSound = _BG;
        m_Audio.clip = audio;
        m_Audio.Play();
    }

}
