using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;

using Pnc.Model.Hands;
using DG.Tweening;

public class PinControl : MonoBehaviour
{
    public bool pin = true;
    public Canvas mainCanvas = null;
    public GameObject UIButtonSquareIcon;
    public Camera mainCamera = null;
    public float centerDepth = 0.8f;
    //public GameObject head = null;
    //SvrManager svrManager;
    Vector3 initPos = Vector3.zero;
    Vector3 unpinPos = Vector3.zero;
    SVGImage img = null;
    int preGesture = (int)util.GestureType.UNKNOWN;
    
    private util.ProcessGesture processGesture;

    // Start is called before the first frame update
    void Start()
    {        
        if(mainCanvas == null)
            mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

        if(mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCam").GetComponent<Camera>();
            if(mainCamera == null)
                mainCamera = GameObject.Find("Eye Left")?.GetComponent<Camera>();
        }
        
        if(UIButtonSquareIcon == null)
        {
            GameObject iconAndText = this.gameObject.transform.Find("IconAndText").gameObject;
            UIButtonSquareIcon = iconAndText.transform.Find("UIButtonSquareIcon").gameObject;
        }
        
        img = UIButtonSquareIcon.GetComponent<SVGImage>();
        
        //svrManager = SvrManager.Instance;

        initPos = mainCanvas.transform.position;
        unpinPos = initPos;
    }

    void Awake()
    {        
        Vector3 vCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f,0.5f,centerDepth));
        Debug.Log($"PinControl Update Hit Center x:{vCenter.x}, y:{vCenter.y}, z:{vCenter.z}");
        
        mainCanvas.transform.position = vCenter;
        mainCanvas.transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward*3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //if(!pin && svrManager)
        if(!pin)
        {
            // var headTransform = svrManager.head;

            // Debug.Log($"PinControl Update headTransform x:{headTransform.position.x}, y:{headTransform.position.y}, z:{headTransform.position.z}");
            //mainCanvas.transform.position = new Vector3(headTransform.position.x, headTransform.position.y, headTransform.position.z+0.7f);
            //mainCanvas.transform.rotation = headTransform.rotation;
            
            Vector3 vPos = mainCamera.ViewportToWorldPoint(unpinPos);
            Debug.Log($"PinControl Update vPos x:{vPos.x}, y:{vPos.y}, z:{vPos.z}");
            
            mainCanvas.transform.position = vPos;
            mainCanvas.transform.LookAt(mainCanvas.transform.position + mainCamera.transform.rotation * Vector3.forward*3.0f);
        }

        if(processGesture == null)
        {
            Debug.Log($"PinControl Update processGesture is null");
            processGesture = util.ProcessGesture.Instance();
        }
        
        Hand rightHand = processGesture.GetHand(false);
        if(rightHand == null)
        {
            //Debug.Log($"PinControl Update rightHand is null");
            return;
        }
        
        if(preGesture != (int)util.GestureType.THREE &&
            rightHand.gesture == (int)util.GestureType.THREE)                      //Three 제스처일 때 현재 카메라의 화면 중앙으로 이동
        {
            Vector3 vCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f,0.5f,centerDepth));
            Debug.Log($"PinControl Update Hit Center x:{vCenter.x}, y:{vCenter.y}, z:{vCenter.z}");
            
            //mainCanvas.transform.position = vCenter;
            //mainCanvas.transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward*3.0f);
            //mainCanvas.transform.DOMove(vCenter + mainCamera.transform.rotation * Vector3.forward*3.0f, 1);
            mainCanvas.transform.DOMove(vCenter, 1);
            mainCanvas.transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward*3.0f);
            
            SetAudioSource("move");
        }
        
        preGesture = rightHand.gesture;
    }

    public void Pin()
    {
        pin = !pin;
        Debug.Log($"PinControl Pin val:{pin}");

        string strSprite = "";
        if(pin)
        {
            Vector3 vCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f,0.5f,centerDepth));
            Debug.Log($"PinControl Update Hit Center x:{vCenter.x}, y:{vCenter.y}, z:{vCenter.z}");
            
            mainCanvas.transform.position = vCenter;
            mainCanvas.transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward*3.0f);
            //mainCanvas.transform.position = initPos;
            strSprite = "Sprite/Launcher/02_pin";
        }
        else
        {
            unpinPos = mainCamera.WorldToViewportPoint(mainCanvas.transform.position);
            Debug.Log($"PinControl Update unpinPos Screen x:{unpinPos.x}, y:{unpinPos.y}, z:{unpinPos.z}");
            strSprite = "Sprite/Launcher/03_unpin";
        }

        Sprite sprPin = Resources.Load<Sprite>(strSprite) as Sprite;
        img.sprite = sprPin;
    }
    
    private void SetAudioSource(string audioName)
    {
        Debug.Log($"PinControl SetAudioSource");
        
        GameObject goSound = this.gameObject.transform.Find("Sound").gameObject;
        GameObject go = goSound.gameObject.transform.Find(audioName).gameObject;
        if(go == null)
        {
            Debug.Log("PinControl SetAudioSource sound object not found.");
            return;
        }

        AudioSource audio = go.GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
        audio.time = 0.0f;

        audio.Play();
    }    
}
