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

                    DrawQRCodeFrame(result.ResultPoints);
                }
            }

            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }

    //QR코드 테두리에 빨간색 박스 그리는 함수
    private void DrawQRCodeFrame(ResultPoint[] corners)
    {
        Vector3[] vertices = new Vector3[corners.Length];

        //QR코드의 각 꼭지점을 배열로 저장
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(corners[i].X, corners[i].Y, 0);
        }

        //배열의 첫번째 꼭지점을 배열의 마지막에 추가
        int[] triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        //메시 생성
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        //메시 오브젝트 생성
        if (meshObject == null)
        {
            meshObject = new GameObject("QRCodeMesh");
            meshObject.transform.parent = transform;
            meshObject.transform.position = Vector3.zero;
            meshObject.transform.rotation = Quaternion.identity;

            //메시 렌더러 생성
            if (material == null)
            {
                material = new Material(Shader.Find("Standard"));
                material.color = Color.red;
            }

            // 메시 필터와 메시 렌더러 컴포넌트 추가
            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
        }
        else
        {
            // 기존 오브젝트가 있는 경우 메시와 메시 렌더러 업데이트
            MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
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
