
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pnc.Model;
using Pnc.Model.Hands;

namespace util
{
    public enum GestureType {
        UNKNOWN=0,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        PICK,
        RELEASE,
        FIST,
        CLICK,
        PAGE_UP,
        PAGE_DOWN,
        EXIT,
        ETC,
        RESERVED1,
        CUSTOM1,
        MY_GESTURE1
    }

    public class ProcessGesture : MonoBehaviour
    {
        private static ProcessGesture _instance = null; //singleton object

        //simon 220408 이전 몇 개의 프레임의 Gesture 정보를 참조할 것인지
        private const int MAX_FRAME_GESTURE = 5;
        private const int MAX_HAND_COUNT = 2;

        private DateTime lastReceivedTime;
        private bool isCleared = false;
        private bool validHands = false;

        private Hand leftHand = null;
        private Hand rightHand = null;

        private GestureType[,] preGesture = new GestureType[MAX_HAND_COUNT, MAX_FRAME_GESTURE];
        private GestureType[] curGesture = new GestureType[MAX_HAND_COUNT];
        private GestureType[] newGesture = new GestureType[MAX_HAND_COUNT];

        public int sleepCnt = 0;

        public float xScale = 1.5f;
        public float yScale = 1.5f;
        public float zScale = 1.0f;
        public float xAdjust = -0.1f;
        public float yAdjust = -0.1f;
        public float zAdjust = -0.1f;
        public float pickDistScale = 1.5f;
        public bool fixRaycast = false;
        public bool exitGesture = false;

        void Start()
        {
            Debug.Log($"ProcessGesture Start");
            lastReceivedTime = DateTime.Now;
            
            if (_instance==null)
            {
                _instance = this;

            } else {
                Destroy(this.gameObject);
                return;
            }
            
            //앱이 종료되기 전까지 singleton 객체 유지하도록 
            DontDestroyOnLoad(this.gameObject);

            //Gesture 배열들 초기화
            for (int i = 0; i < MAX_HAND_COUNT; i++) {
                curGesture[i] = GestureType.UNKNOWN;
                newGesture[i] = GestureType.UNKNOWN;
                for (int j = 0; j < MAX_FRAME_GESTURE; j++)     // initialization
                {
                    preGesture[i,j] = GestureType.UNKNOWN;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            TimeSpan diff = DateTime.Now - lastReceivedTime;

            //한손 또는 양손 다 있는데 일정 시간 수신받지 못하면 화면에 표시하지 않는다(landmark 이동 직후 제외 - 제외하지 않으면 수신 못받는 동안 계속 이동하므로).
            if((!isCleared && diff.TotalMilliseconds >= 500) ||
            (validHands && diff.TotalMilliseconds >= 500))
            {
                //        Debug.Log("ProcessGesture Update min=" + diff.Minutes.ToString() + ", sec=" + diff.Seconds.ToString());
                DisableLandmarks(false, false);

                isCleared = true;
                validHands = false;
            }

            if(exitGesture && GetGesture(false) == (int)util.GestureType.EXIT)                  //앱 종료
            {
                Debug.Log("########## GestureType.EXIT ##########");
                Application.Quit();
            }
        }
        public static ProcessGesture Instance() {
            if(!Exists()) {
                throw new PncException("[ProcessGesture] could not find the ProcessGesture object.");       
            }

            return _instance;
        }

        public static bool Exists()
        {
            return _instance != null;
        }

        public Camera GetCamera()
        {
            Camera camera = GameObject.Find("Eye Left")?.GetComponent<Camera>();
            if (camera == null)
            {
                camera = GameObject.Find("left Camera")?.GetComponent<Camera>();
            }
            return camera;
        }

        //Hand object를 전달. 최근 데이터가 없으면 null
        public Hand GetHand(bool isLeft=true)
        {
            if(isLeft)
            {
                return leftHand;
            }
            
            return rightHand;
        }

        public int GetGesture(bool isLeft=true)
        {
            if (isLeft)
            {
                if (leftHand == null)
                    return 0;

                return leftHand.gesture;
            }

            if (rightHand == null)
                return 0;

            return rightHand.gesture;
        }
        
        public bool LaunchApp(string packageName)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject pm = jo.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName);
            if(intent == null)
            {
                Debug.Log("Raycast Update intent is null");
                return false;
            }

            Debug.Log("Raycast Update startActivity intent: " + packageName);
            jo.Call("startActivity", intent);
            return true;
        }

