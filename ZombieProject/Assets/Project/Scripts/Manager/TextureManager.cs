using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;


public class TextureManager : Singleton<TextureManager>
{
    Dictionary<string, Sprite> m_ItemImageTable = new Dictionary<string, Sprite>();
    Dictionary<string, Sprite> m_UIImageTable = new Dictionary<string, Sprite>();

    public override bool Initialize()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Image/ItemIcon");
        Sprite[] UISprites = Resources.LoadAll<Sprite>("Image/UIIcon");

        foreach (Sprite sprite in sprites)
        {
            Debug.Log("sprite.name : " + sprite.name);
            m_ItemImageTable.Add(sprite.name , sprite);
        }

        foreach (Sprite sprite in UISprites)
        {
            m_UIImageTable.Add(sprite.name, sprite);
        }

        return true;
    }

    public override void DestroyManager()
    {
    }

    public Sprite GetItemIcon(string _ItemString)
    {
        if (!m_ItemImageTable.ContainsKey(_ItemString))
        {
            Debug.LogError("그런 아이콘 텍스터 없는데???? : " + _ItemString);
            return null;
        }
        
        return m_ItemImageTable[_ItemString];
    }

    public Sprite GetUIIcon(string _ItemString)
    {
        if (!m_UIImageTable.ContainsKey(_ItemString))
        {
            Debug.LogError("그런 UI 아이콘 텍스터 없는데???? : " + _ItemString);
            return null;
        }

        return m_UIImageTable[_ItemString];
    }





}
