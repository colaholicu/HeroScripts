using UnityEngine;
using System.Collections;

public class General : MonoBehaviour
{

    static public Vector2 minCoords;

    static public bool IsZeroF (float f)
    {
        return (Mathf.Abs (f) <= Mathf.Epsilon);
    }

    static public bool IsEqualF (float a, float b)
    {
        return IsZeroF (a - b);
    }

    static public bool IsGreaterF (float a, float b)
    {
        return ((a * a - b * b) > (Mathf.Epsilon * Mathf.Epsilon));
    }

    static public bool IsLessF (float a, float b)
    {
        return ((a * a - b * b) < (Mathf.Epsilon * Mathf.Epsilon));
    }

    static public bool IsGreatEqF (float a, float b)
    {
        return ((a * a - b * b) > (Mathf.Epsilon * Mathf.Epsilon) || IsEqualF (a, b));
    }

    static public bool IsLessEqF (float a, float b)
    {
        return ((a * a - b * b) < (Mathf.Epsilon * Mathf.Epsilon) || IsEqualF (a, b));
    }

    // Use this for initialization
    void Start ()
    {
        minCoords = new Vector2 (0, 0);
    }

    // Update is called once per frame
    void Update ()
    {

    }
}