        public void Execute(HandsResult handsResult)
        {
            if(sleepCnt > 0)
            {
                Debug.Log($"ProcessGesture Execute sleepCnt:{sleepCnt}");
                sleepCnt--;
                return;
            }

            if(handsResult == null)
            {
                return;
            }
            
            leftHand = null;
            rightHand = null;

            bool bIsRight = false;
            bool bIsPalm = false;

            bool validLeftHand = false;
            bool validRightHand = false;

            List<Hand> hand = handsResult.getHands();            
            //Debug.Log("ProcessGesture Execute hand count=" + hand.Count);

            int nHandIndex = 0;             //0:left, 1:right

            for(int i=0; i<hand.Count; i++)
            {
                //Debug.Log("ProcessGesture Execute: gesture=" + hand[i].gesture + ", type=" + hand[i].type);

                //1. 손 object 저장(각 script에서 가져다 쓴다)
                if(hand[i].type == "right")
                {
                    rightHand = hand[i];
                    nHandIndex = 1;

                    bIsRight = true;
                }
                else
                {
                    leftHand = hand[i];
                    nHandIndex = 0;

                    bIsRight = false;
                }

                setPreGestures(nHandIndex);
                
                // finger states
                bool thumbIsOpen = false;
                bool indexFingerIsOpen = false;
                bool middleFingerIsOpen = false;
                bool ringFingerIsOpen = false;
                bool pinkyFingerIsOpen = false;
                
                List<Landmark> landmarks = hand[i].getLandmarks();

                //각 손가락이 펴진 상태인지 체크
                if (bIsRight) {
                    if (landmarks[3].x < landmarks[2].x && landmarks[4].x < landmarks[2].x) {
                        thumbIsOpen = true;
                    }
                    if (landmarks[4].x > landmarks[20].x) {
                        bIsPalm = true;
                    }
                } else {
                    if (landmarks[3].x > landmarks[2].x && landmarks[4].x > landmarks[2].x) {
                        thumbIsOpen = true;
                    }

                    if (landmarks[4].x < landmarks[20].x) {
                        bIsPalm = true;
                    }
                }
                
                if (landmarks[7].y > landmarks[6].y && landmarks[8].y > landmarks[6].y) {
                    indexFingerIsOpen = true;
                }
                if (landmarks[11].y > landmarks[10].y && landmarks[12].y > landmarks[10].y) {
                    middleFingerIsOpen = true;
                }
                if (landmarks[15].y > landmarks[14].y && landmarks[16].y > landmarks[14].y) {
                    ringFingerIsOpen = true;
                }
                if (landmarks[19].y > landmarks[18].y && landmarks[20].y > landmarks[18].y) {
                    pinkyFingerIsOpen = true;
                }

                // Hand gesture recognition
                if (thumbIsOpen && indexFingerIsOpen && middleFingerIsOpen && ringFingerIsOpen && pinkyFingerIsOpen) {
                    if (bIsPalm)
                        curGesture[nHandIndex] = GestureType.RELEASE;
                    else
                        curGesture[nHandIndex] = GestureType.FIVE;
                } else if (/*middleFingerIsOpen && ringFingerIsOpen && pinkyFingerIsOpen &&*/ is2PointsNearScaled(landmarks[0], landmarks[17], landmarks[4], landmarks[8]) == true && !indexFingerIsOpen) {
                    curGesture[nHandIndex] = GestureType.PICK;
                } else if (thumbIsOpen && indexFingerIsOpen && !middleFingerIsOpen && !ringFingerIsOpen && !pinkyFingerIsOpen) {
                    curGesture[nHandIndex] = GestureType.TWO;
                } else if (!thumbIsOpen && indexFingerIsOpen && !middleFingerIsOpen && !ringFingerIsOpen && !pinkyFingerIsOpen) {
                    curGesture[nHandIndex] = GestureType.ONE;
                } else if (!thumbIsOpen && indexFingerIsOpen && middleFingerIsOpen && !ringFingerIsOpen && !pinkyFingerIsOpen) {
                    curGesture[nHandIndex] = GestureType.TWO;
                } else if (!thumbIsOpen && indexFingerIsOpen && middleFingerIsOpen && ringFingerIsOpen && pinkyFingerIsOpen) {
                    curGesture[nHandIndex] = GestureType.FOUR;
                } else if (thumbIsOpen && indexFingerIsOpen && middleFingerIsOpen && !ringFingerIsOpen && !pinkyFingerIsOpen) {
                    //simon 230329 오동작이 많아 수정
                    if(landmarks[10].y > landmarks[9].y && 
                    landmarks[11].y > landmarks[9].y && 
                    landmarks[12].y > landmarks[11].y && 
                    landmarks[12].y > landmarks[9].y && 
                    landmarks[7].y > landmarks[5].y && 
                    landmarks[8].y > landmarks[7].y && 
                    landmarks[8].y > landmarks[5].y)
                    {
                        curGesture[nHandIndex] = GestureType.THREE;
                    }
                } else if (!thumbIsOpen && indexFingerIsOpen && !middleFingerIsOpen && !ringFingerIsOpen && pinkyFingerIsOpen) {
                    curGesture[nHandIndex] = GestureType.ETC;
                } else if (thumbIsOpen && indexFingerIsOpen && !middleFingerIsOpen && !ringFingerIsOpen && pinkyFingerIsOpen) {                    
                    //simon 221005 손은 반듯하게 세워져 있을 때에만 적용해야 오작동할 확률이 적다.
                    if(landmarks[3].y > landmarks[0].y && 
                    landmarks[7].y > landmarks[0].y && 
                    landmarks[11].y > landmarks[0].y && 
                    landmarks[15].y > landmarks[0].y && 
                    landmarks[19].y > landmarks[0].y && 
                    landmarks[12].y < landmarks[9].y && 
                    landmarks[16].y < landmarks[9].y )
                    {
                        Debug.Log($"ProcessGesture Execute GestureType.EXIT 9.y={landmarks[9].y}, 12, 16.y={landmarks[12].y},{landmarks[16].y}");
                        curGesture[nHandIndex] = GestureType.EXIT;                        
                    }
                }
                else {
                    curGesture[nHandIndex] = GestureType.UNKNOWN;
                }

                if(bIsRight)
                {
                    rightHand.gesture = (int)curGesture[nHandIndex];
                }
                else
                {
                    leftHand.gesture = (int)curGesture[nHandIndex];
                }

                // Debug.Log("ProcessGesture Execute right? " + nHandIndex + " palm?" + bIsPalm + " Finger States :" +  thumbIsOpen + ", " + indexFingerIsOpen + ", " + 
                //     middleFingerIsOpen + ", " + ringFingerIsOpen + ", " + pinkyFingerIsOpen);

                Debug.Log("ProcessGesture Execute: right?=" + nHandIndex + ", curGesture=" + curGesture[nHandIndex] + ", bIsPalm=" + bIsPalm);
                
                //2. 손가락 landmark의 위치를 설정하는 부분
                string strLandmarkIndex = "";
                for(int j=0; j<21; j++)
                {
                    //simon 221031 손 크기를 설정한다.
                    Vector3 vTmp = new Vector3(landmarks[j].x*xScale, landmarks[j].y*yScale, landmarks[j].z*zScale);

                    vTmp.x = vTmp.x+xAdjust;
                    vTmp.y = vTmp.y+yAdjust;
                    vTmp.z = vTmp.z+zAdjust;
                    //simon 220628 스크린 좌표를 월드 좌표로 바꿔준다(고개를 돌려 화면을 벗어나도 손을 인식할 수 있게 한다).
                    Vector3 vWorld = GetCamera().ViewportToWorldPoint(vTmp);
                    
                    landmarks[j].x = vWorld.x;
                    landmarks[j].y = vWorld.y;
                    landmarks[j].z = vWorld.z;

                    if(bIsRight)
                    {
                        strLandmarkIndex = "landmark" + j;
                        validRightHand = true;
                    }
                    else
                    {
                        strLandmarkIndex = "landmark" + j + " (1)";
                        validLeftHand = true;
                    }

                    GameObject curObject = GameObject.Find(strLandmarkIndex);

                    if(curObject != null) {
                        curObject.transform.position = vWorld;
                    }
                }
            }
            lastReceivedTime = DateTime.Now;

            //손가락 좌표가 유효하지 않으면 표시하지 않는다.
            if(validLeftHand && validRightHand)
                validHands = true;
            else
                DisableLandmarks(validLeftHand, validRightHand);
            
            isCleared = false;
        }

