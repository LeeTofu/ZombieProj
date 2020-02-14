using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffRimLight : MonoBehaviour
{
    private Material m_Material;
    private Shader m_Standard;
    private Shader m_Rim;

    private void Awake()
    {
        m_Rim = Shader.Find("Custom/RimLightShader");
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
}
