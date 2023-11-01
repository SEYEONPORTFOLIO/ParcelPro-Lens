using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using ZXing;
using TMPro;
using UnityEngine.UI;
public class _ARCamera : MonoBehaviour
{
    WebCamTexture camTexture;
    public Renderer cameraRenderer;  // 웹캠 영상을 표시할 Renderer
    public Vector3 rotationOffset = new Vector3(0, 0, 180);
    public AudioSource sound;
    private bool soundPlayed = false;

    private void Start()
    {
        CameraOn();
     
    }

    private void Update()
    {
        if (Input.GetAxisRaw("B") == 1)
        {
            SceneManager.LoadScene("next");
        }
        if (camTexture != null && camTexture.didUpdateThisFrame)
        {
            cameraRenderer.transform.rotation = Quaternion.Euler(rotationOffset);

            // Update the camera texture
            cameraRenderer.material.mainTexture = camTexture;
            // Add your QR code detection logic here
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();

                //웹캠 텍스처에서 QR코드 인식
                var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
                if (result != null)
                {
                    Debug.Log("============================================================================================QR 코드: " + result.Text);

                    if (!soundPlayed)
                    {
                        sound.Play();
                        soundPlayed = true;
                    }

                }                
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        
        }
    }

    public void CameraOn()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("No camera!");
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        int selectedCameraIndex = -1;

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                selectedCameraIndex = i;
                break;
            }
        }

        if (selectedCameraIndex >= 0)
        {
            camTexture = new WebCamTexture(devices[selectedCameraIndex].name);
            camTexture.requestedFPS = 30;
            cameraRenderer.material.mainTexture = camTexture;
            camTexture.Play();
        }
    }

    public void CameraOff()
    {
        if (camTexture != null)
        {
            camTexture.Stop();
            WebCamTexture.Destroy(camTexture);
            camTexture = null;
        }
    }
}