        public void DisableLandmarks(bool validLeftHand, bool validRightHand)
        {
            //Debug.Log("ProcessGesture DisableLandmarks left: " + validLeftHand + ", right: " + validRightHand);
            string strLandmarkIndex = "";
            
            Vector3 vTmp = new Vector3(-500.0f, -500.0f, -500.0f);
            if(!validRightHand)
            {
                for(int j=0; j<21; j++)
                {
                    strLandmarkIndex = "landmark" + j;
                    GameObject curObject = GameObject.Find(strLandmarkIndex);

                    if(curObject!=null) curObject.transform.position = vTmp;
                }             
                rightHand = null;
            }
            
            if(!validLeftHand)
            {
                for(int j=0; j<21; j++)
                {
                    strLandmarkIndex = "landmark" + j + " (1)";
                    GameObject curObject = GameObject.Find(strLandmarkIndex);
                    if(curObject!=null) curObject.transform.position = vTmp;
                }
                leftHand = null;
            }
        }
        
        //nHandIdx : 0(left), 1(right)
        private GestureType setPreGestures(int nHandIdx) {
            if (nHandIdx > MAX_HAND_COUNT - 1)
                return GestureType.UNKNOWN;


            bool bAllSame = true;
            for (int i = 0; i < MAX_FRAME_GESTURE - 1; i++) {
                preGesture[nHandIdx, i] = preGesture[nHandIdx, i + 1];

                if (preGesture[nHandIdx, i] != curGesture[nHandIdx])
                    bAllSame = false;
            }

            preGesture[nHandIdx, MAX_FRAME_GESTURE - 1] = curGesture[nHandIdx];

            if (bAllSame)          //simon 220408 새로운 Gesture이고, 이전 10개와 현재 Gesture가 전부 같을 때만 로그를 찍는다.
            {
                newGesture[nHandIdx] = curGesture[nHandIdx];
    //            Debug.Log("setPreGestures new Gesture: " + newGesture);
                //clearPreGestures();
            } else
                newGesture[nHandIdx] = GestureType.UNKNOWN;
    //        Debug.Log("setPreGestures new index: " + nHandIdx);
            return newGesture[nHandIdx];
        }

