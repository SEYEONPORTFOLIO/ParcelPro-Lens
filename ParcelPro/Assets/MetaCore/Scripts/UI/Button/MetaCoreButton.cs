using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Pnc.Model.Hands;
using TMPro;
using System;
using Unity.VectorGraphics;

namespace Pnc.UI.Button
{
    ///<summary>
    /// A button that can be pushed via direct touch.
    /// You can use <see cref="Microsoft.MixedReality.Toolkit.PhysicalPressEventRouter"/> to route these events to <see cref="Microsoft.MixedReality.Toolkit.UI.Interactable"/>.
    ///</summary>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/button")]
    [AddComponentMenu("MetaCore/Scripts/UI/Button/MetaCoreButton")]

    public class MetaCoreButton : MonoBehaviour, IMetaCoreTouchHandler
    {
        public bool useHLPlate = true;         //Highlight plate 사용 여부
        private bool setHighlight = false;
        public bool isTouched = true;
        public bool isPressed = false;
        public bool checkDist = true;
        public float closeDist = 0.1f;
        private Vector3 frontPlateScale = new Vector3(0.5f, 0.5f, 0.5f);
        private Vector3 enterPos;

        public GameObject frontPlate = null;
        public GameObject highlightPlate = null;
        public GameObject outline;
        public GameObject iconAndText = null;
        public GameObject fingerTip = null;
        public GameObject cursor = null;
        public int pressIntervalMs = 500;       //버튼 클릭(press) 간격(연속 이벤트 방지)
        public int soundIntervalMs = 200;       //sound 자주 실행되지 않도록
        
        private util.ProcessGesture processGesture;                     //특정 제스처일 때 클릭

        public bool useGestureClick = true;     //특정 제스처에서만 클릭
        public util.GestureType buttonGesture = util.GestureType.ONE;
        public List<util.GestureType> buttonGestures;

        private DateTime lastPressedTime;
        private DateTime lastSoundTime;

        [Header("Events")]
        public UnityEvent TouchBegin = new UnityEvent();
        public UnityEvent TouchEnd = new UnityEvent();
        public UnityEvent ButtonPressed = new UnityEvent();
        public UnityEvent ButtonReleased = new UnityEvent();
        
