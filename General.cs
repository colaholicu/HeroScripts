using UnityEngine;
using System.Collections;

public class General : MonoBehaviour
{

    static public Vector2 minCoords;

    static public bool isZeroF(float f)
    {
        return (Mathf.Abs(f) <= Mathf.Epsilon);
    }

    // Use this for initialization
    void Start()
    {
        minCoords = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
