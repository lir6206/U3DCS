//  AStarTest.cs
//
//
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

enum ListState{NULL, OPEN, CLOSE}
class nodepos
{
    public int x,y;
    public nodepos(int px, int py){x = px; y = py;}
}
class ASNode
{
    public int f,g,h;
    public nodepos pos;
    public bool walkable;
    public ListState  ls;
    public GameObject obj;
    public ASNode pre;
    public void CleanPathData()
    {
        f = g = h = 0;
        ls = ListState.NULL;
        pre = null;
    }
}
class ASMap
{
    ASNode[,] map;
    int r,c;
    public ASMap(int row, int col)
    {
        r = row;
        c = col;
        map = new ASNode[row,col];
        Debug.Log("map : " + map.ToString());
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                map[i,j] = new ASNode();
                map[i,j].pos = new nodepos(0,0);
            }
        }
    }
    public ASNode GetNode(int row, int col)
    {
        if (row >= r|| col >= c)return null;
        if (row < 0 || col < 0)return null;
        return map[row, col];
    }
    public void SetNode(int row, int col, ASNode node)
    {
        map[row, col] = node;
    }
}

class AStarPath
{
    List<ASNode> openList;
    List<ASNode> closeList;
    //List<ASNode> path;
    ASNode _start;
    ASNode _end;
    ASMap _map;
    public AStarPath()
    {
        openList = new List<ASNode>();
        closeList = new List<ASNode>();
        //path = new List<ASNode>();
    }
    public void Find(ASMap map, ASNode start, ASNode end)
    {
        openList.Clear();
        closeList.Clear();
        //path.Clear();
        _start = start;
        _end   = end;
        _map = map;

        _start.g = _start.h = _start.f = 0;
        _start.pre = null;
        AddOpen(_start);


        bool hasPath = false;
        while(openList.Count > 0)
        {
            ASNode cur = GetMinNode();
            //Debug.Log("cur find : (" + cur.pos.x + "," + cur.pos.y + ")");
            AddClose(cur);
            if (cur == _end)
            {
                EndSearch(cur);
                hasPath = true;
                break;
            }
            Near(cur);
        }

        if (!hasPath)Debug.Log("No Path");
    }
    void EndSearch(ASNode node)
    {
        Debug.Log("Search end : Success!");
        ASNode tempNode = node.pre;
        while(true)
        {
            if (tempNode != null)
            {
                if (tempNode.pre == null)break;
                tempNode.obj.GetComponent<SpriteRenderer>().color = Color.blue;
                tempNode = tempNode.pre;
            }
            else break;
        }
    }
    void RandSwapList()
    {
        int times = UnityEngine.Random.Range(0, 5);
        int src, des, len;
        len = poslist.Count - 1;
        nodepos temppos;
        for (int i = 0; i < times; i++)
        {
            src = UnityEngine.Random.Range(0, len);
            des = UnityEngine.Random.Range(0, len);
            if (src != des)
            {
                temppos = poslist[src];
                poslist[src] = poslist[des];
                poslist[des] = temppos;
            }
        }
    }
    List<nodepos> poslist = new List<nodepos>{new nodepos(1,0),
                                              new nodepos(0,1),
                                              new nodepos(-1,0),
                                              new nodepos(0,-1)};
    // the near node of the node, add near node to open
    void Near(ASNode node)
    {
        RandSwapList();
        foreach (nodepos pos in poslist)
        {
            ASNode n = _map.GetNode(node.pos.x + pos.x, node.pos.y + pos.y);
            if (n == null || n.ls == ListState.CLOSE)continue; // 没有点,在close中
            if (n.walkable) // 可走
            {
                if (n.ls == ListState.NULL) // 没处理的点
                {
                    n.g = node.g + 1;
                    n.h = H(node.pos.x + pos.x, node.pos.y - pos.y);
                    n.f = n.g + n.h;
                    n.pre = node;
                    AddOpen(n);
                }
                else if (n.ls == ListState.OPEN) // 在open中
                {
                    if (n.g > node.g + 1) // 当前的计算比open中的更优
                    {
                        n.g = node.g + 1;
                        n.h = node.h;
                        n.f = node.f;
                        n.pre = node;

                        //SortOpen();        // 重新排列open
                    }
                }
            }
        }
    }
    int W(int posx, int posy)
    {
        return 1;
    }
    int H(int posx, int posy)
    {
        return Mathf.Abs(_end.pos.x - posx) + Mathf.Abs(_end.pos.y - posy);
    }
    void AddOpen(ASNode node)
    {
        if (node != _start && node != _end)
        {
            node.obj.GetComponent<SpriteRenderer>().color = Color.gray;
        }
        node.ls = ListState.OPEN;
        openList.Add(node);
    }
    void RemoveOpen(ASNode node)
    {
        node.ls = ListState.NULL;
        openList.Remove(node);
    }
    void AddClose(ASNode node)
    {
        node.ls = ListState.CLOSE;
        closeList.Add(node);
    }
    void RemoveClose(ASNode node)
    {
        node.ls = ListState.NULL;
        closeList.Remove(node);
    }
    void SortOpen()
    {
        openList.Sort((ASNode a, ASNode b) =>{
            return (a.f - b.f);
        });
    }
    ASNode GetMinNode()
    {
        SortOpen();
        ASNode node = openList[0];
        RemoveOpen(node);
        return node;
    }
}
class GridClick : MonoBehaviour, IPointerClickHandler
{
    public ASNode node;
    public Action<GridClick> ClickAction;
    public void OnPointerClick(PointerEventData eventData)
    {
        /*
        if (node.walkable)
            Debug.Log("click (" + node.pos.x + "," + node.pos.y + ")");
        else
            Debug.Log("click (" + node.pos.x + "," + node.pos.y + ") unwalkable");
        */

        if (ClickAction != null)ClickAction(this);
    }
}
public class AStarTest : MonoBehaviour
{
    const int _ROW = 15;
    const int _COL = 30;
    ASMap map = new ASMap(_ROW, _COL);
    AStarPath path = new AStarPath();
    void Start()
    {
        ASNode node;
        for(int i = 0; i < _ROW; i++)
        {
            for(int j = 0; j < _COL; j++)
            {
                node = map.GetNode(i,j);
                if (node == null)Debug.Log("node[" + i + "," + j + "] null");
                else
                {
                    node.pos.x = i;
                    node.pos.y = j;
                    node.walkable = RandWalkable();
                }
            }
        }
        transform.position = new Vector3(0 - _ROW * 0.5f, 0 - _COL * 0.5f);
        //();
        DrawMap();
    }
    ASNode node_start;
    ASNode node_end;
    ClickState state = ClickState.NULL;
    enum ClickState {START, END, NULL};
    void OnGUI()
    {
        if (GUILayout.Button("Start"))
        {
            state = ClickState.START;
        }
        if (GUILayout.Button("End"))
        {
            state = ClickState.END;
        }
        if (GUILayout.Button("Path"))
        {
            if (node_start == null)
            {
                Debug.Log("start node is null!");
                return;
            }
            if (node_end == null)
            {
                Debug.Log("end node is null!");
                return;
            }

            CleanPathData();
            path.Find(map, node_start, node_end);
        }
    }
    void CleanPathData()
    {
        for (int i = 0; i < _ROW; i++)
        {
            for (int j = 0; j < _COL; j++)
            {
                ASNode node = map.GetNode(i,j);
                node.CleanPathData();
                if (node.walkable)
                    if (node != node_start && node != node_end)
                        node.obj.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
    void DrawMap()
    {
        for (int i = 0; i < _ROW; i++)
        {
            for (int j = 0; j < _COL; j++)
            {
                GameObject obj = CreateOneGrid(i,j);
                ASNode node = map.GetNode(i,j);
                node.obj = obj;
                GridClick grid = obj.AddComponent<GridClick>();
                grid.node = node;
                grid.ClickAction = this.GridClick;
                obj.AddComponent<BoxCollider2D>();
                if (!node.walkable)
                {
                    obj.GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
        }
    }
    void UpdateMap()
    {
        for (int i = 0; i < _ROW; i++)
        {
            for (int j = 0; j < _COL; j++)
            {
                ASNode node = map.GetNode(i,j);
                SpriteRenderer sr = node.obj.GetComponent<SpriteRenderer>();
                if (node.walkable)
                {
                    if (node == node_start)sr.color = Color.red;
                    else if (node == node_end)sr.color = Color.green;
                    else sr.color = Color.white;
                }
                else sr.color = Color.black;
            }
        }
    }
    GameObject CreateOneGrid(float x, float y)
    {
        GameObject grid = Resources.Load<GameObject>("Test/OneGrid");
        GameObject g = GameObject.Instantiate(grid);



        // scale
        Vector3 scale = g.transform.localScale;
        scale = scale * 0.92f;
        g.transform.localScale = scale;

        // parent
        g.transform.SetParent(transform);

        // position
        g.transform.localPosition = new Vector3(x, y);

        return g;
    }
    void GridClick(GridClick grid)
    {
        Debug.Log("Grid Click (" + grid.node.pos.x + "," + grid.node.pos.y + ")");

        switch(state)
        {
        case ClickState.START:
            node_start = grid.node;
            UpdateMap();
            break;
        case ClickState.END:
            node_end = grid.node;
            UpdateMap();
            break;
        default : break;
        }
        state = ClickState.NULL;
    }
    void PrintMap()
    {
        string tempstr = "";
        for (int i = 0; i < _ROW; i++)
        {
            tempstr = "";
            for (int j = 0; j < _COL; j++)
            {
                if (map.GetNode(i,j).walkable)
                    tempstr += "1";
                else
                    tempstr += "0";
            }
            Debug.Log(tempstr);
        }
    }
    bool RandWalkable()
    {
        return (UnityEngine.Random.Range(0, 99) > 10);
    }
}
