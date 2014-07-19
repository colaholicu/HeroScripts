using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridGraph : MonoBehaviour
{
    int Rows, Columns;
    public List<GameObject> nodes;
    
    public GridGraph (int _Rows, int _Columns)
    {
        Rows = _Rows;
        Columns = _Columns;
        nodes = new List<GameObject> ();
        nodes.Clear ();
        Debug.Log ("GridGraph constructor");
    }      
    
    int getValidIndex (int i, int j)
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
    
    public void Connect ()
    {
        for (int i = 0; i < Rows; i++) {
            bool even = (i % 2 == 0);
            for (int j = 0; j < Columns; j++) {
                for (int c = 0; c < 6; c++) {
                    int neighIndex = -1, nodeIndex = -1;
                    switch (c) {
                    case 0:
                        neighIndex = getValidIndex (i, j - 1);
                        break;
                    case 1:
                        neighIndex = getValidIndex (i, j + 1);
                        break;
                    case 2:
                        neighIndex = getValidIndex (i - 1, j);
                        break;
                    case 3:
                        neighIndex = getValidIndex (i + 1, j);
                        break;
                    case 4:
                        // i+1, j-1/j+1
                        neighIndex = getValidIndex (i - 1, even ? j + 1 : j - 1);
                        break;
                    case 5:
                        // i-1, j-1/j+1
                        neighIndex = getValidIndex (i + 1, even ? j + 1 : j - 1);
                        break;
                    }
                    
                    nodeIndex = getValidIndex (i, j);
                    if (neighIndex >= 0 && nodeIndex >= 0) {
                        GridTile node = nodes [nodeIndex].GetComponent<GridTile> ();
                        GridTile neigh = nodes [neighIndex].GetComponent<GridTile> ();
                        if (node && neigh)
                            node.addNeighbor (neigh);
                    }
                }
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
	
    }
	
    // Update is called once per frame
    void Update ()
    {
	
    }
}