        // Start is called before the first frame update
        void Start()
        {
            if(frontPlate == null)
            {
                frontPlate = this.gameObject.transform.Find("FrontPlate").gameObject;
            }
            if(useHLPlate && highlightPlate == null)
            {
                highlightPlate = frontPlate.transform.Find("HighlightPlate").gameObject;
            }
            if(outline == null)
            {
                outline = frontPlate.transform.Find("Outline").gameObject;
            }
            if(iconAndText == null)
            {
                iconAndText = this.gameObject.transform.Find("IconAndText").gameObject;
            }
            if(fingerTip == null)
            {
                fingerTip = GameObject.FindGameObjectWithTag("FingerTip").gameObject;
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
                float dist;
                // if(useHLPlate)
                //     dist = util.AndroidUtils.getEuclideanDistance_3D(highlightPlate.transform.position, fingerTip.transform.position);
                // else
                //     dist = util.AndroidUtils.getEuclideanDistance_3D(frontPlate.transform.position, fingerTip.transform.position);
                dist = util.AndroidUtils.getEuclideanDistance_3D(this.transform.position, fingerTip.transform.position);

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

                        SVGImage image = highlightPlate.GetComponent<SVGImage>();
                        image.material = mat;
                        image.color = new Color32(255,255,255,45);
                        //Debug.Log($"MetaCoreButton Update PlateTouched");

                        setHighlight = true;

                        SetOutline();
                    }
                }
                else if(setHighlight)
                {
                    Material mat = Resources.Load<Material>("Materials/PlateTransparent");
                    MeshRenderer mr = highlightPlate.GetComponent<MeshRenderer>();
                    mr.material = mat;
                    
                    SVGImage image = highlightPlate.GetComponent<SVGImage>();
                    image.material = mat;
                    image.color = new Color32(255,255,255,45);
                    //Debug.Log($"MetaCoreButton Update PlateTransparent");

                    setHighlight = false;
                    
                    SetOutline(false);
                    // if(cursor)
                    //     cursor.SetActive(false);
                }
            }
        }

        void IMetaCoreTouchHandler.OnTouchStarted(HandsResult eventData)
        {
            Debug.Log($"MetaCoreButton OnTouchStarted start");
        }

        void IMetaCoreTouchHandler.OnTouchCompleted(HandsResult eventData)
        {
            Debug.Log($"MetaCoreButton OnTouchCompleted start");
        }

        void IMetaCoreTouchHandler.OnTouchUpdated(HandsResult eventData)
        {
            Debug.Log($"MetaCoreButton OnTouchUpdated start");
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("FingerTip") || other.CompareTag("HitRay"))
            {
                if(useGestureClick)
                {
                    if(processGesture == null)
                    {
                        Debug.Log($"MetaCoreButton Update processGesture is null");
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

                if(useHLPlate && !isTouched)
                {
                    Debug.Log($"MetaCoreButton OnTriggerEnter touching is not started. return.");
                    return;
                }
                SetAudioSource("focus");
                enterPos = other.transform.position;

                if(cursor)
                {
                    //cursor.transform.position = other.transform.position;

                    Material mat = Resources.Load<Material>("Materials/CursorActivated");
                    MeshRenderer mr = cursor.GetComponent<MeshRenderer>();
                    mr.material = mat;
                }

                Debug.Log($"MetaCoreButton OnTriggerEnter Event!!! TouchBegin Icon obj:{this.transform.name}, other:{other.transform.name}");
                TouchBegin.Invoke();
            }
        }

        void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("FingerTip") || other.CompareTag("HitRay"))
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
                float zDiff = other.transform.position.z - enterPos.z;
                //Debug.Log($"MetaCoreButton OnTriggerStay zDiff: {zDiff}, other.transform.position.z: {other.transform.position.z}, enterPos.z:{enterPos.z}");
                
                float localScaleZ = frontPlateScale.z - (zDiff*2.0f);
                //Debug.Log($"MetaCoreButton OnTriggerStay frontPlateScale.z:{frontPlateScale.z}, zDiff: {zDiff}");

                Debug.Log($"MetaCoreButton OnTriggerStay z(localScaleZ): {localScaleZ}");
                frontPlate.transform.localScale = new Vector3(frontPlateScale.x, frontPlateScale.y, localScaleZ);

                //실행 여부와 관계없이 눌린 모양은 유지
                TimeSpan diff = DateTime.Now - lastPressedTime;
                if(diff.TotalMilliseconds < pressIntervalMs)                       //중복 실행 방지
                {
                    Debug.Log("MetaCoreButton OnTriggerStay event is too frequent");
                    isTouched = false;      //다시 Highlight plate부터 터치하고 와야 함.
                    return;
                }
                
                if(useHLPlate && !isTouched)
                {
                    Debug.Log($"MetaCoreButton OnTriggerStay touching is not started. return.");
                    return;
                }

                // if(cursor)
                //     cursor.transform.position = other.transform.position;

                //if(other.transform.position.z < Math.Abs(iconAndText.transform.position.z) && !isPressed)
                if(!isPressed)
                {
                    SetAudioSource("click");

                    Debug.Log($"MetaCoreButton OnTriggerStay Event!!! ButtonPressed:{this.transform.name}, iconAndText.transform.position.z: {Math.Abs(iconAndText.transform.position.z)}");
                    isPressed = true;
                    isTouched = false;      //다시 Highlight plate부터 터치하고 와야 함.
                    lastPressedTime = DateTime.Now;
                    
                    ButtonPressed.Invoke();
                }
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("FingerTip") || other.CompareTag("HitRay"))
            {
                if(useHLPlate && !isTouched)
                {
                    Debug.Log($"MetaCoreButton OnTriggerExit touching is not started. return.");
                    //return;
                }
                
                Debug.Log($"MetaCoreButton OnTriggerExit Event!!! TouchEnd FingerTip");
                isTouched = false;
                isPressed = false;

                if(cursor)
                {
                    Material mat = Resources.Load<Material>("Materials/Cursor");
                    MeshRenderer mr = cursor.GetComponent<MeshRenderer>();
                    mr.material = mat;
                }

                frontPlate.transform.localScale = new Vector3(frontPlateScale.x, frontPlateScale.y, frontPlateScale.z);
                
                TouchEnd.Invoke();
                ButtonReleased.Invoke();
            }
        }

        private void SetAudioSource(string audioName)
        {
            TimeSpan diff = DateTime.Now - lastSoundTime;
            Debug.Log($"MetaCoreButton SetAudioSource diff:{diff.TotalMilliseconds}, sound interval:{soundIntervalMs}");

            // if(diff.TotalMilliseconds < soundIntervalMs)                       //중복 실행 방지
            // {
            //     Debug.Log("MetaCoreButton SetAudioSource too frequent! ignore.");
            //     return;
            // }
            
            GameObject goSound = this.gameObject.transform.Find("Sound").gameObject;
            GameObject go = goSound.gameObject.transform.Find(audioName).gameObject;
            if(go == null)
            {
                Debug.Log("MetaCoreButton SetAudioSource sound object not found.");
                return;
            }

            AudioSource audio = go.GetComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.loop = false;
            audio.time = 0.0f;

            audio.Play();
            lastSoundTime = DateTime.Now;
        }
        
        void SetOutline(bool isOn=true)
        {
            string strLineName = "";
            Material mat;
            
            Debug.Log($"PhysicalPressEventRouter SetOutline isOn:{isOn}");

            if(isOn)
                mat = Resources.Load<Material>("Materials/Outline");
            else
                mat = Resources.Load<Material>("Materials/PlateTransparent");

            for(int i=1; i<13; i++)
            {
                strLineName = "line" + i.ToString();
                GameObject goLine = outline.transform.Find(strLineName).gameObject;

                if(!goLine)
                {
                    Debug.Log($"PhysicalPressEventRouter SetOutline goLine is null LineName:{strLineName}");
                    continue;
                }

                goLine.GetComponent<MeshRenderer>().material = mat;
            }
        }
    }
}