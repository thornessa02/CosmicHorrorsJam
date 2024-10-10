using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] float fadeDistanceThreshold = 1f;
    [SerializeField] Image fadeImage;         
    [SerializeField] GameObject hudImage;         
    [SerializeField] AnimationCurve fadeCurve;
    float currentFadeTime = 0f;
    bool isFading = false;
    void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    // Update is called once per frame
    void Update()
    {
        //Lerp polaroid
        if (Input.GetMouseButton(1))
        {
            // Lerp towards the target position
            polaroidModel.transform.position = Vector3.Lerp(polaroidModel.transform.position, targetPosition.position, lerpSpeed * Time.deltaTime);
            polaroidModel.transform.rotation = Quaternion.Lerp(polaroidModel.transform.rotation, targetPosition.rotation, lerpSpeed * Time.deltaTime);

            float distanceToTarget = Vector3.Distance(polaroidModel.transform.position, targetPosition.position);
            if (distanceToTarget <= fadeDistanceThreshold)
            {
                currentFadeTime = Mathf.Clamp01(currentFadeTime + Time.deltaTime);
                float fadeValue = fadeCurve.Evaluate(currentFadeTime); // Use curve to represent the alpha directly
                if (currentFadeTime >= 0.49f) 
                {
                    polaroidModel.SetActive(false);
                    hudImage.SetActive(true);
                } 
                // Apply the fade (alpha) value from the curve
                Color color = fadeImage.color;
                color.a = fadeValue;
                fadeImage.color = color;

                isFading = true; // We're currently fading in

                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(CapturePhoto());
                }
            }
        }
        else
        {
            hudImage.SetActive(false);
            polaroidModel.SetActive(true);
            if (isFading)
            {
                // When the button is released, lerp the alpha back using the same curve but in reverse
                currentFadeTime = Mathf.Clamp01(currentFadeTime - Time.deltaTime); // Move time backward along the curve
                float fadeValue = fadeCurve.Evaluate(currentFadeTime); // Evaluate curve for fade-out

                // Apply the fade (alpha) value from the curve
                Color color = fadeImage.color;
                color.a = fadeValue;
                fadeImage.color = color;

                // If the fade has fully returned, stop fading
                
                if (currentFadeTime <= 0f)
                {
                    isFading = false;
                }
            }
            // Lerp back to the initial position when the button is released
            polaroidModel.transform.position = Vector3.Lerp(polaroidModel.transform.position, initialPosition.position, lerpSpeed * Time.deltaTime);
            polaroidModel.transform.rotation = Quaternion.Lerp(polaroidModel.transform.rotation, initialPosition.rotation, lerpSpeed * Time.deltaTime);
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
