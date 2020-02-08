using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : Singleton<NavMeshBaker>
{
    public List<NavMeshSurface> m_NavSurfaces = new List<NavMeshSurface>();

    public override void DestroyManager()
    {
        m_NavSurfaces.Clear();
    }

    public override bool Initialize()
    {

        return true;
    }

    // Start is called before the first frame update
    public void BakeNavMeshes()
    {
        foreach(NavMeshSurface surface in m_NavSurfaces)
            surface.BuildNavMesh();
    }

    public void AddNavSurface(NavMeshSurface surface)
    {
        m_NavSurfaces.Add(surface);
    }
}
