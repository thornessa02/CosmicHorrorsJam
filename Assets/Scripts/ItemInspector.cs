using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInspector : MonoBehaviour
{
    public float rotationSpeed = 100f;  

    private bool isInspecting = false;   
    private Vector3 previousMousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isInspecting = true;
            previousMousePosition = Input.mousePosition; 
        }

        if (Input.GetMouseButtonUp(0))
        {
            isInspecting = false;
        }

        if (isInspecting)
        {
            RotateObjectWithMouse();
        }
    }

    void RotateObjectWithMouse()
    {
        // Apply rotation based on mouse movement
        float rotationX = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        float rotationY = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime; // Inverted to rotate as expected

        // Apply the rotation to the object's transform
        transform.Rotate(Vector3.up, rotationY, Space.World); // Rotate around the Y-axis (horizontal)
        transform.Rotate(Vector3.right, rotationX, Space.World); // Rotate around the X-axis (vertical)

        previousMousePosition = Input.mousePosition; // Update previous mouse position
    }
}
