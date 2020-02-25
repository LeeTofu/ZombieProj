using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum COLOR_TYPE
{
    NONE=0,
    RED=1,
    YELLOW=2,
    GREEN=4,
    PURPLE=8,
    END=128
}

public class BuffRimLight : MonoBehaviour
{
    private Material m_Material;
    private Shader m_Standard;
    private Shader m_Rim;
    private Shader m_Dissolve;
    private int m_BitMask;
    private MovingObject m_MovingObject;
    public IEnumerator m_Coroutine;

    private void Awake()
    {
        m_Rim = Shader.Find("Custom/RimLightShader");
        m_Dissolve = Shader.Find("Custom/DissolveShader");
    }

    public void Initialize(GameObject _Go)
    {
        m_Material = _Go.GetComponentInChildren<SkinnedMeshRenderer>().material;
        m_Standard = m_Material.shader;
        m_BitMask = 0;
        m_Coroutine = ColorCoroutine();
    }

    public void SetColor(Color _Color)
    {
        m_Material.SetColor("_RimColor", _Color);
    }

    public void SetRimLight()
    {
        m_Material.shader = m_Rim;
    }
    public void SetStandard()
    {
        m_Material.shader = m_Standard;
    }

    public void SetDissolve()
    {
        m_Material.shader = m_Dissolve;
        m_Material.SetTexture("_SliceGuide", TextureManager.Instance.GetTextures("dissolve"));
    }

    public void SetDissolveAmount(float _Amount)
    {
        m_Material.SetFloat("_SliceAmount", _Amount);
    }
    public void SetDissolveColor(Color _Color)
    {
        m_Material.SetColor("_DissolveColor", _Color);
    }
    public void SetDissolveEmission(float _Amount)
    {
        m_Material.SetFloat("_DissolveEmission", _Amount);
    }
    public int GetColorMask()
    {
        return m_BitMask;
    }
    public void SetColorMask(COLOR_TYPE _ColorType)
    {
        m_BitMask += (int)_ColorType;
    }
    public bool IsSetColor(COLOR_TYPE _ColorType)
    {
        int check = m_BitMask & (int)_ColorType;
        if (check != 0)
            return true;
        return false;
    }
    public void SetMovingObject(MovingObject _Mo)
    {
        m_MovingObject = _Mo;
    }
    public IEnumerator ColorCoroutine()
    {
        while(true)
        {
            for(int i=0; i<m_MovingObject.GetListBuff().Count; i++)
            {
                SetColor(m_MovingObject.GetListBuff()[i].m_Color);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
