using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroGridGraph
{
    int Rows, Columns;
    public List<GameObject> Nodes;
        
    public int GetValidIndex (int i, int j)
    {
        if (i < 0 || j < 0)
            return -1;
        
        // columns exceeded
        if (j > (Columns - 1))
            return -1;
        
        // rows exceeded
        if (i > (Rows - 1))
            return -1;

        return ((i * Columns) + j);
    }

    public HeroGridGraph (int _Rows, int _Columns)
    {
        Rows = _Rows;
        Columns = _Columns;
        Nodes = new List<GameObject> ();
        Nodes.Clear ();
    }
    
    public void Connect ()
    {
        for (int i = 0; i < Rows; i++) {
            bool IsEven = (i % 2 == 0);
            for (int j = 0; j < Columns; j++) {
                for (int c = 0; c < 6; c++) {
                    int NeighIndex = -1, NodeIndex = -1;
                    switch (c) {
                    case 0:
                        NeighIndex = GetValidIndex (i, j - 1);
                        break;
                    case 1:
                        NeighIndex = GetValidIndex (i, j + 1);
                        break;
                    case 2:
                        NeighIndex = GetValidIndex (i - 1, j);
                        break;
                    case 3:
                        NeighIndex = GetValidIndex (i + 1, j);
                        break;
                    case 4:
                        // i+1, j-1/j+1
                        NeighIndex = GetValidIndex (i - 1, IsEven ? j + 1 : j - 1);
                        break;
                    case 5:
                        // i-1, j-1/j+1
                        NeighIndex = GetValidIndex (i + 1, IsEven ? j + 1 : j - 1);
                        break;
                    }
                    
                    NodeIndex = GetValidIndex (i, j);
                    if (NeighIndex >= 0 && NodeIndex >= 0) {
                        HeroGridTile Node = Nodes [NodeIndex].GetComponent<HeroGridTile> ();
                        HeroGridTile Neighbor = Nodes [NeighIndex].GetComponent<HeroGridTile> ();
                        if (Node && Neighbor)
                            Node.AddNeighbor (Neighbor);
                    }
                }
            }
        }
    }
}
