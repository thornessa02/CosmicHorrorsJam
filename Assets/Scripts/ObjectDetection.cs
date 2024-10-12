using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetection : MonoBehaviour
{
    void Start()
    {
        // Find the player and register to its action event
        Polaroid polaroid = FindObjectOfType<Polaroid>();
        if (polaroid != null)
        {
            // Subscribe to the player's action event
            polaroid.onPlayerAction.AddListener(OnPlayerAction);
        }
    }

    void OnPlayerAction()
    {
        if (gameObject.GetComponent<Renderer>().isVisible)
        Debug.Log("Red Cube");
    }
}
