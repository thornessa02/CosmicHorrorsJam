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
    [SerializeField] Camera inspectCam;
    [SerializeField] Transform initialPosition;
    [SerializeField] Transform targetPosition;
    [SerializeField] float lerpSpeed = 5f;
    [SerializeField] float fovLerpSpeed = 5f;
    [SerializeField] float initialFOV = 60f;
    [SerializeField] float targetFOV = 10f;

    [Header("Photo Anim")]
    [SerializeField] Transform leftHand_PhotoPos;

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
            if (distanceToTarget <= 0.1) inspectCam.fieldOfView = Mathf.Lerp(inspectCam.fieldOfView, targetFOV, fovLerpSpeed * Time.deltaTime);

            if (Input.GetMouseButtonDown(0) && distanceToTarget <= 0.02)
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
            inspectCam.fieldOfView = Mathf.Lerp(inspectCam.fieldOfView, initialFOV, fovLerpSpeed * Time.deltaTime);
            if (inspectCam.fieldOfView >= 50)
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
        if (leftHand_PhotoPos.childCount > 0) Destroy(leftHand_PhotoPos.GetChild(0).gameObject); 

        GameObject newPhoto = Instantiate(photo3D, Vector3.zero, Quaternion.identity,leftHand_PhotoPos);
        newPhoto.transform.localPosition = Vector3.zero;
        newPhoto.transform.localRotation = Quaternion.Euler(0,180,0);
        Material newMaterial = new Material(photoMat); // Use the standard shader or any shader of your choice
        newMaterial.mainTexture = screenCapture;

        newPhoto.GetComponent<Renderer>().material = newMaterial;
    }
}
