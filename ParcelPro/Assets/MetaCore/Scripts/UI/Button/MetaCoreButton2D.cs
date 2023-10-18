using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Pnc.Model.Hands;
using TMPro;
using System;

namespace Pnc.UI.Button
{
    ///<summary>
    /// A button that can be pushed via direct touch.
    /// You can use <see cref="Microsoft.MixedReality.Toolkit.PhysicalPressEventRouter"/> to route these events to <see cref="Microsoft.MixedReality.Toolkit.UI.Interactable"/>.
    ///</summary>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/button")]
    [AddComponentMenu("MetaCore/Scripts/UI/Button/MetaCoreButton")]

    public class MetaCoreButton2D : MonoBehaviour
    {
        public bool useHLPlate = true;         //Highlight plate 사용 여부
        private bool setHighlight = false;
        public bool isTouched = false;
        private bool isPressed = false;
        public bool checkDist = true;
        public float closeDist = 0.1f;
        private Vector3 frontPlateScale = new Vector3(0.5f, 0.5f, 0.5f);
        private Vector3 enterPos;
        public GameObject frontPlate = null;
        public GameObject highlightPlate = null;
        public GameObject cursor = null;

        public string packageName;
        public GameObject fingerTip = null;
        public int pressIntervalMs = 500;       //버튼 클릭(press) 간격(연속 이벤트 방지)
        public int soundIntervalMs = 200;       //sound 자주 실행되지 않도록

        private util.ProcessGesture processGesture;                     //특정 제스처일 때 클릭

        public bool useGestureClick = true;     //특정 제스처에서만 클릭
        //public util.GestureType buttonGesture = util.GestureType.ONE;
        public List<util.GestureType> buttonGestures;

        public int btnStayMs = 2000;
        private int btnPressStayMs = 0;

        private DateTime lastPressedTime;
        private DateTime lastSoundTime;

        private Vector3 vTouchStart = Vector3.zero;
        
        // Start is called before the first frame update
        void Start()
        {
            AndroidJavaClass _jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject _currentActivity = _jc.GetStatic<AndroidJavaObject>("currentActivity");
            
            if(fingerTip == null)
            {
                fingerTip = GameObject.FindGameObjectWithTag("FingerTip").gameObject;
            }
            if(frontPlate == null)
            {
                frontPlate = this.gameObject.transform.Find("FrontPlate").gameObject;
            }
            if(useHLPlate && highlightPlate == null)
            {
                highlightPlate = frontPlate.transform.Find("HighlightPlate").gameObject;
            }
            if(cursor == null)
            {
                cursor = GameObject.FindGameObjectWithTag("Cursor");
            }

            frontPlateScale = frontPlate.transform.localScale;
            lastPressedTime = DateTime.Now;
            lastSoundTime = DateTime.Now;
        }

