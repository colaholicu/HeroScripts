using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GridPathfinder
{
    public List<GridTile> Path;

    public GridGraph GraphNodes;

    public static int NodeCompare (GridTile Node1, GridTile Node2)
    {
        if (Node1.f > Node2.f)
            return 1;
        else if (Node1.f < Node2.f)
            return -1;
        
        return 0;
    }

    List<GridTile> OpenList, ClosedList;
    GridTile Start, Finish;

    public GridPathfinder ()
    {
        Path = new List<GridTile> ();
        Path.Clear ();
        OpenList = new List<GridTile> ();
        OpenList.Clear ();
        ClosedList = new List<GridTile> ();
        ClosedList.Clear ();
    }

    public void Initialize (GridTile _Start, GridTile _Finish)
    {
        Path.Clear ();
        OpenList.Clear ();
        ClosedList.Clear ();
        Start = _Start;
        Finish = _Finish;

        Start.Parent = null;
        Start.calcF (Finish);
        OpenList.Add (Start);
    }

    void ProcessNeighbors (GridTile Node)
    {
        foreach (GridTile Neighbor in Node.Neighbors) {
            if (ClosedList.Contains (Neighbor) || !Neighbor.Passable)
                continue;
            
            int tempG = Node.g + Node.distance (Neighbor);
            if (!OpenList.Contains (Neighbor) || tempG < Neighbor.g) {
                Neighbor.Parent = Node;
                Neighbor.g = tempG;
                Neighbor.calcF (Finish);
                
                if (!OpenList.Contains (Neighbor))
                    OpenList.Add (Neighbor);
            }
        }
    }
    
    public void FindPath ()
    {
        while (OpenList.Count > 0) {
            OpenList.Sort (NodeCompare);
            GridTile CurrentNode = OpenList [0];
            OpenList.RemoveAt (0);
            if (CurrentNode == Finish) {
                GridTile PathNode = Finish;
                while (PathNode != null) {
                    Path.Add (PathNode);
                    PathNode = PathNode.Parent;
                }
                break;
            }
            
            ClosedList.Add (CurrentNode);

            ProcessNeighbors (CurrentNode);
        }
    }
}