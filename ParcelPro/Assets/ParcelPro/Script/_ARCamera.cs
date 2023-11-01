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
   // public GameObject canvasObject; // Unity Hierarchy에서 Canvas 오브젝트를 연결합니다.
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

                //웹캠 텍스처에서 QR코드 인식
                var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
                if (result != null)
                {
                    Debug.Log("============================================================================================QR 코드: " + result.Text);
                    sound.Play();
                    //QR 코드가 인식되면 다음 씬으로 전환합니다.
                    //if (result.Text.Equals("https://qrco.de/beSUsK")) // 여기에 원하는 QR 코드 데이터를 넣으세요.
                    //{
                    //    SceneManager.LoadScene("Three Boxes"); // 여기에 다음 씬의 이름을 넣으세요.
                    //    Debug.Log("=======================================================================================================다음씬 이동...");
                    //}

                }

                // Canvas를 HMD 위치에 고정
              //  canvasObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.0f; // 텍스트를 HMD에서 약간 떨어진 위치에 표시
               // canvasObject.transform.LookAt(Camera.main.transform); // 텍스트가 항상 HMD를 향하도록 설정
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
