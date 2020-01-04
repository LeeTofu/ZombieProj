using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode
{
    public Vector3 m_nodePos;
    public List<int> m_connectedNodeIdx = new List<int>();  //연결된 노드들의 idx모음
}

public class NavCell
{
    public int[] m_vertices = new int[3] { -1, -1, -1 };   //Cell에 포함된 노드들의 idx
    public Vector3 m_centerPos;          //Cell을 이루는 삼각형의 중심
    public int[] m_adjacentCells = new int[3] { -1, -1, -1 };  //인접한 cell들 정보

    public bool CellCheck(int a, int b, int c)
    {
        int cnt = 0;

        for(int i = 0; i < 3; i++)
        {
            if(m_vertices[i] == a)
            {
                cnt++; break;
            }
        }
        for(int i = 0; i < 3; i++)
        {
            if (m_vertices[i] == b)
            {
                cnt++; break;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (m_vertices[i] == c)
            {
                cnt++; break;
            }
        }

        return (cnt > 2)? true : false;
    }

    public bool IsAdjacentCell(int a, int b, int c)
    {
        int cnt = 0;

        for (int i = 0; i < 3; i++)
        {
            if (m_vertices[i] == a)
            {
                cnt++; break;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (m_vertices[i] == b)
            {
                cnt++; break;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (m_vertices[i] == c)
            {
                cnt++; break;
            }
        }

        return (cnt == 2) ? true : false;
    }
}

public class NavMeshMakingTool : Singleton<NavMeshMakingTool>
{
    public List<NavNode> m_navNodeList = new List<NavNode>();
    public List<NavCell> m_navCellList = new List<NavCell>();
    public int m_targetLayerMask;
    public int m_selectedNodeNum, m_targetNodeNum;

    public override bool Initialize()
    {
        m_targetLayerMask = 1 << LayerMask.NameToLayer("NavPicking") + 1 << LayerMask.NameToLayer("NavNode");
        m_selectedNodeNum = m_targetNodeNum = -1;

        return true;
    }

    public bool LoadNavMeshInfoFromFile(string filePath)
    {
        

        return true;
    }

    public void SaveNavMeshInfo()
    {

    }

    public bool CheckDuplicatedCell(int a, int b, int c)
    {
        foreach(NavCell cell in m_navCellList)
        {
            if (cell.CellCheck(a, b, c)) return true;
        }

        return false;
    }

    public void Update()
    {
        //노드 생성 or 노드 선택
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = NavMeshMakingCam.Instance.m_Camera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray.origin, ray.direction, out hit, 1000f, m_targetLayerMask))
            {
                if(hit.collider.gameObject.layer == 21)
                {
                    NavNode newNode = new NavNode();
                    newNode.m_nodePos = hit.point;

                    m_navNodeList.Add(newNode); 
                }
                else if(hit.collider.gameObject.layer == 22)
                {
                    float dist = 0.5f;
                    for(int i = 0; i < m_navNodeList.Count; i++)
                    {
                        float curDist = Vector3.Distance(hit.point, m_navNodeList[i].m_nodePos);
                        if(curDist < dist)
                        {
                            m_selectedNodeNum = i;
                            curDist = dist;
                        }
                    }
                }

            }
        }

        //노드끼리 잇는 부분
        if(Input.GetMouseButton(0))
        {
            if(m_selectedNodeNum >= 0)
            {
                RaycastHit hit;

                Ray ray = NavMeshMakingCam.Instance.m_Camera.ScreenPointToRay(Input.mousePosition);


                if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000f, m_targetLayerMask))
                {
                    float dist = 0.5f;
                    for (int i = 0; i < m_navNodeList.Count; i++)
                    {
                        if (i == m_selectedNodeNum) continue;
                        float curDist = Vector3.Distance(hit.point, m_navNodeList[i].m_nodePos);
                        if (curDist < dist)
                        {
                            m_targetNodeNum = i;
                            curDist = dist;
                        }
                    }

                    Gizmos.color = Color.black;
                    if (m_targetNodeNum >= 0)
                    {
                        Gizmos.DrawLine(m_navNodeList[m_selectedNodeNum].m_nodePos, m_navNodeList[m_targetNodeNum].m_nodePos);
                    }
                    else
                    {
                        Gizmos.DrawLine(m_navNodeList[m_selectedNodeNum].m_nodePos, hit.point);
                    }
                }
                   
            }
        }

        //노드 연결
        if(Input.GetMouseButtonUp(0))
        {
            if(m_selectedNodeNum >= 0 && m_targetNodeNum >= 0)
            {
                if(!m_navNodeList[m_selectedNodeNum].m_connectedNodeIdx.Contains(m_targetNodeNum))
                    m_navNodeList[m_selectedNodeNum].m_connectedNodeIdx.Add(m_targetNodeNum);
                if(!m_navNodeList[m_targetNodeNum].m_connectedNodeIdx.Contains(m_selectedNodeNum))
                    m_navNodeList[m_targetNodeNum].m_connectedNodeIdx.Add(m_selectedNodeNum);

                //이어진 노드끼리 cell형성할 수 있는지, 있다면 cell 생성후 리스트 삽입

                int cellMakingNodeNum = -1;

                for(int i = 0; i < m_navNodeList[m_targetNodeNum].m_connectedNodeIdx.Count; i++)
                {
                    if (cellMakingNodeNum > 0) break;
                    for(int j = 0; j < m_navNodeList[i].m_connectedNodeIdx.Count; j++)
                    {
                        int nowNodeNum = m_navNodeList[i].m_connectedNodeIdx[j];
                        if (nowNodeNum == m_selectedNodeNum)
                        {
                            if(!CheckDuplicatedCell(m_selectedNodeNum, m_targetNodeNum, nowNodeNum))
                            {
                                cellMakingNodeNum = nowNodeNum;
                                break;
                            }
                        }
                    }
                }

                if(cellMakingNodeNum > 0)
                {
                    NavCell newCell = new NavCell();
                    newCell.m_vertices[0] = m_selectedNodeNum;
                    newCell.m_vertices[1] = m_targetNodeNum;
                    newCell.m_vertices[2] = cellMakingNodeNum;
                    newCell.m_centerPos = (m_navNodeList[m_selectedNodeNum].m_nodePos +
                        m_navNodeList[m_targetNodeNum].m_nodePos +
                        m_navNodeList[cellMakingNodeNum].m_nodePos) / 3f;

                    //연결된 cell 추가작업 필요
                    for (int i = 0; i < m_navCellList.Count; i++)
                    {
                        if(m_navCellList[i].IsAdjacentCell(m_selectedNodeNum, m_targetNodeNum, cellMakingNodeNum))
                        {
                            newCell.m_adjacentCells[0] = i;
                            for(int j = 0; j < 3; j++)
                            {
                                if(m_navCellList[i].m_adjacentCells[j] < 0)
                                {
                                    m_navCellList[i].m_adjacentCells[j] = m_navCellList.Count;
                                    break;
                                }
                            }
                        }
                    }

                    m_navCellList.Add(newCell);
                }

            }

            m_selectedNodeNum = m_targetNodeNum = -1;
        }
    }
}
