using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuffRimLight : MonoBehaviour
{
    private Material m_Material;
    private Shader m_Standard;
    private Shader m_Rim;
    private Shader m_Dissolve;

    private void Awake()
    {
        m_Rim = Shader.Find("Custom/RimLightShader");
        m_Dissolve = Shader.Find("Custom/DissolveShader");
    }

    public void Initialize(GameObject _Go)
    {
        m_Material = _Go.GetComponentInChildren<SkinnedMeshRenderer>().material;
        m_Standard = m_Material.shader;
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
}
