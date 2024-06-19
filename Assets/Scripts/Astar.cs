using UnityEngine;
using System.Collections.Generic;
public class Node
{
    /*
     이 노드가 벽인지 
    부모노드 
    x, y 좌표값
    f  h+g
    h  추정값 즉 가로 세로 장애물을 무시하여 목표까지의 거리
    g  시작으로 부터 이동했던 거리 
     */
    public bool isWall;
    public Node Parentnode;
    public int x, y;
    public int G;
    public int H;

    public int F
    {
        get
        {
            return G + H;
        }
    }
    public Node(bool iswall, int x, int y)
    {
        this.isWall = iswall;
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "x : " + x + ",y : " + y;
    }

}
public class Astar : MonoBehaviour
{
    GameObject destination;

    public int range = 2;

    public Vector2Int bottomLeft, topRight, start_Pos, end_Pos;

    public List<Node> final_node;
    public Node[,] nodeArray;

    //대각선을 이용 할것인지 경우 // false일 시 직선으로만 이동
    public bool AllowDigonal = true;
    //코너를 가로질러 가지 않을 경우 이동중 수직 수평 장애물이 있는 지 판단 //false일 시 코너에 닿으면서 지나감
    public bool DontCrossCorner = false;

    Node startNode, endNode, curNode;

    private int sizeX, sizeY;

    const int CostStraight = 10;
    const int CostDiagonal = 14;

    int index = 0;

    List<Node> OpenList, ClosedList;
    public void SetGizmoIndex(int index)
    {
        this.index = index;
    }
    public void Clear()
    {
        final_node.Clear();
    }

    public void SetPosInit()
    {
        float minX, maxX, minZ, maxZ;
        minX = transform.position.x;
        maxX = destination.transform.position.x;
        minZ = transform.position.z;
        maxZ = destination.transform.position.z;

        if (transform.position.x > destination.transform.position.x)
        {
            float temp = minX;
            minX = maxX;
            maxX = temp;
        }
        if (transform.position.z > destination.transform.position.z)
        {
            float temp = minZ;
            minZ = maxZ;
            maxZ = temp;
        }

        //시작부터 도착까지 range만큼 넓은 범위로 검색
        minX -= range;
        minZ -= range;
        maxX += range;
        maxZ += range;

        bottomLeft = new Vector2Int((int)minX, (int)minZ);
        topRight = new Vector2Int((int)maxX, (int)maxZ);
        start_Pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        end_Pos = new Vector2Int((int)destination.transform.position.x, (int)destination.transform.position.z);
    }

