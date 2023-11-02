using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using ZXing;
using TMPro;
using UnityEngine.UI;
using Pnc.Model;
using TMPro;


public class _ARCamera : MonoBehaviour
{
    WebCamTexture camTexture;
    public Renderer cameraRenderer;  // ��ķ ������ ǥ���� Renderer
    public Vector3 rotationOffset = new Vector3(0, 0, 180);
    public AudioSource sound;
    private bool soundPlayed = false;
    public TextMeshProUGUI AddressNum;
    private string OldText = null;
    public Image Addressbackground;


    private void Start()
    {
        CameraOn();
    }

    private void Update()
    {
        if (camTexture != null && camTexture.didUpdateThisFrame)
        {
            cameraRenderer.transform.rotation = Quaternion.Euler(rotationOffset);

            // Update the camera texture
            cameraRenderer.material.mainTexture = camTexture;
            // Add your QR code detection logic here
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();

                //��ķ �ؽ�ó���� QR�ڵ� �ν�
                var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
                if (result != null)
                {

                    Debug.Log("============================================================================================QR �ڵ�: " + result.Text);
                    if (OldText != result.Text)
                    {
                        sound.Play();
                        soundPlayed = true;
                    }
                    if (result.Text.Equals("17C03"))
                    {
                        GameObject.Find("Outline_Img").GetComponent<Image>().color = new Color(162 / 255f, 0 / 255f, 0 / 255f);
                        GameObject.Find("AddressBG_Img").GetComponent<Image>().color = new Color(162 / 255f, 0 / 255f, 0 / 255f);
                    }


                    else if (result.Text.Equals("17C01") || result.Text.Equals("17C02"))
                    {
                        GameObject.Find("Outline_Img").GetComponent<Image>().color = new Color(135 / 255f, 135 / 255f, 135 / 255f);
                        GameObject.Find("AddressBG_Img").GetComponent<Image>().color = new Color(135 / 255f, 135 / 255f, 135 / 255f);
                    }
                    Addressbackground.gameObject.SetActive(true);
                    AddressNum.text = result.Text;
                    OldText = result.Text;

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

