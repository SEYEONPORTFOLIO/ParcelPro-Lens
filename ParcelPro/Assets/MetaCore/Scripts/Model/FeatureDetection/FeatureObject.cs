using Pnc.Model.FeatureDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureObject : MonoBehaviour //[클래스설명] Unity에서 게임 오브젝트의 상태에 따라 특징 오브젝트를 관리하고 표시하는 클래스.
{
    // Start is called before the first frame update

    public string FileName; // 파일 이름을 저장하는 변수입니다.

    private GameObject[] frame; // 프레임 오브젝트를 저장하는 배열입니다.

    public float DefaultDepth = 5.0f; // 기본 깊이를 나타내는 변수입니다.

    public float DepthRate = 2.0f; // 깊이 조절 레이트를 나타내는 변수입니다.
    enum FeatureState // 오브젝트 상태를 정의하는 열거형입니다.
    {
        none, // 아무 상태도 아님
        detect // 특징을 감지 중인 상태
    }

    FeatureState state = FeatureState.none; // 초기 상태는 'none'입니다.

    private void Start()
    {
        int arraysize = this.transform.childCount; 
        frame = new GameObject[arraysize]; // 자식 오브젝트의 수만큼 프레임 배열을 초기화합니다.

        for (int i = 0; i < frame.Length; i++)
        {
            frame[i] = this.transform.GetChild(i).gameObject;
            frame[i].SetActive(false); // 자식 오브젝트를 배열에 저장하고 비활성화합니다.
            Debug.Log("Child Name :: " + frame[i].name); // 자식 오브젝트의 이름을 디버그 로그로 출력합니다.
        }
    }


    // Update is called once per frame
    void Update()
    {
        string featureNames = null;
        float[] featurePos = new float[8];

        // 특징 결과를 확인하고, 결과가 없으면 함수를 종료합니다.
        if (ComparedFeatureResult.GetFeatureResult() == null)
        {
            return;
        }
        // 파일 이름이 없으면 함수를 종료합니다.
        if (FileName == null)
        {
            return;
        }
        // 특징 결과에 파일 이름이 존재하지 않으면 함수를 종료합니다.
        if (!ComparedFeatureResult.GetFeatureResult().getFeatures().Exists(item => item.filename == FileName))
        {
            return;
        }
        // 파일 이름에 해당하는 특징을 가져옵니다.
        Feature temp = ComparedFeatureResult.GetFeatureResult().getFeatures()[ComparedFeatureResult.GetFeatureResult().getFeatures().FindIndex(item => item.filename == FileName)];
        // 상태가 'none'이고 특징의 상태가 1이면 특징을 감지 중인 상태로 변경합니다.
        if (state == FeatureState.none && temp.status == 1)
        {
            state = FeatureState.detect;
            temp.getPosratio().CopyTo(featurePos);

            for (int i = 0; i < featurePos.Length; i++)
            {
                Debug.Log(":::::     AndroidServiceBridge::featureobjTest= Position :: " + featurePos[i]);
            }

            // 특징 위치를 이동합니다.
            MovePos(featurePos, temp.arearatio);

            //Debug.Log(":::::     AndroidServiceBridge::featureobjTest=}     ::::: " + FileName + " :: " + temp.arearatio);
        }
        
        else if (state == FeatureState.detect && temp.status == 0)
        {
            state = FeatureState.none;
        }
        // 상태가 'detect'이면 모든 프레임 오브젝트를 활성화합니다.
        if (state == FeatureState.detect)
        {
            for (int i = 0; i < frame.Length; i++)
            {
                frame[i].SetActive(true);
            }
        }
        else // 상태가 'none'이면 모든 프레임 오브젝트를 비활성화합니다.
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