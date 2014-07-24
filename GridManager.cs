using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroGridManager : MonoBehaviour
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

    HeroGridGraph GraphNodes;
    HeroGridPathfinder Pathfinder;

    void DoPathfinding ()
    {
        markPath (false);
        HeroGridTile Start = Tile1.GetComponent<HeroGridTile> ();
        HeroGridTile Finish = Tile2.GetComponent<HeroGridTile> ();
        if (Start == null || Finish == null)
            return;

        Pathfinder.Initialize (Start, Finish);
        Pathfinder.FindPath ();
        markPath (true);
    }

    public int GetValidIndex (int i, int j)
    {
        int Index = GraphNodes.GetValidIndex (i, j);
        if (Index == -1)
            return -1;

        if (GraphNodes.Nodes [Index].GetComponent<HeroGridTile> ().Passable == false)
            return -1;

        return Index;
    }


    void markNeighbors (GameObject Tile, bool Mark)
    {
        markTile (Tile, Mark);
        HeroGridTile Node = Tile.GetComponent<HeroGridTile> ();
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

        foreach (HeroGridTile Node in Pathfinder.Path) {
            int Index = GraphNodes.GetValidIndex (Node.Row, Node.Col);
            if (Index != -1)
                markTile (GraphNodes.Nodes [Index], Mark);
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

        HeroGridTile gridTile = Tile.GetComponent<HeroGridTile> ();
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

    float GetNextPositionX (float f, ref bool bUseOffset)
    {
        if (General.IsZeroF (f)) {
            // shift right 1 tile
            float newX = General.minCoords.x + (bUseOffset ? 0 : HexWidth / 2);
            bUseOffset = false;
            return newX;
        }

        return (f + HexWidth);
    }

    float GetNextPositionY (float f)
    {
        if (General.IsZeroF (f))
            return General.minCoords.y;

        // return the proper new placement (3/4 tile height)
        return (f + 3 * HexHeight / 4);
    }

    HeroGridTile.TileType GetTileTypeByRandPercentage (int Value)
    {
        if (Value > 90)
            return HeroGridTile.TileType.Rock;
        else if (Value > 80)
            return HeroGridTile.TileType.Water;
        else if (Value > 40)
            return HeroGridTile.TileType.Sand;
        else
            return HeroGridTile.TileType.Grass;
    }

    void createGrid ()
    {
        GraphNodes = new HeroGridGraph (Rows, Columns);
        Vector2 vPos = new Vector2 (0, 0);
        bool bUseOffset = false;
        int tagCnt = 1;
        for (int y = 0; y < Rows; y++) {
            vPos.y = GetNextPositionY (vPos.y);
            vPos.x = 0;
            bUseOffset = (y % 2 != 0) ? true : false;
            for (int x = 0; x < Columns; x++) {
                // GameObject assigned to Hex public variable is cloned
                GameObject Hex = (GameObject)Instantiate (HexTile);
                Hex.name = "Tile" + tagCnt++;

                HeroGridTile Tile = Hex.GetComponent<HeroGridTile> ();
                if (!Tile)
                    continue;

                HeroGridTile.TileType type = GetTileTypeByRandPercentage (Random.Range (1, 100));
                Tile.SetTileType (type);
                Tile.Row = y;
                Tile.Col = x;

                // Current position in grid
                vPos.x = GetNextPositionX (vPos.x, ref bUseOffset);
                Hex.transform.position = new Vector3 (vPos.x, vPos.y, 0);

                GraphNodes.Nodes.Add (Tile.gameObject);
            }
        }

        GraphNodes.Connect ();
    }

    // Use this for initialization
    void Start ()
    {
        setSizes ();
        createGrid ();
        Pathfinder = new HeroGridPathfinder ();
        Pathfinder.GraphNodes = GraphNodes;
    }

    // Update is called once per frame
    void Update ()
    {
        checkForClickedTiles ();
    }
}
