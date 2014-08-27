using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroGridTile : MonoBehaviour
{
    public bool Passable { get; internal set; }
    public int Row, Col;
    public int f = 0, g = 0, h = 0;
    public HeroGridTile Parent;
    TileType Type = TileType.Building;

    public List<HeroGridTile> Neighbors;

    public enum TileType
    {
        Grass = 1,
        Sand = 2,
        Rock = 3,
        Water = 4,
        Building = 5,
    };

    public Color DefaultColor = Color.white;

    public void CalcF (HeroGridTile _Node)
    {           
        int X = Row - _Node.Row;
        int Y = Col - _Node.Col;
        int Z = (0 - (Row + Col)) - (0 - (_Node.Row + _Node.Col));
        h = (int)(System.Math.Sqrt (X * X) + System.Math.Sqrt (Y * Y) + System.Math.Sqrt (Z * Z)) / 2;
        f = g + h;
    }

    public int Distance (HeroGridTile _Node)
    {
        int X = Row - _Node.Row;
        int Y = Col - _Node.Col;
        int Z = (0 - (Row + Col)) - (0 - (_Node.Row + _Node.Col));
        return (int)(System.Math.Sqrt (X * X) + System.Math.Sqrt (Y * Y) + System.Math.Sqrt (Z * Z)) / 2;
    }

    public TileType GetTileType ()
    {
        return Type;
    }

    public void SetTileType (TileType _Type)
    {
        Passable = false;

        switch (_Type) {
        case TileType.Grass:
            DefaultColor = new Color (0.0f, 0.7f, 0.0f, 1.0f);
            Passable = true;
            break;
        case TileType.Sand:
            DefaultColor = new Color (0.7f, 0.7f, 0.0f, 1.0f);
            Passable = true;
            break;
        case TileType.Rock:
            DefaultColor = new Color (0.7f, 0.7f, 0.7f, 1.0f);
            break;
        case TileType.Water:
            DefaultColor = new Color (0.0f, 0.0f, 0.7f, 1.0f);
            break;
        default:
            break;

        }

        SpriteRenderer SpriteComponent = GetComponent<SpriteRenderer> ();
        if (SpriteComponent) {
            SpriteComponent.color = DefaultColor;
        }
        Type = _Type;
    }

    public void AddNeighbor (HeroGridTile _Node)
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
