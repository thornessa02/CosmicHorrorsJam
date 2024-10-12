using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;

    [Header("Camera")]
    [SerializeField] float mouseSensitivityX;
    [SerializeField] float mouseSensitivityY;
    [SerializeField] float camAngleLimit;
    [SerializeField] GameObject camera;

    private float xRotation = 0f;

    [Header("Inspect")]
    [SerializeField] float inspectDistance = 100f;  // The distance the ray will travel
    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject itemPos;
    Vector3 initialPos;
    Quaternion initialRot;
    bool isInspecting;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInspecting)
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
            Vector3 movement = (transform.forward * verticalMovement) + (transform.right * horizontalMovement);

            transform.position += movement.normalized * speed * Time.deltaTime;

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + mouseX, 0);

            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -camAngleLimit, camAngleLimit);
            camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, inspectDistance, layerMask))
            {
                if (hit.collider.gameObject.GetComponent<ItemInspector>() != null) 
                {
                    isInspecting = true;

                    initialPos = hit.collider.transform.position;
                    initialRot = hit.collider.transform.rotation;
                    hit.collider.transform.parent = itemPos.transform;
                    hit.collider.transform.localPosition = Vector3.zero;
                    
                    hit.collider.gameObject.layer = LayerMask.NameToLayer("Inspected");
                    hit.collider.gameObject.GetComponent<ItemInspector>().enabled = true;
                }
            }
        }

        if (isInspecting)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameObject inspectedObj = itemPos.transform.GetChild(0).gameObject;
                inspectedObj.transform.parent = null;
                inspectedObj.transform.position = initialPos;
                inspectedObj.transform.rotation = initialRot;
                inspectedObj.layer = LayerMask.NameToLayer("Default");
                inspectedObj.GetComponent<ItemInspector>().enabled = false;
                isInspecting = false;
            }
        }
    }
}
