using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;   // Permission 클래스를 사용
using UnityEditor;
using UnityEngine.UIElements;
using System;
using ZXing;    //ZXing.dll 임포트 후 선언.
using ZXing.QrCode;
using UnityEngine.SceneManagement;
//이 코드는 Unity에서 웹캠을 통해 QR 코드를 감지하고 해당 QR 코드 주변에 빨간색 상자를 그리는 기능을 수행합니다

public class ARCamera : MonoBehaviour
{
    WebCamTexture camTexture;
    public RawImage cameraViewImage;  //웹캠 영상을 표시할 RawImage
    public AudioSource sound;  //QR코드 인식 성공시 재생할 사운드


    //Start함수에서 권한을 확인하고 카메라를 켜는 함수를 호출
    private void Start()
    {
        CameraOn();
    }

    //Update함수에서 웹캠 텍스처가 업데이트 되었는지 확인하고 QR코드를 인식
    private void Update()
    {
        //웹캠 텍스처가 업데이트 되었는지 확인
        if (camTexture != null && camTexture.didUpdateThisFrame)

        {
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();

                //웹캠 텍스처에서 QR코드 인식
                var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
                if (result != null)
                {
                    Debug.Log("=====================================================================================QR : " + result.Text);
                    sound.Play();

                    // QR 코드가 인식되면 다음 씬으로 전환합니다.
                    if (result.Text.Equals("https://qrco.de/beSUsK")) // 여기에 원하는 QR 코드 데이터를 넣으세요.
                    {
                        SceneManager.LoadScene("main"); // 여기에 다음 씬의 이름을 넣으세요.
                        Debug.Log("=======================================================================================================다음씬 이동...");
                    }

                }
            }

            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }

    //카메라 켜는 함수
    public void CameraOn()
    {
        //사용자 권한이 있는지 확인하고 권한을 요청
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        //연결된 카메라가 있는지 확인
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("no camera!");
            return;
        }

        //사용 가능한 카메라 목록에서 후면 카메라 선택
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

        //선택된 카메라 인덱스가 0이상인 경우 카메라 활성화
        if (selectedCameraIndex >= 0)
        {
            camTexture = new WebCamTexture(devices[selectedCameraIndex].name); //선택하 카메라로 WebcamTexture 생성
            camTexture.requestedFPS = 30; //요청된 프레임 속도 설정
            cameraViewImage.texture = camTexture; //RawImage에 웹갬 텍스처 할당
            camTexture.Play(); //카메라 시작

            //웹갬 이미지 크게에 따라 RawImage 크기 조정
            cameraViewImage.rectTransform.sizeDelta = new Vector2(camTexture.width, camTexture.height);
        }
    }

    //카메라 끄는 함수
    public void CameraOff()
    {
        if (camTexture != null)
        {
            camTexture.Stop(); //카메라 중지
            WebCamTexture.Destroy(camTexture); //웹캡 텍스처 제거
            camTexture = null; //변수 초기화
        }
    }
}