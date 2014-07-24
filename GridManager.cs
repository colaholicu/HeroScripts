using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject HexTile;
    public int Columns = 50;
    public int Rows = 50;
    public bool UsePathfinding = false;   
    public bool UseNeighborHighlight = false;     

    float HexWidth = 0.0f;
    float HexHeight = 0.0f;

    GameObject CrtTile = null;
    GameObject Tile1 = null, Tile2 = null;

    GridGraph GraphNodes;
    GridPathfinder Pathfinder;

    void DoPathfinding ()
    {
        markPath (false);
        GridTile Start = Tile1.GetComponent<GridTile> ();
        GridTile Finish = Tile2.GetComponent<GridTile> ();
        if (Start == null || Finish == null)
            return;

        Pathfinder.Initialize (Start, Finish);
        Pathfinder.FindPath ();
        markPath (true);
    }

    public int getValidIndex (int i, int j)
    {
        int index = GraphNodes.getValidIndex (i, j);
        if (index == -1)
            return -1;

        if (GraphNodes.nodes [index].GetComponent<GridTile> ().Passable == false)
            return -1;

        return index;
    }


    void markNeighbors (GameObject Tile, bool Mark)
    {
        markTile (Tile, Mark);
        GridTile Node = Tile.GetComponent<GridTile> ();
        if (!Node)
            return;

        for (int i = 0; i < Node.Neighbors.Count; i++) {
            markTile (Node.Neighbors [i].gameObject, Mark);
        }
    }

    void markPath (bool Mark)
    {
        if (Pathfinder == null || Pathfinder.Path == null)
            return;

        foreach (GridTile Node in Pathfinder.Path) {
            int index = GraphNodes.getValidIndex (Node.Row, Node.Col);
            if (index != -1)
                markTile (GraphNodes.nodes [index], Mark);
        }
    }

    void markForPathFinding (GameObject Tile)
    {
        if (!Tile1 && !Tile2) { // none selected
            Tile1 = Tile;
            markTile (Tile1, true);
        } else if (!Tile2) { // first selected
            Tile2 = Tile;
            markTile (Tile2, true);
            DoPathfinding ();
        } else { // both selected => new selection
            markPath (false);
            markTile (Tile2, false);
            markTile (Tile1, false);
            Tile2 = null;
            Tile1 = Tile;
            markTile (Tile1, true);
        }
    }

    void checkForClickedTiles ()
    {
        if (Input.GetMouseButtonDown (0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit)) {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.GetComponent<SpriteRenderer> ()) {
                    if (!UsePathfinding && !UseNeighborHighlight) {
                        highlightTile (hitObject);
                    } else if (UseNeighborHighlight) {
                        if (Tile1)
                            markNeighbors (Tile1, false);

                        Tile1 = hitObject;
                        markNeighbors (Tile1, true);
                    } else {
                        markForPathFinding (hitObject);
                    }
                }
            }
        }
    }

    public void markTile (GameObject Tile, bool mark)
    {
        SpriteRenderer sprite = Tile.GetComponent<SpriteRenderer> ();
        if (!sprite)
            return;

        sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, mark ? 0.5f : 1.0f);
    }
  
    public void highlightTile (GameObject Tile)
    {
        if (Tile == CrtTile)
            return;

        SpriteRenderer sprite = Tile.GetComponent<SpriteRenderer> ();
        if (!sprite)
            return;

        GridTile gridTile = Tile.GetComponent<GridTile> ();
        if (!gridTile)
            return;

        // restore alpha
        if (CrtTile) {
            SpriteRenderer currentSprite = CrtTile.GetComponent<SpriteRenderer> ();
            if (currentSprite) {
                currentSprite.color = new Color (currentSprite.color.r, 
                                                 currentSprite.color.g, 
                                                 currentSprite.color.b, 
                                                 1.0f);
            }
        }

        sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, 0.5f); // transparency
        CrtTile = Tile;
    }

    void setSizes ()
    {
        if (!HexTile)
            return;
        // renderer component attached to the Hex prefab is used to get the current width and height
        HexWidth = HexTile.renderer.bounds.size.x;
        HexHeight = HexTile.renderer.bounds.size.y;

        General.minCoords.x = -1 * Camera.main.orthographicSize * Screen.width / Screen.height;
        General.minCoords.y = -1 * Camera.main.orthographicSize;
    }

    float getNextPositionX (float f, ref bool bUseOffset)
    {
        if (General.isZeroF (f)) {
            // shift right 1 tile
            float newX = General.minCoords.x + (bUseOffset ? 0 : HexWidth / 2);
            bUseOffset = false;
            return newX;
        }

        return (f + HexWidth);
    }

    float getNextPositionY (float f)
    {
        if (General.isZeroF (f))
            return General.minCoords.y;

        // return the proper new placement (3/4 tile height)
        return (f + 3 * HexHeight / 4);
    }

    GridTile.TileType getTileTypeByRandPercentage (int value)
    {
        if (value > 90)
            return GridTile.TileType.Rock;
        else if (value > 80)
            return GridTile.TileType.Water;
        else if (value > 40)
            return GridTile.TileType.Sand;
        else
            return GridTile.TileType.Grass;
    }

    void createGrid ()
    {
        GraphNodes = new GridGraph (Rows, Columns);
        Vector2 vPos = new Vector2 (0, 0);
        bool bUseOffset = false;
        int tagCnt = 1;
        for (int y = 0; y < Rows; y++) {
            vPos.y = getNextPositionY (vPos.y);
            vPos.x = 0;
            bUseOffset = (y % 2 != 0) ? true : false;
            for (int x = 0; x < Columns; x++) {
                // GameObject assigned to Hex public variable is cloned
                GameObject Hex = (GameObject)Instantiate (HexTile);
                Hex.name = "Tile" + tagCnt++;

                GridTile Tile = Hex.GetComponent<GridTile> ();
                if (!Tile)
                    continue;

                GridTile.TileType type = getTileTypeByRandPercentage (Random.Range (1, 100));
                Tile.setTileType (type);
                Tile.setCoords (y, x);

                // Current position in grid
                vPos.x = getNextPositionX (vPos.x, ref bUseOffset);
                Hex.transform.position = new Vector3 (vPos.x, vPos.y, 0);

                GraphNodes.nodes.Add (Tile.gameObject);
            }
        }

        GraphNodes.Connect ();
    }

    // Use this for initialization
    void Start ()
    {
        setSizes ();
        createGrid ();
        Pathfinder = new GridPathfinder ();
        Pathfinder.GraphNodes = GraphNodes;
    }

    // Update is called once per frame
    void Update ()
    {
        checkForClickedTiles ();
    }
}