        // Update is called once per frame
        void Update()
        {
            if(checkDist)
            {
                float dist = util.AndroidUtils.getEuclideanDistance_3D(frontPlate.transform.position, fingerTip.transform.position);

                if(dist < closeDist)
                {
                    //Debug.Log($"MetaCoreButton Update name:{this.transform.name} distance:{dist}");
                    
                    if(cursor)
                    {
                        cursor.SetActive(true);
                        cursor.transform.position = new Vector3(fingerTip.transform.position.x, fingerTip.transform.position.y, fingerTip.transform.position.z);
                    }

                    if(useHLPlate && !setHighlight)
                    {
                        Material mat = Resources.Load<Material>("Materials/PlateTouched");
                        MeshRenderer mr = highlightPlate.GetComponent<MeshRenderer>();
                        mr.material = mat;

                        setHighlight = true;
                    }
                }
                else
                {
                    Material mat = Resources.Load<Material>("Materials/PlateTransparent");
                    MeshRenderer mr = highlightPlate.GetComponent<MeshRenderer>();
                    mr.material = mat;
                    
                    setHighlight = false;
                    
                    if(cursor)
                        cursor.SetActive(false);
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            btnPressStayMs = 0;             //머무른 시간 초기화
            lastPressedTime = DateTime.Now;

            if(other.CompareTag("FingerTip"))
            {
                if(useGestureClick)
                {
                    if(processGesture == null)
                    {
                        Debug.Log($"MetaCoreButton OnTriggerEnter processGesture is null");
                        processGesture = util.ProcessGesture.Instance();
                    }
                    Hand rightHand = processGesture.GetHand(false);
                    if(rightHand == null)         //특정 제스처일 때에만 인식
                    {
                        //Debug.Log($"MetaCoreButton OnTriggerEnter rightHand is null");
                        return;
                    }
                    else
                    {
                        bool pressGesture = false;
                        for (int i = 0; i < buttonGestures.Count; i++)         //특정 제스처일 때에만 인식
                        {
                            Debug.Log($"MetaCoreButton OnTriggerEnter rightHand:{rightHand.gesture} button[i]:{(int)buttonGestures[i]}");
                            if(rightHand.gesture == (int)buttonGestures[i])
                            {
                                pressGesture = true;
                            }
                        }

                        Debug.Log($"MetaCoreButton OnTriggerEnter pressGesture:{pressGesture}");
                        if(!pressGesture)
                        {
                            //Debug.Log($"MetaCoreButton OnTriggerEnter rightHand is null");
                            return;
                        }
                    }
                }

                Debug.Log($"MetaCoreButton2D OnTriggerEnter Start isTouched:{isTouched}");
                if(useHLPlate && !isTouched)
                {
                    Debug.Log($"MetaCoreButton2D OnTriggerEnter touching is not started. return.");
                    return;
                }
                SetAudioSource("focus");
                enterPos = other.transform.position;

                if(cursor)
                {
                    cursor.transform.position = other.transform.position;

                    Material mat = Resources.Load<Material>("Materials/CursorActivated");
                    MeshRenderer mr = cursor.GetComponent<MeshRenderer>();
                    mr.material = mat;
                }

                Debug.Log($"MetaCoreButton2D OnTriggerEnter Event!!! TouchBegin Icon obj:{this.transform.name}, other:{other.transform.name}");
                //TouchBegin.Invoke();
            }
        }

        void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("FingerTip"))
            {
                if(useGestureClick)
                {
                    if(processGesture == null)
                    {
                        Debug.Log($"MetaCoreButton OnTriggerStay processGesture is null");
                        processGesture = util.ProcessGesture.Instance();
                    }
                    Hand rightHand = processGesture.GetHand(false);
                    if(rightHand == null)         //특정 제스처일 때에만 인식
                    {
                        //Debug.Log($"MetaCoreButton OnTriggerStay rightHand is null");
                        return;
                    }
                    else
                    {
                        bool pressGesture = false;
                        for (int i = 0; i < buttonGestures.Count; i++)         //특정 제스처일 때에만 인식
                        {
                            Debug.Log($"MetaCoreButton OnTriggerEnter rightHand:{rightHand.gesture} button[i]:{(int)buttonGestures[i]}");
                            if(rightHand.gesture == (int)buttonGestures[i])
                            {
                                pressGesture = true;
                            }
                        }

                        Debug.Log($"MetaCoreButton OnTriggerEnter pressGesture:{pressGesture}");
                        if(!pressGesture)
                        {
                            //Debug.Log($"MetaCoreButton OnTriggerStay rightHand is null");
                            return;
                        }
                    }
                }

                if(useHLPlate && !isTouched)
                {
                    Debug.Log($"MetaCoreButton OnTriggerStay touching is not started. return.");
                    return;
                }

                //TimeSpan diff = DateTime.Now - lastPressedTime;
                // if(diff.TotalMilliseconds < pressIntervalMs)                       //중복 실행 방지
                // {
                //     Debug.Log("MetaCoreButton OnTriggerStay event is too frequent");
                //     return;
                // }

                float zDiff = other.transform.position.z - enterPos.z;
                //Debug.Log($"MetaCoreButton OnTriggerStay zDiff: {zDiff}, other.transform.position.z: {other.transform.position.z}, enterPos.z:{enterPos.z}");
                float localScaleZ = frontPlateScale.z - (zDiff*2.0f);
                //Debug.Log($"MetaCoreButton OnTriggerStay frontPlateScale.z:{frontPlateScale.z}, zDiff: {zDiff}");
                frontPlate.transform.localScale = new Vector3(frontPlateScale.x, frontPlateScale.y, localScaleZ);

                if(cursor)
                    cursor.transform.position = other.transform.position;

                //Debug.Log($"MetaCoreButton OnTriggerStay btnPressStayMs: {btnPressStayMs}, btnStayMs:{btnStayMs}, isPressed:{isPressed}");
                //if(other.transform.position.z < transform.position.z && !isPressed)
                if(!isPressed)
                {
                    if(btnPressStayMs > btnStayMs)                    //simon 230308 일정 시간 머물러야 클릭 이벤트 발생.
                    {
                        SetAudioSource("click");

                        Debug.Log($"MetaCoreButton OnTriggerStay Event!!! ButtonPressed:{this.transform.name}, other.transform.position.z: {other.transform.position.z}");
                        isPressed = true;
                        btnPressStayMs = 0;
                        lastPressedTime = DateTime.Now;

                        OpenAppwithPackageName();
                        //ButtonPressed.Invoke();
                    }
                    else                        //버튼 안에 머무른 시간
                    {
                        TimeSpan diff = DateTime.Now - lastPressedTime;
                        btnPressStayMs = (int)diff.TotalMilliseconds;
                    }
                }
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("FingerTip"))
            {
                if(useHLPlate && !isTouched)
                {
                    Debug.Log($"MetaCoreButton OnTriggerExit touching is not started. return.");
                    return;
                }
                
                Debug.Log($"MetaCoreButton OnTriggerExit Event!!! TouchEnd FingerTip");

                if(cursor)
                {
                    Material mat = Resources.Load<Material>("Materials/Cursor");
                    MeshRenderer mr = cursor.GetComponent<MeshRenderer>();
                    mr.material = mat;
                }

                frontPlate.transform.localScale = new Vector3(frontPlateScale.x, frontPlateScale.y, frontPlateScale.z);
                
                // TouchEnd.Invoke();
                // ButtonReleased.Invoke();
                lastPressedTime = DateTime.Now;
                
                // if(isPressed)
                // {   
                //     //button pressed!
                //     OpenAppwithPackageName();
                // }
                isTouched = false;
                isPressed = false;

                btnPressStayMs = 0;
            }
        }

