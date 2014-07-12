using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour
{
    public GameObject HexTile;
    public int HorizontalTiles = 50;
    public int VerticalTiles = 50;
    public bool UsePathfinding = false;

    float fHexWidth;
    float fHexHeight;

    GameObject crtTile = null;
    GameObject Tile1 = null, Tile2 = null;

    void DoPathfinding ()
    {
    }

    void checkForClickedTiles ()
    {
        if (Input.GetMouseButtonDown (0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit)) {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.GetComponent<SpriteRenderer> ()) {
                    if (!UsePathfinding) {
                        highlightTile (hitObject);
                        return;
                    }

                    if (!Tile1 && !Tile2) { // none selected
                        Tile1 = hitObject;
                        markTile (Tile1, true);
                    } else if (!Tile2) { // first selected
                        Tile2 = hitObject;
                        markTile (Tile2, true);
                        DoPathfinding ();
                    } else { // both selected => new selection
                        markTile (Tile2, false);
                        markTile (Tile1, false);
                        Tile2 = null;
                        Tile1 = hitObject;
                        markTile (Tile1, true);
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
        if (Tile == crtTile)
            return;

        SpriteRenderer sprite = Tile.GetComponent<SpriteRenderer> ();
        if (!sprite)
            return;

        GridTile gridTile = Tile.GetComponent<GridTile> ();
        if (!gridTile)
            return;

        // restore alpha
        if (crtTile) {
            SpriteRenderer currentSprite = crtTile.GetComponent<SpriteRenderer> ();
            if (currentSprite) {
                currentSprite.color = new Color (currentSprite.color.r, 
                                                 currentSprite.color.g, 
                                                 currentSprite.color.b, 
                                                 1.0f);
            }
        }

        sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, 0.5f); // transparency
        crtTile = Tile;
    }

    void setSizes ()
    {
        if (!HexTile)
            return;
        // renderer component attached to the Hex prefab is used to get the current width and height
        fHexWidth = HexTile.renderer.bounds.size.x;
        fHexHeight = HexTile.renderer.bounds.size.y;

        General.minCoords.x = -1 * Camera.main.orthographicSize * Screen.width / Screen.height;
        General.minCoords.y = -1 * Camera.main.orthographicSize;
    }

    float getNextPositionX (float f, ref bool bUseOffset)
    {
        if (General.isZeroF (f)) {
            // shift left 1 tile
            float newX = General.minCoords.x - (bUseOffset ? 0 : fHexWidth / 2);
            bUseOffset = false;
            return newX;
        }

        return (f + fHexWidth);
    }

    float getNextPositionY (float f)
    {
        if (General.isZeroF (f))
            return General.minCoords.y;

        // return the proper new placement (3/4 tile height)
        return (f + 3 * fHexHeight / 4);
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
        Vector2 vPos = new Vector2 (0, 0);
        bool bUseOffset = false;
        int tagCnt = 1;
        for (int y = 0; y < VerticalTiles; y++) {
            vPos.y = getNextPositionY (vPos.y);
            vPos.x = 0;
            bUseOffset = (y % 2 != 0) ? true : false;
            for (int x = 0; x < HorizontalTiles; x++) {
                // GameObject assigned to Hex public variable is cloned
                GameObject hex = (GameObject)Instantiate (HexTile);
                hex.name = "Tile" + tagCnt++;

                GridTile gridTile = hex.GetComponent<GridTile> ();
                if (!gridTile)
                    continue;


                GridTile.TileType type = getTileTypeByRandPercentage (Random.Range (1, 100));
                gridTile.setTileType (type);


                // Current position in grid
                vPos.x = getNextPositionX (vPos.x, ref bUseOffset);
                hex.transform.position = new Vector3 (vPos.x, vPos.y, 0);
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        setSizes ();
        createGrid ();
    }

    // Update is called once per frame
    void Update ()
    {
        checkForClickedTiles ();
    }
}
