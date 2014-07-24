using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class HeroGridPathfinder
{
    public List<HeroGridTile> Path;

    public HeroGridGraph GraphNodes;

    public static int NodeCompare (HeroGridTile Node1, HeroGridTile Node2)
    {
        if (Node1.f > Node2.f)
            return 1;
        else if (Node1.f < Node2.f)
            return -1;
        
        return 0;
    }

    List<HeroGridTile> OpenList, ClosedList;
    HeroGridTile Start, Finish;

    public HeroGridPathfinder ()
    {
        Path = new List<HeroGridTile> ();
        Path.Clear ();
        OpenList = new List<HeroGridTile> ();
        OpenList.Clear ();
        ClosedList = new List<HeroGridTile> ();
        ClosedList.Clear ();
    }

    public void Initialize (HeroGridTile _Start, HeroGridTile _Finish)
    {
        Path.Clear ();
        OpenList.Clear ();
        ClosedList.Clear ();
        Start = _Start;
        Finish = _Finish;

        Start.Parent = null;
        Start.CalcF (Finish);
        OpenList.Add (Start);
    }

    void ProcessNeighbors (HeroGridTile Node)
    {
        foreach (HeroGridTile Neighbor in Node.Neighbors) {
            if (ClosedList.Contains (Neighbor) || !Neighbor.Passable)
                continue;
            
            int tempG = Node.g + Node.Distance (Neighbor);
            if (!OpenList.Contains (Neighbor) || tempG < Neighbor.g) {
                Neighbor.Parent = Node;
                Neighbor.g = tempG;
                Neighbor.CalcF (Finish);
                
                if (!OpenList.Contains (Neighbor))
                    OpenList.Add (Neighbor);
            }
        }
    }
    
    public void FindPath ()
    {
        while (OpenList.Count > 0) {
            OpenList.Sort (NodeCompare);
            HeroGridTile CurrentNode = OpenList [0];
            OpenList.RemoveAt (0);
            if (CurrentNode == Finish) {
                HeroGridTile PathNode = Finish;
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