        private void SetAudioSource(string audioName)
        {
            TimeSpan diff = DateTime.Now - lastSoundTime;
            Debug.Log($"MetaCoreButton2D SetAudioSource diff:{diff.TotalMilliseconds}, sound interval:{soundIntervalMs}");

            // if(diff.TotalMilliseconds < soundIntervalMs)                       //중복 실행 방지
            // {
            //     Debug.Log("MetaCoreButton2D SetAudioSource too frequent! ignore.");
            //     return;
            // }
            
            GameObject goSound = this.gameObject.transform.Find("Sound").gameObject;
            GameObject go = goSound.gameObject.transform.Find(audioName).gameObject;
            //GameObject go = GameObject.Find(audioName);
            if(go == null)
            {
                Debug.Log("MetaCoreButton2D SetAudioSource sound object not found.");
                return;
            }

            AudioSource audio = go.GetComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.loop = false;
            audio.time = 0.0f;

            audio.Play();
            lastSoundTime = DateTime.Now;
        }
        
        void OpenAppwithPackageName()
        {
            Debug.Log($"MetaCoreButton2D OpenAppwithPackageName package={packageName}");

            AndroidJavaClass _jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = _jc.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject param1 = new AndroidJavaObject("java.lang.String", packageName);
            AndroidJavaObject param2 = new AndroidJavaObject("java.lang.String", "");

            Debug.Log($"MetaCoreButton2D OpenAppwithPackageName goApp, package={packageName}, param1={param1}, param2={param2}");
            currentActivity.CallStatic("goApp", currentActivity, param1, param2);
            //Application.Quit();
        }
    }
}