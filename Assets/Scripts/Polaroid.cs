using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class Polaroid : MonoBehaviour
{
    [SerializeField] GameObject photo3D;
    [SerializeField] Material photoMat;
    [SerializeField] RenderTexture renderTexture;
    private Texture2D screenCapture;

    [Header("Polaroid Anim")]
    [SerializeField] GameObject polaroidModel;
    [SerializeField] Transform initialPosition;
    [SerializeField] Transform targetPosition;
    [SerializeField] float lerpSpeed = 5f;
    [SerializeField] float fovLerpSpeed = 5f;
    [SerializeField] float initialFOV = 60f;
    [SerializeField] float targetFOV = 10f;


    [HideInInspector]public UnityEvent onPlayerAction;
    void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    // Update is called once per frame
    void Update()
    {
        //print(Vector3.Distance(polaroidModel.transform.position, targetPosition.position));
        //Lerp polaroid
        if (Input.GetMouseButton(1))
        {
            // Lerp towards the target position
            polaroidModel.transform.position = Vector3.Lerp(polaroidModel.transform.position, targetPosition.position, lerpSpeed * Time.deltaTime);
            polaroidModel.transform.rotation = Quaternion.Lerp(polaroidModel.transform.rotation, targetPosition.rotation, lerpSpeed * Time.deltaTime);
            

            float distanceToTarget = Vector3.Distance(polaroidModel.transform.position, targetPosition.position);
            if (distanceToTarget <= 0.1) Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, fovLerpSpeed * Time.deltaTime);

            if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(CapturePhoto());
                    if (onPlayerAction != null)
                    {
                        onPlayerAction.Invoke();
                    }
                }
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, initialFOV, fovLerpSpeed * Time.deltaTime);
            if (Camera.main.fieldOfView >= 50)
            {
                polaroidModel.transform.position = Vector3.Lerp(polaroidModel.transform.position, initialPosition.position, lerpSpeed * Time.deltaTime);
                polaroidModel.transform.rotation = Quaternion.Lerp(polaroidModel.transform.rotation, initialPosition.rotation, lerpSpeed * Time.deltaTime);
            }

        }
    }

    IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture.active = renderTexture;

        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenCapture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenCapture.Apply();

        RenderTexture.active = null;

        ShowPhoto();
    }

    void ShowPhoto()
    {
        GameObject newPhoto = Instantiate(photo3D,polaroidModel.transform.position,Quaternion.identity);
        Material newMaterial = new Material(photoMat); // Use the standard shader or any shader of your choice
        newMaterial.mainTexture = screenCapture;

        newPhoto.GetComponent<Renderer>().material = newMaterial;
    }
}
