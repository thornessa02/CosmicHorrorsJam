using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Polaroid : MonoBehaviour
{
    public Renderer photo3D;
    public RenderTexture renderTexture;
    private Texture2D screenCapture;
    void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) 
        {
            StartCoroutine(CapturePhoto());
        }
    }

    IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture.active = renderTexture;

        screenCapture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenCapture.Apply();

        RenderTexture.active = null;

        ShowPhoto();
    }

    void ShowPhoto()
    {
        Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")); // Use the standard shader or any shader of your choice
        newMaterial.mainTexture = screenCapture;

        photo3D.material = newMaterial;
    }
}