        private void clearPreGestures() {
            for (int i = 0; i < MAX_HAND_COUNT; i++) {
                for (int j = 0; j < MAX_FRAME_GESTURE; j++) {
                    preGesture[i, j] = GestureType.UNKNOWN;
                }
            }
        }

        //simon 220802 손의 크기에 따라 landmark가 가까운지 여부를 판단한다.
        private bool is2PointsNearScaled(Landmark lm0, Landmark lm17, Landmark point1, Landmark point2) {
            float scale = getEuclideanDistance_3D(lm0, lm17) * 0.2F;

            float distance = getEuclideanDistance_3D(point1, point2);
            //Debug.Log("is2PointsNearScaled pick? distance : " + distance + ", scale : " + scale + ", compare : " + scale * 0.008F);

            //Debug.Log($"is2PointsNearScaled pick? distance : {distance}, scale*{pickDistScale} : {scale*pickDistScale} fixRaycast:{fixRaycast}");

            if(distance < scale*pickDistScale)
            {
                fixRaycast = true;
            }
            else
            {
                fixRaycast = false;
            }

            if(distance < scale)
            {
                //Debug.Log("is2PointsNearScaled return true distance: " + distance + ", compare: " + scale);
                return true;
            }

            //Debug.Log("is2PointsNearScaled return false distance: " + distance + ", compare: " + scale);
            return false;
        }
        
        private float getEuclideanDistance_3D(Landmark point1, Landmark point2) {
            float fX = point1.x - point2.x;
            float fY = point1.y - point2.y;
            float fZ = point1.z - point2.z;

            return Mathf.Sqrt((fX * fX) + (fY * fY) + (fZ * fZ));
        }
    }
}