    public List<Node> AStar(GameObject targetObject)
    {
        destination = targetObject;
        SetPosInit();
        sizeX = topRight.x-bottomLeft.x + 1;
        sizeY = topRight.y-bottomLeft.y + 1;

        nodeArray = new Node[sizeX, sizeY];
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isObstacle = false;
                //각 노드에 0.49f반지름의 원을 생성하여 충돌감지 후 노드 담기
                foreach (Collider col in Physics.OverlapSphere(new Vector3(i + bottomLeft.x,0, j + bottomLeft.y), 0.49f))
                {
                    if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Border")))
                    {
                        isObstacle = true;
                    }
                }
                nodeArray[i, j] = new Node(isObstacle, i + bottomLeft.x, j + bottomLeft.y);
            }
        }

        //Node 초기화, 시작지점 끝지점 넣어주기
        startNode = nodeArray[start_Pos.x - bottomLeft.x, start_Pos.y - bottomLeft.y];
        endNode = nodeArray[end_Pos.x - bottomLeft.x, end_Pos.y - bottomLeft.y];

        //List 초기화
        OpenList = new List<Node>();
        ClosedList = new List<Node>();
        final_node = new List<Node>();

        OpenList.Add(startNode);

        while (OpenList.Count > 0)
        {
            curNode = OpenList[0];
            for (int i = 0; i < OpenList.Count; i++)
            {
                //열린 리스트중 가장 F가 작고 -> F최종 비용
                //F가 같다면 H가 작은 것을 현재 노드로 설정 -> 가상 최종비용
                if (OpenList[i].F <= curNode.F &&
                    OpenList[i].H < curNode.H)
                {
                    curNode = OpenList[i];
                }
                //열린 리스트에서 닫힌 리스트로 옮기기
                OpenList.Remove(curNode);
                ClosedList.Add(curNode);

                //노드가 목적지에 도착했을 때
                if (curNode == endNode)
                {
                    Node targetnode = endNode;
                    while (targetnode != startNode)
                    {
                        final_node.Add(targetnode);
                        targetnode = targetnode.Parentnode;
                    }
                    final_node.Add(startNode);
                    final_node.Reverse(); // 시작지점부터 노드반환
                    return final_node;
                }
                if (AllowDigonal) // 도착하지 않았을 시 계속해서 계산, 주변노드를 리스트에 넣기
                {
                    //대각선으로 움직이는 cost 계산
                    // ↗↖↙↘
                    openListAdd(curNode.x + 1, curNode.y - 1);
                    openListAdd(curNode.x - 1, curNode.y + 1);
                    openListAdd(curNode.x + 1, curNode.y + 1);
                    openListAdd(curNode.x - 1, curNode.y - 1);
                }
                //직선으로 움직이는 cost 계산
                // ↑ → ↓ ←
                openListAdd(curNode.x + 1, curNode.y);
                openListAdd(curNode.x - 1, curNode.y);
                openListAdd(curNode.x, curNode.y + 1);
                openListAdd(curNode.x, curNode.y - 1);
            }

        }
        return final_node;

    }

    public void openListAdd(int checkX, int checkY)
    {
        /*
            상하좌우 범위를 벗어나지 않고,
            벽도 아니면서
            닫힌리스트에 없어야 한다.
         */
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1//x가 bottomleft와 top right안에 있고 벗어나면 예측범위 밖
            && checkY >= bottomLeft.y && checkY < topRight.y + 1 //y도 마찬가지로 범위 확인
            && !nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall //벽인지 확인
            && !ClosedList.Contains(nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y])//닫힌 리스트인지(이미 계산된 노드인지)
            )
        {
            if (AllowDigonal)//대각선 허용 시
            {
                if (nodeArray[curNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall &&
                    nodeArray[checkX - bottomLeft.x, curNode.y - bottomLeft.y].isWall)
                {
                    return;
                }
            }
            //코너를 가로질러 가지 않을 시 (이동 중 수직 수펑 장애물이 있으면 안됨.)
            if (DontCrossCorner)
            {
                if (nodeArray[curNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall ||
                   nodeArray[checkX - bottomLeft.x, curNode.y - bottomLeft.y].isWall)
                {
                    return;
                }
            }
            //check하는 노드를 이웃 노드에 넣고 직선은 10 대각선 14
            Node neightdorNode =
                nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
            int movecost = curNode.G +
                (curNode.x - checkX == 0 || curNode.y - checkY == 0 ? CostStraight : CostDiagonal);


            //이동비용이 이웃노드 G보다 작거나, 또는 열린 리스트에 이웃노드가 없다면
            if (movecost < neightdorNode.G || !OpenList.Contains(neightdorNode))
            {
                //G H parentnode를 설정후 열린 리스트에 추가
                neightdorNode.G = movecost;
                neightdorNode.H = (
                    Mathf.Abs(neightdorNode.x - endNode.x) +
                    Mathf.Abs(neightdorNode.y - endNode.y)) * CostStraight;

                neightdorNode.Parentnode = curNode;

                OpenList.Add(neightdorNode);
            }
        }
    }
    private void OnDrawGizmos()
    {
        //씬 뷰의 Debug용도로 그림을 그릴 떄 사용합니다. 
        if (final_node != null)
        {
            for (int i = index; i < final_node.Count - 1; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(final_node[i].x,0, final_node[i].y),
                    new Vector3(final_node[i + 1].x,0, final_node[i + 1].y));
            }
        }
    }
}
