using UnityEngine;
using System.Collections;

public class GridTile : MonoBehaviour
{
    bool isPassable = false;
    TileType tileType = TileType.Building;

    public enum TileType
    {
        Grass = 1,
        Sand = 2,
        Rock = 3,
        Water = 4,
        Building = 5,
    };   

    public Color defaultColor = Color.white;

    public TileType getTileType ()
    {
        return tileType;
    }

    public void setTileType (TileType type)
    {
        isPassable = false;

        switch (type) {
        case TileType.Grass:
            defaultColor = new Color (0.0f, 0.7f, 0.0f, 1.0f);
            isPassable = true;
            break;
        case TileType.Sand:
            defaultColor = new Color (0.7f, 0.7f, 0.0f, 1.0f);
            isPassable = false;
            break;
        case TileType.Rock:
            defaultColor = new Color (0.7f, 0.7f, 0.7f, 1.0f);
            break;
        case TileType.Water:
            defaultColor = new Color (0.0f, 0.0f, 0.7f, 1.0f);
            break;
        default:
            break;

        }

        SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
        if (sprite) {
            sprite.color = defaultColor;
        }
        tileType = type;
    }

    // Use this for initialization
    void Start ()
    {
        //setTileType(TileType.Building);
    }

    // Update is called once per frame
    void Update ()
    {

    }
}
