using Pnc.Model.FeatureDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureObject : MonoBehaviour
{
    // Start is called before the first frame update

    public string FileName;

    private GameObject[] frame;

    public float DefaultDepth = 5.0f;

    public float DepthRate = 2.0f;
    enum FeatureState
    {
        none,
        detect
    }

    FeatureState state = FeatureState.none;

    private void Start()
    {
        int arraysize = this.transform.childCount;
        frame = new GameObject[arraysize];

        for (int i = 0; i < frame.Length; i++)
        {
            frame[i] = this.transform.GetChild(i).gameObject;
            frame[i].SetActive(false);
            Debug.Log("Child Name :: " + frame[i].name);
        }
    }


    // Update is called once per frame
    void Update()
    {
        string featureNames = null;
        float[] featurePos = new float[8];
        if (ComparedFeatureResult.GetFeatureResult() == null)
        {
            return;
        }
        if (FileName == null)
        {
            return;
        }

        if (!ComparedFeatureResult.GetFeatureResult().getFeatures().Exists(item => item.filename == FileName))
        {
            return;
        }
        Feature temp = ComparedFeatureResult.GetFeatureResult().getFeatures()[ComparedFeatureResult.GetFeatureResult().getFeatures().FindIndex(item => item.filename == FileName)];

        if (state == FeatureState.none && temp.status == 1)
        {
            state = FeatureState.detect;
            temp.getPosratio().CopyTo(featurePos);

            for (int i = 0; i < featurePos.Length; i++)
            {
                Debug.Log(":::::     AndroidServiceBridge::featureobjTest= Position :: " + featurePos[i]);
            }
            MovePos(featurePos, temp.arearatio);

            //Debug.Log(":::::     AndroidServiceBridge::featureobjTest=}     ::::: " + FileName + " :: " + temp.arearatio);
        }
        else if (state == FeatureState.detect && temp.status == 0)
        {
            state = FeatureState.none;
        }

        if (state == FeatureState.detect)
        {
            for (int i = 0; i < frame.Length; i++)
            {
                frame[i].SetActive(true);
            }
        }
        else
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