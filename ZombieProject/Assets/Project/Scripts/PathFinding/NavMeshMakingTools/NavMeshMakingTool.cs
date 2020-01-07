using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode
{
    public Vector3 m_nodePos;
    public List<int> m_connectedNodeIndices = new List<int>();  //연결된 노드들의 idx모음

    public GameObject m_nodeModel;
}

public class NavCell
{
    public int[] m_vertices = new int[3] { -1, -1, -1 };   //Cell에 포함된 노드들의 idx
    public Vector3 m_centerPos;          //Cell을 이루는 삼각형의 중심
    public int[] m_adjacentCells = new int[3] { -1, -1, -1 };  //인접한 cell들 정보

    public bool CellCheck(int _a, int _b, int _c)
    {
        int cnt = 0;

        for(int i = 0; i < 3; i++)
        {
            if(m_vertices[i] == _a)
            {
                cnt++; break;
            }
        }
        for(int i = 0; i < 3; i++)
        {
            if (m_vertices[i] == _b)
            {
                cnt++; break;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (m_vertices[i] == _c)
            {
                cnt++; break;
            }
        }

        return (cnt > 2)? true : false;
    }

    public bool IsAdjacentCell(int _a, int _b, int _c)
    {
        int cnt = 0;

        for (int i = 0; i < 3; i++)
        {
            if (m_vertices[i] == _a)
            {
                cnt++; break;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (m_vertices[i] == _b)
            {
                cnt++; break;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (m_vertices[i] == _c)
            {
                cnt++; break;
            }
        }

        return (cnt == 2) ? true : false;
    }
}

public class NavMeshMakingTool : Singleton<NavMeshMakingTool>
{
    private List<NavNode> m_navNodeList = new List<NavNode>();
    private List<NavCell> m_navCellList = new List<NavCell>();
    private int m_targetLayerMask;
    private int m_selectedNodeNum, m_targetNodeNum;
    private Vector3 m_nowMousePos;
    private GameObject m_nodePrefab;

    public override bool Initialize()
    {
        m_targetLayerMask = 1 << LayerMask.NameToLayer("NavPicking") | 1 << LayerMask.NameToLayer("NavNode");
        m_selectedNodeNum = m_targetNodeNum = -1;
        m_nodePrefab = Resources.Load<GameObject>("Prefabs/PathFinding/Node");

        return true;
    }

    public bool LoadNavMeshInfoFromFile(string _filePath)
    {
        

        return true;
    }

    public void SaveNavMeshInfo()
    {

    }

    public bool CheckDuplicatedCell(int _a, int _b, int _c)
    {
        foreach(NavCell cell in m_navCellList)
        {
            if (cell.CellCheck(_a, _b, _c)) return true;
        }

        return false;
    }

    public void Update()
    {
        

        //노드 생성 or 노드 선택
        if(Input.GetMouseButtonDown(0))
        {
            

            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Ray ray = NavMeshMakingCam.Instance.m_Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000f, m_targetLayerMask))
            {
                if (hit.transform.gameObject.layer == 21)
                {
                    NavNode newNode = new NavNode();
                    newNode.m_nodePos = hit.point;
                    newNode.m_nodeModel = Instantiate(m_nodePrefab, newNode.m_nodePos, Quaternion.identity);

                    m_navNodeList.Add(newNode);
                }
                else if(hit.transform.gameObject.layer == 22)
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

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Ray ray = NavMeshMakingCam.Instance.m_Camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000f, m_targetLayerMask))
                {
                    m_targetNodeNum = -1;
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

                    if (m_targetNodeNum < 0)
                    {
                        m_nowMousePos = hit.point;
                    }
                }
                   
            }
        }
        
        //노드 연결
        if (Input.GetMouseButtonUp(0))
        {
            if(m_selectedNodeNum >= 0 && m_targetNodeNum >= 0)
            {
                if(!m_navNodeList[m_selectedNodeNum].m_connectedNodeIndices.Contains(m_targetNodeNum))
                    m_navNodeList[m_selectedNodeNum].m_connectedNodeIndices.Add(m_targetNodeNum);
                if(!m_navNodeList[m_targetNodeNum].m_connectedNodeIndices.Contains(m_selectedNodeNum))
                    m_navNodeList[m_targetNodeNum].m_connectedNodeIndices.Add(m_selectedNodeNum);

                //이어진 노드끼리 cell형성할 수 있는지, 있다면 cell 생성후 리스트 삽입

                int cellMakingNodeNum = -1;

                for(int i = 0; i < m_navNodeList[m_targetNodeNum].m_connectedNodeIndices.Count; i++)
                {
                    if (cellMakingNodeNum > 0) break;
                    for(int j = 0; j < m_navNodeList[i].m_connectedNodeIndices.Count; j++) // -> 수정필요
                    {
                        int nowNodeNum = m_navNodeList[i].m_connectedNodeIndices[j];
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
                    Debug.Log(m_navCellList.Count);
                }

            }

            m_selectedNodeNum = m_targetNodeNum = -1;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for(int i = 0; i < m_navNodeList.Count; i++)
        {
            for(int j = 0; j < m_navNodeList[i].m_connectedNodeIndices.Count; j++)
            {
                if (i >= m_navNodeList[i].m_connectedNodeIndices[j]) continue;

                Gizmos.DrawLine(m_navNodeList[i].m_nodePos, m_navNodeList[m_navNodeList[i].m_connectedNodeIndices[j]].m_nodePos);
            }
        }

        Gizmos.color = Color.red;
        if (m_selectedNodeNum >= 0)
        {
            if (m_targetNodeNum >= 0)
                Gizmos.DrawLine(m_navNodeList[m_selectedNodeNum].m_nodePos, m_navNodeList[m_targetNodeNum].m_nodePos);
            else
                Gizmos.DrawLine(m_navNodeList[m_selectedNodeNum].m_nodePos, m_nowMousePos);
        }
    }
}