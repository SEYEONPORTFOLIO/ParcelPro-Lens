using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;   // Permission Ŭ������ ���
using UnityEditor;
using UnityEngine.UIElements;
using System;
using ZXing;    //ZXing.dll ����Ʈ �� ����.
using ZXing.QrCode;
//�� �ڵ�� Unity���� ��ķ�� ���� QR �ڵ带 �����ϰ� �ش� QR �ڵ� �ֺ��� ������ ���ڸ� �׸��� ����� �����մϴ�

public class ARCamera : MonoBehaviour
{
    WebCamTexture camTexture;
    public RawImage cameraViewImage;  //��ķ ������ ǥ���� RawImage
    public AudioSource sound;  //QR�ڵ� �ν� ������ ����� ����
    GameObject meshObject; //�޽� ������Ʈ
    Material material; //������ ���͸���

    //Start�Լ����� ������ Ȯ���ϰ� ī�޶� �Ѵ� �Լ��� ȣ��
    private void Start()
    {
        CameraOn();
    }

    //Update�Լ����� ��ķ �ؽ�ó�� ������Ʈ �Ǿ����� Ȯ���ϰ� QR�ڵ带 �ν�
    private void Update()
    {
        //��ķ �ؽ�ó�� ������Ʈ �Ǿ����� Ȯ��
        if (camTexture != null && camTexture.didUpdateThisFrame)

        {
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();

                //��ķ �ؽ�ó���� QR�ڵ� �ν�
                var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
                if (result != null)
                {
                    Debug.Log("QR �ڵ�: " + result.Text);
                    sound.Play();

                    //QR�ڵ� �׵θ��� ������ �ڽ� �׸���
                    //QR�ڵ��� �� �������� �迭�� ����
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


    // QR �ڵ� �ֺ��� ������ ���ڸ� �׸��� �Լ�
    private void DrawQRCodeFrame(ResultPoint[] corners)
    {
        if (corners.Length < 4)
        {
            Debug.LogWarning("QR �ڵ带 �׸� �� �ִ� ����� �������� �����ϴ�.");
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
            Debug.LogWarning("�̹� QR �ڵ带 �׸��� ������Ʈ�� �ֽ��ϴ�. ������Ʈ���� �ʾҽ��ϴ�.");
        }
    }


    //ī�޶� �Ѵ� �Լ�
    public void CameraOn()
    {
        //����� ������ �ִ��� Ȯ���ϰ� ������ ��û
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        //����� ī�޶� �ִ��� Ȯ��
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("no camera!");
            return;
        }

        //��� ������ ī�޶� ��Ͽ��� �ĸ� ī�޶� ����
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

        //���õ� ī�޶� �ε����� 0�̻��� ��� ī�޶� Ȱ��ȭ
        if (selectedCameraIndex >= 0)
        {
            camTexture = new WebCamTexture(devices[selectedCameraIndex].name); //������ ī�޶�� WebcamTexture ����
            camTexture.requestedFPS = 30; //��û�� ������ �ӵ� ����
            cameraViewImage.texture = camTexture; //RawImage�� ���� �ؽ�ó �Ҵ�
            camTexture.Play(); //ī�޶� ����

            //���� �̹��� ũ�Կ� ���� RawImage ũ�� ����
            cameraViewImage.rectTransform.sizeDelta = new Vector2(camTexture.width, camTexture.height);
        }
    }

    //ī�޶� ���� �Լ�
    public void CameraOff()
    {
        if (camTexture != null)
        {
            camTexture.Stop(); //ī�޶� ����
            WebCamTexture.Destroy(camTexture); //��ĸ �ؽ�ó ����
            camTexture = null; //���� �ʱ�ȭ
        }

        if (meshObject != null)
        {
            Destroy(meshObject);
            meshObject = null;
        }
    }
}