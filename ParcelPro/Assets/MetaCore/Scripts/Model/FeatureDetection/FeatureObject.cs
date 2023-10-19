using Pnc.Model.FeatureDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureObject : MonoBehaviour //[Ŭ��������] Unity���� ���� ������Ʈ�� ���¿� ���� Ư¡ ������Ʈ�� �����ϰ� ǥ���ϴ� Ŭ����.
{
    // Start is called before the first frame update

    public string FileName; // ���� �̸��� �����ϴ� �����Դϴ�.

    private GameObject[] frame; // ������ ������Ʈ�� �����ϴ� �迭�Դϴ�.

    public float DefaultDepth = 5.0f; // �⺻ ���̸� ��Ÿ���� �����Դϴ�.

    public float DepthRate = 2.0f; // ���� ���� ����Ʈ�� ��Ÿ���� �����Դϴ�.
    enum FeatureState // ������Ʈ ���¸� �����ϴ� �������Դϴ�.
    {
        none, // �ƹ� ���µ� �ƴ�
        detect // Ư¡�� ���� ���� ����
    }

    FeatureState state = FeatureState.none; // �ʱ� ���´� 'none'�Դϴ�.

    private void Start()
    {
        int arraysize = this.transform.childCount; 
        frame = new GameObject[arraysize]; // �ڽ� ������Ʈ�� ����ŭ ������ �迭�� �ʱ�ȭ�մϴ�.

        for (int i = 0; i < frame.Length; i++)
        {
            frame[i] = this.transform.GetChild(i).gameObject;
            frame[i].SetActive(false); // �ڽ� ������Ʈ�� �迭�� �����ϰ� ��Ȱ��ȭ�մϴ�.
            Debug.Log("Child Name :: " + frame[i].name); // �ڽ� ������Ʈ�� �̸��� ����� �α׷� ����մϴ�.
        }
    }


    // Update is called once per frame
    void Update()
    {
        string featureNames = null;
        float[] featurePos = new float[8];

        // Ư¡ ����� Ȯ���ϰ�, ����� ������ �Լ��� �����մϴ�.
        if (ComparedFeatureResult.GetFeatureResult() == null)
        {
            return;
        }
        // ���� �̸��� ������ �Լ��� �����մϴ�.
        if (FileName == null)
        {
            return;
        }
        // Ư¡ ����� ���� �̸��� �������� ������ �Լ��� �����մϴ�.
        if (!ComparedFeatureResult.GetFeatureResult().getFeatures().Exists(item => item.filename == FileName))
        {
            return;
        }
        // ���� �̸��� �ش��ϴ� Ư¡�� �����ɴϴ�.
        Feature temp = ComparedFeatureResult.GetFeatureResult().getFeatures()[ComparedFeatureResult.GetFeatureResult().getFeatures().FindIndex(item => item.filename == FileName)];
        // ���°� 'none'�̰� Ư¡�� ���°� 1�̸� Ư¡�� ���� ���� ���·� �����մϴ�.
        if (state == FeatureState.none && temp.status == 1)
        {
            state = FeatureState.detect;
            temp.getPosratio().CopyTo(featurePos);

            for (int i = 0; i < featurePos.Length; i++)
            {
                Debug.Log(":::::     AndroidServiceBridge::featureobjTest= Position :: " + featurePos[i]);
            }

            // Ư¡ ��ġ�� �̵��մϴ�.
            MovePos(featurePos, temp.arearatio);

            //Debug.Log(":::::     AndroidServiceBridge::featureobjTest=}     ::::: " + FileName + " :: " + temp.arearatio);
        }
        
        else if (state == FeatureState.detect && temp.status == 0)
        {
            state = FeatureState.none;
        }
        // ���°� 'detect'�̸� ��� ������ ������Ʈ�� Ȱ��ȭ�մϴ�.
        if (state == FeatureState.detect)
        {
            for (int i = 0; i < frame.Length; i++)
            {
                frame[i].SetActive(true);
            }
        }
        else // ���°� 'none'�̸� ��� ������ ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        {
            for (int i = 0; i < frame.Length; i++)
            {
                frame[i].SetActive(false);
            }
        }
    }

    void moveToCenter()
    {
        Camera camera = GameObject.FindGameObjectWithTag("MainCam").GetComponent<Camera>();
        if (camera == null)
        {
            camera = GameObject.Find("Eye Left")?.GetComponent<Camera>();
        }

        Vector3 vCenter = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1.0f));
        Debug.Log($"Raycast Update Hit Center x:{vCenter.x}, y:{vCenter.y}, z:{vCenter.z}");

        this.transform.position = vCenter;
        this.transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward * 3.0f);
    }

    // GameObject obj = GameObject.Find("FeatureDetection");
    // obj.GetComponent<FeatureObject>().SetObjectEnable(false);

    public void SetObjectEnable(bool isEnable)
    {
        if (isEnable)
        {
            state = FeatureState.detect;
        }
        else
        {
            state = FeatureState.none;
        }
    }

    public void MovePos(float[] _Pos, float depth)
    {
        float _centerx = 0, _centery = 0;
        for (int i = 0; i < 4; i++)
        {
            _centerx += _Pos[2 * i];
            _centery += _Pos[2 * i + 1];
        }
        _centerx = _centerx / 4;
        _centery = (1 - _centery / 4);

        Camera camera = GameObject.FindGameObjectWithTag("MainCam").GetComponent<Camera>();
        if (camera == null)
        {
            camera = GameObject.Find("Eye Left")?.GetComponent<Camera>();
        }
        //Debug.Log($":::::     AndroidServiceBridge::featureobjTest= Position :: x:{_centerx}, y:{_centery}, z:{depth}");

        Vector3 vCenter = camera.ViewportToWorldPoint(new Vector3(_centerx, _centery, camera.nearClipPlane + (DefaultDepth - DepthRate * 2)));
        //Debug.Log($":::::     AndroidServiceBridge::featureobjTest= CamPosition :: x:{camera.transform.position.x}, y:{camera.transform.position.y}, z:{camera.transform.position.z}");
        //Debug.Log($":::::     AndroidServiceBridge::featureobjTest= Position :: x:{vCenter.x}, y:{vCenter.y}, z:{vCenter.z}");

        this.transform.position = vCenter;
        this.transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward * 3.0f);
    }
}