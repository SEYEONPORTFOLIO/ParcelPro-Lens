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
//이 코드는 Unity에서 웹캠을 통해 QR 코드를 감지하고 해당 QR 코드 주변에 빨간색 상자를 그리는 기능을 수행합니다

public class ARCamera : MonoBehaviour
{
    WebCamTexture camTexture;
    public RawImage cameraViewImage;  //웹캠 영상을 표시할 RawImage
    public AudioSource sound;  //QR코드 인식 성공시 재생할 사운드
    GameObject meshObject; //메시 오브젝트
    Material material; //빨간색 머터리얼

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
                    Debug.Log("QR 코드: " + result.Text);
                    sound.Play();

                    //QR코드 테두리에 빨간색 박스 그리기
                    //QR코드의 각 꼭지점을 배열로 저장
                    ResultPoint[] point = result.ResultPoints;
                    DrawQRCodeFrame(point);
                }
            }

            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }


    // QR 코드 주변에 빨간색 상자를 그리는 함수
    private void DrawQRCodeFrame(ResultPoint[] corners)
    {
        if (corners.Length < 4)
        {
            Debug.LogWarning("QR 코드를 그릴 수 있는 충분한 꼭지점이 없습니다.");
            return;
        }

        if (meshObject == null)
        {
            meshObject = new GameObject("QRCodeMesh");
            meshObject.transform.SetParent(transform);
            meshObject.transform.position = Vector3.zero;
            meshObject.transform.rotation = Quaternion.identity;

            if (material == null)
            {
                material = new Material(Shader.Find("Standard"));
                material.color = Color.red;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
            new Vector3(corners[0].X, corners[0].Y, 0),
            new Vector3(corners[1].X, corners[1].Y, 0),
            new Vector3(corners[2].X, corners[2].Y, 0),
            new Vector3(corners[3].X, corners[3].Y, 0),
            };

            mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
        }
        else
        {
            Debug.LogWarning("이미 QR 코드를 그리는 오브젝트가 있습니다. 업데이트하지 않았습니다.");
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

        if (meshObject != null)
        {
            Destroy(meshObject);
            meshObject = null;
        }
    }
}