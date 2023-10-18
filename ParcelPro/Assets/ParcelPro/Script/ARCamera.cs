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

                    DrawQRCodeFrame(result.ResultPoints);
                }
            }

            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }

    //QR�ڵ� �׵θ��� ������ �ڽ� �׸��� �Լ�
    private void DrawQRCodeFrame(ResultPoint[] corners)
    {
        Vector3[] vertices = new Vector3[corners.Length];

        //QR�ڵ��� �� �������� �迭�� ����
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(corners[i].X, corners[i].Y, 0);
        }

        //�迭�� ù��° �������� �迭�� �������� �߰�
        int[] triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        //�޽� ����
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        //�޽� ������Ʈ ����
        if (meshObject == null)
        {
            meshObject = new GameObject("QRCodeMesh");
            meshObject.transform.parent = transform;
            meshObject.transform.position = Vector3.zero;
            meshObject.transform.rotation = Quaternion.identity;

            //�޽� ������ ����
            if (material == null)
            {
                material = new Material(Shader.Find("Standard"));
                material.color = Color.red;
            }

            // �޽� ���Ϳ� �޽� ������ ������Ʈ �߰�
            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
        }
        else
        {
            // ���� ������Ʈ�� �ִ� ��� �޽ÿ� �޽� ������ ������Ʈ
            MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
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
