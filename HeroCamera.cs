using UnityEngine;
using System.Collections;

public class HeroCamera : MonoBehaviour
{
    private float upperLimit = 0.85f;
    private float lowerLimit = 0.15f;

    // Use this for initialization
    void Start ()
    {
	
    }
	
    // Update is called once per frame
    void Update ()
    {
        Vector3 newPos = Camera.main.transform.position;
        Vector3 mousePos = Camera.main.ScreenToViewportPoint (Input.mousePosition);
        if (General.IsLessF (mousePos.x, 0.0f) ||
            General.IsLessF (mousePos.y, 0.0f) ||
            General.IsGreaterF (mousePos.x, 1.0f) ||
            General.IsGreaterF (mousePos.y, 1.0f)) {
            return;
        }

        if (General.IsLessEqF (mousePos.y, lowerLimit)) {
            newPos.y -= 0.1f;
        }

        if (General.IsGreatEqF (mousePos.y, upperLimit)) {
            newPos.y += 0.1f;
        }

        if (General.IsLessEqF (mousePos.x, lowerLimit)) {
            newPos.x -= 0.1f;
        }

        if (General.IsGreatEqF (mousePos.x, upperLimit)) {
            newPos.x += 0.1f;
        }

        Camera.main.transform.position = newPos;
    }
}

