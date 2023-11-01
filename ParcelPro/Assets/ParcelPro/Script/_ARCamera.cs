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
    public Renderer cameraRenderer;  // ��ķ ������ ǥ���� Renderer
    public Vector3 rotationOffset = new Vector3(0, 0, 180);
    public AudioSource sound;
   // public GameObject canvasObject; // Unity Hierarchy���� Canvas ������Ʈ�� �����մϴ�.
   // public Text displayText;

    private void Start()
    {
        CameraOn();
       // displayText.text = "QR Reader";
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
                    sound.Play();
                    //QR �ڵ尡 �νĵǸ� ���� ������ ��ȯ�մϴ�.
                    //if (result.Text.Equals("https://qrco.de/beSUsK")) // ���⿡ ���ϴ� QR �ڵ� �����͸� ��������.
                    //{
                    //    SceneManager.LoadScene("Three Boxes"); // ���⿡ ���� ���� �̸��� ��������.
                    //    Debug.Log("=======================================================================================================������ �̵�...");
                    //}

                }

                // Canvas�� HMD ��ġ�� ����
              //  canvasObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.0f; // �ؽ�Ʈ�� HMD���� �ణ ������ ��ġ�� ǥ��
               // canvasObject.transform.LookAt(Camera.main.transform); // �ؽ�Ʈ�� �׻� HMD�� ���ϵ��� ����
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
