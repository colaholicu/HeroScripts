using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridTile : MonoBehaviour
{
    public bool Passable { get; internal set; }
    public int Row, Col;
    public int f = 0, g = 0, h = 0;
    public GridTile Parent;
    TileType tileType = TileType.Building;

    public List<GridTile> Neighbors;

    public enum TileType
    {
        Grass = 1,
        Sand = 2,
        Rock = 3,
        Water = 4,
        Building = 5,
    };

    public Color defaultColor = Color.white;

    public void calcF (GridTile _Node)
    {           
        int X = Row - _Node.Row;
        int Y = Col - _Node.Col;
        int Z = (0 - (Row + Col)) - (0 - (_Node.Row + _Node.Col));
        h = (int)(System.Math.Sqrt (X * X) + System.Math.Sqrt (Y * Y) + System.Math.Sqrt (Z * Z)) / 2;
        f = g + h;
    }

    public int distance (GridTile _Node)
    {
        int X = Row - _Node.Row;
        int Y = Col - _Node.Col;
        int Z = (0 - (Row + Col)) - (0 - (_Node.Row + _Node.Col));
        return (int)(System.Math.Sqrt (X * X) + System.Math.Sqrt (Y * Y) + System.Math.Sqrt (Z * Z)) / 2;
    }
    
    public void setCoords (int _Row, int _Col)
    {
        Row = _Row;
        Col = _Col;
    }

    public TileType getTileType ()
    {
        return tileType;
    }

    public void setTileType (TileType type)
    {
        Passable = false;

        switch (type) {
        case TileType.Grass:
            defaultColor = new Color (0.0f, 0.7f, 0.0f, 1.0f);
            Passable = true;
            break;
        case TileType.Sand:
            defaultColor = new Color (0.7f, 0.7f, 0.0f, 1.0f);
            Passable = true;
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

    public void addNeighbor (GridTile _Node)
    {
        if (!Passable || !_Node.Passable)
            return;

        Neighbors.Add (_Node);
        _Node.Neighbors.Add (this);
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
