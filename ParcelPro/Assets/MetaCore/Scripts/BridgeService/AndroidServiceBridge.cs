using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

using Pnc.Model;
using Pnc.Model.Hands;
using Pnc.Model.Slam;
using Pnc.Model.Stt;
using Pnc.Model.FeatureDetection;
//using Pnc.ui;

/*************************************************************
* PNC XR SDK API 및 이벤트(메세지) 전송규격
1.SDK에서의 메세지 전송에는 json 을 사용한다.
2.메세지데이터(json) 의 serialize/deserialize 에는 newtonsoft-json 을 사용한다.
3.newtonsoft-json 은 unity(il2cpp) 에서 사용이 가능한 라이브러리를 사용해야 한다.
  3.1 "https://github.com/jilleJr/Newtonsoft.Json-for-Unity/wiki/Install-official-via-UPM" 참조

4.다음과 같은 방식으로 json 라이브러리를 설치해야 한다.
  4.1 패키지 폴더(수정"~/packages" )의 manifest.json 파일을 텍스트 편집기로 연다
  4.2 "dependencies" 항목에 패키지명을 입력하고 저장한다.
    {
      "dependencies": {
        "com.unity.nuget.newtonsoft-json": "3.0.2",
        // ...
      }
}
*************************************************************/
namespace Pnc
{
  public class AndroidServiceBridge : MonoBehaviour
  {
    private const Boolean DEBUG = true;
    public class AidlServiceContext {
      /* 카메라 ID(A21M 기준) */
      public const string PNC_CAMERA_RGB = "0"; //FRONT
      public const string PNC_CAMERA_6DOF_L = "1"; //EXTERNAL 1 (6DoF-L)
      public const string PNC_CAMERA_6DOF_R = "2"; //EXTERNAL 2 (6DoF-R)
      public const string PNC_CAMERA_IR = "3"; //BACK

      public const string PNC_CAMERA_STEREO_6DOF = "5"; //6DoF-L + 6DoF-R (SLAM에서 stereo 모드로 사용)

      public const string PNC_CAMERA_POS_LEFT = "0"; //카메라 위치-왼쪽카메라
      public const string PNC_CAMERA_POS_RIGHT = "1"; //카메라 위치-오른쪽
      public const string CAMERA_ROTATION_DEGREE = "0"; //카메라센서의 회전상태(A21M D221114이전 버전은 270, 이후는 0)


      private int _aidlContext; //AIDL을 제어하기 위한 context
      private string _serviceType; //실행중인 서비스타입
      private string _serviceState = EVT_SERVICE_TERM; //실행중인 서비스 상태, //서비스연결상태 "serviceInit/serviceStart/serviceStop/serviceTerm";
      private string _cameraId = PNC_CAMERA_RGB; //사용할 카메라ID
      private string _cameraRotationDegree = CAMERA_ROTATION_DEGREE; //카메라센서의 회전상태.
                                                 //이 센서의 값이 90, 180, 270 일 경우, 제스처서비스 내부에서 해당 각도로 회전된 좌표가 반환된다. 

      private string _uniqueId = "FFFFFFFF"; //서비스 제어용 uniqueId, FFFFFFFF: 서비스 없음


      public AidlServiceContext(string serviceType, string cameraId) {
        _serviceType = serviceType;
        _cameraId = cameraId;
        _cameraRotationDegree = CAMERA_ROTATION_DEGREE;
      }

      public string GetServiceType() {
        return _serviceType;
      }

      public void SetServiceType(string serviceType) {
        _serviceType = serviceType;
      }

      public string GetServiceState() {
        return _serviceState;
      }

      public void SetServiceState(string serviceState) {
        _serviceState = serviceState;
      }

      public string GetCameraId() {
        return _cameraId;
      }

      public void SetCameraId(string cameraId) {
        _cameraId = cameraId;
      }
 
      public string GetCameraRotation()
      {
        return _cameraRotationDegree;
      }

      public void setCameraRotation(string rotationDegree)
      {
        _cameraRotationDegree = rotationDegree;
      }

      public string GetServiceUniqueId() {
        return _uniqueId;
      }

      public void SetServiceUniqueId(string uniqueId) {
        _uniqueId = uniqueId;
      }

    }

        private util.ProcessGesture processGesture;


        //선택할 수 있는 안드로이드 서비스의 종류
        public Boolean _dualCamera = false; //유니티에서 카메라를 2개사용할 것인지 여부
                                            //A20 is set to 'false'
                                            //but, A21M is set to 'true'
        public const string SERVICE_TYPE_GESTURE = "gesture"; //제스쳐 서비스
        public const string SERVICE_TYPE_SLAM = "slam"; //SLAM 서비스
        public const string SERVICE_TYPE_STT_VVK = "stt.vvk"; //STT_VVK 서비스
        public const string SERVICE_TYPE_STT_GCS = "stt.gcs"; //STT_GCS 서비스
        public const string SERVICE_TYPE_GESTURE_PNC = "gesture.pnc"; //PNC 제스쳐 서비스
        public const string SERVICE_TYPE_FEATURE = "feature";


        //안드로이드 서비스로부터 수신되는 이벤트 종류
        public const string EVT_SERVICE_INIT = "serviceInit"; //서비스가 초기화되었음
        public const string EVT_SERVICE_TERM = "serviceTerm"; //서비스가 종료되었음
        public const string EVT_SERVICE_START = "serviceStart"; //서비스가 시작되었음
        public const string EVT_SERVICE_STOP = "serviceStop"; ////서비스가 중지되었음
        public const string EVT_HANDS_RESULT = "handsResult"; //HandsResult가 수신됨
        public const string EVT_FEATURE_RESULT = "featureResult"; //FeatureResult가 수신됨
        public const string EVT_SLAM_RESULT = "slamResult"; //SLAM 결과가 수신됨
        public const string EVT_STT_RESULT = "sttResult"; //음성인식 결과 수신됨
        public const string EVT_VOLUME_UP = "volumeUp"; //volume up 이벤트가 수신됨
        public const string EVT_VOLUME_DOWN = "volumeDown"; //volume down 이벤트가 수신됨

    //PncEventListener 정의
    public delegate void PncEventListener(string jsonString);
    public interface PncEventHandler {
		  void OnEventArrived(string jsonString);
	  };


    private List<PncEventListener> _eventlisteners = new List<PncEventListener>(); //이벤트를 수신받을 리스너


    private static AndroidServiceBridge _instance = null; //singleton object

        //사용할(enalble) 서비스 타입정의
        public bool _enableGesture = true;
        public bool _enableFeature = false;
        public bool _enableSlam = false;
        //stt는 백그라운드에서도 동작해야 하므로, 별도로 관리한다.
        public bool _enableStt = false; // STT
        AidlServiceContext _aidlContextStt = null;

    private ConcurrentDictionary <string, AidlServiceContext> _aidlContexts; //AIDL서비스 리스트

    //stt 명령어 정의 및 실행할 함수저장
    private Dictionary<string, Func<int, int>> _sttActions = null;
    string[] _sttCommands = {/*"홈으로",*/"피앤씨", "드로잉","블루투스 설정","와이파이 설정","피플"/*,"앱 종료"*/,"박스월드"};


    public Camera  _leftCamera;
    public Camera  _rightCamera;
    //int delayMillis = 1000; //1 second for wait

    //ipc object for AIDL
    AndroidJavaObject _aidlPlugin;

    float[] fTemp_model;
    float[] fTemp_view;
    float[] fTemp_projection;
    Matrix4x4 tModelMat;
    Matrix4x4 tViewMat;
    Matrix4x4 tProjectionMat;
  
    //for SLAM service
    private bool _needMatrixUpdated = false;
    private SlamResult _lastSlamResult = null;


    // STT 관련 변수 및 메서드
    public string _resultDText = null;


    /*###############################################
    #                                               #
    #      for singleton pattern                    #
    #                                               #
    ################################################*/
    public static AndroidServiceBridge Instance() {
      if(!Exists()) {
        throw new PncException("[AndroidServiceBridge] could not find the AndroidServiceBridge object.");       
      }

      return _instance;
    }

    public static bool Exists()
    {
      return _instance != null;
    }


    /************************************************
    *   이벤트리스너 등록                           *
    *************************************************/
    public void addEventListener(PncEventListener listener)
    {
      Debug.Log($":::::     AndroidServiceBridge addEventListener start, registered={_eventlisteners.Count}     :::::");
      _eventlisteners.Add(listener);
      Debug.Log($":::::     AndroidServiceBridge addEventListener done,  registered={_eventlisteners.Count}     :::::");
    }

    /************************************************
    *   이벤트리스너 해제                           *
    *************************************************/
    public void removeEventListener(PncEventListener listener)
    {
      _eventlisteners.Remove(listener);
    }

    /************************************************
    *   이벤트리스너 찾기                           *
    *************************************************/
    public bool isRegisteredListener(PncEventListener listener)
    {
      return _eventlisteners.Contains(listener);
    }

    /************************************************
    *   이벤트리스너 올 클리어                      *
    *************************************************/
    public void clearEventListener()
    {
      _eventlisteners.Clear();
    }

    /*###############################################
    #                                               #
    #      for unity life cycle                     #
    #                                               #
    ################################################*/
    //override life cycle
    protected void Awake()
    {    
      Debug.Log($":::::     AndroidServiceBridge Awake     :::::");
      if (_instance==null)
      {
        _instance = this;

      } else {
        Destroy(this.gameObject);
        return;
      }

      
      //앱이 종료되기 전까지 singleton 객체 유지하도록 
      DontDestroyOnLoad(this.gameObject);

      //AIDL 로딩한 후 서비스API 호출할 것
      LoadAidl();      

            _aidlContexts = new ConcurrentDictionary<string, AidlServiceContext>();


            if (_enableFeature)
            {
                AddAidlService(SERVICE_TYPE_FEATURE, AidlServiceContext.PNC_CAMERA_RGB); //사물인식 서비스 추가
                if (_enableGesture) AddAidlService(SERVICE_TYPE_GESTURE_PNC, AidlServiceContext.PNC_CAMERA_RGB); //제스쳐인식 서비스 추가
            }
            else{
                if (_enableGesture) AddAidlService(SERVICE_TYPE_GESTURE, AidlServiceContext.PNC_CAMERA_RGB); //제스쳐인식 서비스 추가
            }
            if (_enableSlam) AddAidlService(SERVICE_TYPE_SLAM, AidlServiceContext.PNC_CAMERA_STEREO_6DOF); //SLAM 서비스 추가

            //서비스에서 사용하지 않도라도 인스턴스는 존재해야 한다.
            _aidlContextStt = AddSttAidlService();

      // 221031
      if (_enableStt) {
        _sttActions = new Dictionary<string, System.Func<int, int>>();

        // //메시지 출력용
        foreach (var command in _sttCommands) {
          _sttActions.Add(command, (param)=>{            
            util.AndroidUtils.showToast(command); //명령어 토스트 출력
            util.AndroidUtils.goAction(command); //명렁어에 따른 분기
            return 0;
          });
        }    

        EnableAidlService(_aidlContextStt, true);
      }
    }



    //override life cycle
    protected void Start()
    {
      Debug.Log($":::::     AndroidServiceBridge Start     :::::");

      _leftCamera = GameObject.Find("Eye Left")?.GetComponent<Camera>();
      if (_leftCamera == null)
      {
          _leftCamera = GameObject.Find("left Camera")?.GetComponent<Camera>();
      }

      if(_enableGesture) {
        processGesture = this.GetComponent<util.ProcessGesture>();
        if(processGesture == null)
        {
          Debug.LogError($":::::     [ERROR] AndroidServiceBridge Start processGesture is null     :::::");
        }
      }

      
    }


    //override life cycle
    // protected void Update()
    // {
      
    // }

    //override life cycle
    protected void OnDestroy()
    {
      Debug.Log($":::::     AndroidServiceBridge OnDestroy     :::::");     
    }

    //override life cycle
    // protected void OnApplicationFocus(bool focus)
    // {
    //   if(DEBUG) Debug.Log($":::::     [Unity Lifecycle]::OnApplicationFocus focus={focus}     :::::");

    // }

    //override life cycle
    protected void OnApplicationPause(bool pause)
    {
      Debug.Log($":::::     AndroidServiceBridge OnApplicationPause paused={pause}     :::::");
      if(!pause) {
        //서비스 시작
        EnableAidlServices(true);
      } else {
        //서비스 멈춤
        EnableAidlServices(false);
      }
    }

    //override life cycle
    protected void OnApplicationQuit()
    {
      Debug.Log(":::::     AndroidServiceBridge OnApplicationQuit     :::::");
      EnableAidlServices(false);

      TerminateAidlServices();

      TerminateSttAidlService();
     }


    /*###############################################
    #                                               #
    #      internal functions                       #
    #                                               #
    ################################################*/
    /************************************************
    *   android 서비스의 상태 저장                   *
    *************************************************/
    private void SetServiceState(string serviceType, string serviceState) {
      if(_aidlContexts.ContainsKey(serviceType))
        _aidlContexts[serviceType].SetServiceState(serviceState);

      if(_aidlContextStt.Equals(serviceType))
        _aidlContextStt.SetServiceState(serviceState);
        
    }

    /************************************************
    *   android 서비스의 상태 조회                   *
    *************************************************/
    private string GetServiceState(string serviceType) {
      if(_aidlContexts.ContainsKey(serviceType))
        return _aidlContexts[serviceType].GetServiceState();

      if(_aidlContextStt.Equals(serviceType))
        _aidlContextStt.GetServiceState();

      return null;
    }


    /************************************************
    *   load AIDL module                            *
    *************************************************/
    private void SetupCamera() {
      //_leftCamera = Camera.main;;
      // _leftCamera = GameObject.FindGameObjectWithTag("left Camera").transform;;
      // _rightCamera = GameObject.FindGameObjectWithTag("right Camera").transform;;
      _leftCamera = GameObject.Find("left Camera").GetComponent<Camera>();
      _rightCamera = GameObject.Find("right Camera").GetComponent<Camera>();

      if(_dualCamera) {
        // for A21
        _leftCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
        _leftCamera.enabled = true;

        _rightCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
        _rightCamera.enabled = true;
      } else {
        //for A20
        _leftCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        _rightCamera.enabled = false;
      }

    }

    public void LoadAidl()
    {
      if(DEBUG) Debug.Log($":::::     AndroidServiceBridge::LoadAidl     :::::");

      if (_aidlPlugin == null)
      {
        _aidlPlugin = new AndroidJavaObject("kr.co.pncsolution.aidlmodule.AidlLoaderPlugin");
      }
    }

    /************************************************
    *           AddAidlService:                     *
    *************************************************/
    public AidlServiceContext AddAidlService(string serviceType, string cameraId)
    {
      if(DEBUG) Debug.Log($":::::     AndroidServiceBridge::Add AidlService serviceType={serviceType}     :::::");
      AidlServiceContext context = null;
      if(_aidlContexts!=null) {
        if(!_aidlContexts.ContainsKey(serviceType)) {
          if(DEBUG) Debug.Log($":::::     Add new AidlService serviceType={serviceType}     :::::");
          //서비스가 등록되어 있지 않으면 추가
          context = new AidlServiceContext(serviceType, cameraId);
          _aidlContexts.TryAdd(serviceType, context);

          ServiceInit(serviceType, context.GetCameraId(), context.GetCameraRotation());
          
        } else {
          if(DEBUG) Debug.Log($":::::     Exist AidlService serviceType={serviceType}     :::::");
          context = _aidlContexts[serviceType];
          context.SetCameraId(cameraId);

        }

      } else {
        Debug.LogError(":::::     [ERROR] invalid dictionary for AidlContext     :::::");
      }

      return context;
    }

    /************************************************
    * aidl 서비스의 uniqueId 저장                   *
    *************************************************/
    private void UpdateAidlServiceUniqueId(string serviceType, string uniqueId) {
      if (_aidlContexts.ContainsKey(serviceType)) {
        AidlServiceContext context = _aidlContexts[serviceType];
        context.SetServiceUniqueId(uniqueId);

        //refrence 로 동작하는가?
        //_aidlContexts[serviceType] = context;

        if(DEBUG) Debug.Log(":::::     Update AidlService Handle at dictionries     :::::");
      } else if (serviceType.Equals(_aidlContextStt.GetServiceType()))
      {
        _aidlContextStt.SetServiceUniqueId(uniqueId);
        if(DEBUG) Debug.Log(":::::     Update AidlService Handle at STT context     :::::");
      }
      else {
        Debug.LogError(":::::     [ERROR] Update AidlService Handle, Context not found     :::::");
      }


    }

    /************************************************
    * aidl 서비스의 uniqueId 조회                   *
    *************************************************/
    private string QueryAidlServiceUniqueId(string serviceType) {

      if (_aidlContexts.ContainsKey(serviceType)) {
        AidlServiceContext context = _aidlContexts[serviceType];
        //if(DEBUG) Debug.Log($":::::     QueryAidlServiceUniqueId:: Service Found at Dictionaries, serviceTpye={serviceType}, uniqueId={context.GetServiceUniqueId()}     :::::");
        return context.GetServiceUniqueId();
      }

      if(serviceType.Equals(_aidlContextStt.GetServiceType())) {
        //if(DEBUG) Debug.Log($":::::     QueryAidlServiceUniqueId:: Service Found at Stt, serviceTpye={serviceType}, uniqueId={_aidlContextStt.GetServiceUniqueId()}     :::::");
        return _aidlContextStt.GetServiceUniqueId();
      }

      Debug.LogError(":::::     [ERROR]  QueryAidlServiceUniqueId:: Context not found     :::::");

      return null;
    }

    /************************************************
    *           todo:                *
    *************************************************/
    private void EnableAidlServices(bool enable) {
      foreach(KeyValuePair<string, AidlServiceContext> pair in _aidlContexts) {
        string serviceType = pair.Key;
        AidlServiceContext context = pair.Value;
        EnableAidlService(context, enable);   
      }
    }

    private void EnableAidlService(AidlServiceContext context, bool enable) {
      if(context==null) {
        Debug.LogError($":::::     AndroidServiceBridge::EnableAidlService, [ERROR] Invalid service context!     :::::");
        return;
      }

      string serviceType = context.GetServiceType();
      //if(DEBUG) Debug.Log($":::::     AndroidServiceBridge::EnableAidlService, type={serviceType}, enable={enable}     :::::");

      // if(_aidlContexts.ContainsKey(serviceType)) {
      //   AidlServiceContext context = _aidlContexts[serviceType];

        if(enable) {
          // ServiceInit(serviceType, context.GetCameraId(), context.GetCameraRotation());
          ServiceStart(serviceType);

        } else {
          ServiceStop(serviceType);
        }
      // }
    }

    /************************************************
    *           todo:                *
    *************************************************/
    private void TerminateAidlServices() {
      foreach(KeyValuePair<string, AidlServiceContext> pair in _aidlContexts) {
        string serviceType = pair.Key;
        AidlServiceContext context = pair.Value;
        ServiceTerm(serviceType);
      }
    }

    /*###############################################
    #                                               #
    #      calls to Android service functions       #
    #                                               #
    ################################################*/
    /************************************************
    * calls the 'init' in AIDL service              *
    *************************************************/
    private void ServiceInit(string serviceType, string cameraId, string cameraRotation)
    {
      Debug.Log($":::::     AndroidServiceBridge::ServiceInit, serviceType={serviceType}     :::::");

//      float[] matrix = _aidlActivity.CallStatic<float[]>("getModelMatrix");
      Pnc.Model.PncApi pncApi = new Pnc.Model.PncApi();
      pncApi.name = "init";
      pncApi.serviceType = serviceType;
      pncApi.cameraId = cameraId;
      pncApi.cameraRotation = cameraRotation;

      if (serviceType.Equals(SERVICE_TYPE_STT_GCS)
      || serviceType.Equals(SERVICE_TYPE_STT_VVK)) {
        // pncApi.micId = "0";
        // pncApi.language = "ko_kr";

        //stt 명령어 문자열 취합
        string commands = string.Empty;
        // foreach(var command in _sttCommands) {
        //   commands += command + ",";
        // }

        for(int i=0;i<_sttCommands.Length;i++) {
          var command = _sttCommands[i];
          if(i!=0) commands += ",";
          
          commands += command;        
        }

        pncApi.sttCommand = commands;
    }

      string jsonParam = Pnc.Model.PncJson<Pnc.Model.PncApi>.serialize(pncApi); 
     
      string uniqueId = CallAndroidNative(pncApi.name, jsonParam);

      if(DEBUG) Debug.Log($":::::     AndroidServiceBridge::ServiceInit Done, serviceType={serviceType} is created. uniqueId={uniqueId}      :::::");

      UpdateAidlServiceUniqueId(serviceType, uniqueId);
    }

    /************************************************
    * calls the 'start' in AIDL service             *
    *************************************************/
    private void ServiceStart(string serviceType)
    {
      string uniqueId = QueryAidlServiceUniqueId(serviceType);

      if (uniqueId==null) {
        Debug.LogError($":::::     AndroidServiceBridge::ServiceStart, uniqueId={uniqueId} has not been found. This serivce will be omitted.     :::::");
      }

      //Debug.Log($":::::     AndroidServiceBridge::ServiceStart, uniqueId={uniqueId}, serviceType={serviceType}. Try to start...     :::::");

      Pnc.Model.PncApi pncApi = new Pnc.Model.PncApi();
      pncApi.name = "start";
      pncApi.serviceType = serviceType;
      pncApi.uniqueId = uniqueId;

      //util.UnityMainThreadDispatcher.Instance().Enqueue(() => {
      
        string jsonParam = Pnc.Model.PncJson<Pnc.Model.PncApi>.serialize(pncApi); 
        string result = CallAndroidNative(pncApi.name, jsonParam);
        Debug.Log($":::::     AndroidServiceBridge::ServiceStart, uniqueId={pncApi.uniqueId}, serviceType={serviceType}. Done     :::::");

      //});
    }

    /************************************************
    * calls the 'stop' in AIDL service              *
    *************************************************/
    private void ServiceStop(string serviceType)
    {
      string uniqueId = QueryAidlServiceUniqueId(serviceType);

      if (uniqueId == null)
      {
        Debug.LogError($":::::     AndroidServiceBridge::ServiceStop, uniqueId={uniqueId} has not been found. This serivce will be omitted.     :::::");
      }

      Pnc.Model.PncApi pncApi = new Pnc.Model.PncApi();
      pncApi.name = "stop";
      pncApi.serviceType = serviceType;
      pncApi.uniqueId = uniqueId;

      Debug.Log($":::::     AndroidServiceBridge::ServiceStop, uniqueId={pncApi.uniqueId}, serviceType={serviceType}     :::::");

      string jsonParam = Pnc.Model.PncJson<Pnc.Model.PncApi>.serialize(pncApi); 
      string result = CallAndroidNative(pncApi.name, jsonParam);
    }

    /************************************************
    * calls the 'term' in AIDL service              *
    *************************************************/
    private void ServiceTerm(string serviceType)
    {
      string uniqueId = QueryAidlServiceUniqueId(serviceType);

      if (uniqueId == null)
      {
        Debug.LogError($":::::     AndroidServiceBridge::ServiceTerm, uniqueId={uniqueId} has not been found. This serivce will be omitted.     :::::");
      }

      Pnc.Model.PncApi pncApi = new Pnc.Model.PncApi();
      pncApi.name = "term";
      pncApi.serviceType = serviceType;
      pncApi.uniqueId = QueryAidlServiceUniqueId(serviceType);

      Debug.Log($":::::     AndroidServiceBridge::ServiceTerm, uniqueId={pncApi.uniqueId}, serviceType={serviceType}     :::::");

      string jsonParam = Pnc.Model.PncJson<Pnc.Model.PncApi>.serialize(pncApi); 
      string result = CallAndroidNative(pncApi.name, jsonParam);

    }

    /************************************************
    * calls the ' ' in AIDL service      *
    ************************************************/
    private void reserved1()
    {
      Pnc.Model.PncApi pncApi = new Pnc.Model.PncApi();
      // pncApi.name = "reserved1";
      // string jsonParam = Pnc.Model.PncJson<Pnc.Model.PncApi>.serialize(pncApi); 
      // CallAndroidNative(pncApi.name, jsonParam);
      CallAndroidNative("reserved1", null);
    }


    /*###############################################
    #                                               #
    #      calls to Android native functions        #
    #                                               #
    ################################################*/
    public String CallAndroidNative(string functionName, string jsonParam)
    {
      String result = "FAIL"; //"SUCCESS" or "FAIL"
      if (_aidlPlugin != null)
      {
#if UNITY_ANDROID
        if(jsonParam!=null) {
          result = _aidlPlugin.Call<String>(functionName, jsonParam);
        } else {
          result = _aidlPlugin.Call<String>(functionName);
        }
        //int result = _aidlActivity.CallStatic<int>(functionName);
        if(DEBUG) Debug.Log($"'{functionName}' result code={result}");
#endif
      }
      return result;
    }

    /*###############################################
    #                                               #
    #   the events have been updated from android   #
    #                                               #
    ################################################*/
    void OnEventArrived(string jsonParam)
    {
      
      PncEvent evt = PncJson<PncEvent>.deserialize(jsonParam);

      string type = evt.getEventType();

            if (DEBUG
            && !"handsResult".Equals(type)
            && !"featureResult".Equals(type)
            //&& !"featureResult".Equals(type)
            ) Debug.Log($":::::     AndroidServiceBridge::OnEventArrived: param={jsonParam}     :::::");


            switch (type)
            {
                case EVT_SERVICE_INIT: //서비스 초기화됨
                    OnMsgServiceInit(evt);
                    break;
                case EVT_SERVICE_TERM: //서비스 종료됨
                    OnMsgServiceTerm(evt);
                    break;
                case EVT_SERVICE_START: //서비스 시작됨
                    OnMsgServiceStart(evt);
                    break;
                case EVT_SERVICE_STOP: //서비스 멈춤
                    OnMsgServiceStop(evt);
                    break;
                case EVT_HANDS_RESULT: //제스쳐 이벤트 수신됨
                    OnMsgGestureUpdated(jsonParam);
                    break;
                case EVT_FEATURE_RESULT:
                    OnMsgFeatureUpdated(jsonParam);
                    break;
                case EVT_SLAM_RESULT: //SLAM 이벤트 수신됨
                    OnMsgSlamUpdated(jsonParam);
                    break;
                case EVT_STT_RESULT: //음성인식 결과 수신됨
                    OnMsgSttUpdated(jsonParam);
                    break;
                case EVT_VOLUME_UP: //테스트용
                    OnMsgVolumeUp(jsonParam);
                    break;
                case EVT_VOLUME_DOWN: //테스트용
                    OnMsgVolumeDown(jsonParam);
                    break;
                default:
                    if (DEBUG) Debug.LogWarning($"Unknown event={type}");
                    break;
            }

      //등록된 리스너로 이벤트 전달
      foreach(PncEventListener listener in _eventlisteners) {
        listener(jsonParam);
      }

    }

    /************************************************
    * received 'ServiceInit' message from android   *
    *************************************************/
    void OnMsgServiceInit(PncEvent evt)
    {
      //서비스가 바인드되고, ServiceConnected 후에 "ServiceInit" 이 전달된다.
      //따라서, ServiceInit 수신된 후에 ServiceStart/ServiceStop/ServiceTerm 등을 호출해야 한다.
      string uniqueId = evt.getUniqueuId();
      string owner = evt.getOwner();
      SetServiceState(owner, EVT_SERVICE_INIT);
            
      ServiceStart(owner);
    }

    /************************************************
    * received 'ServiceTerm' message from android   *
    *************************************************/
    void OnMsgServiceTerm(PncEvent evt)
    {
      SetServiceState(evt.owner, EVT_SERVICE_TERM);

    }

    /************************************************
    * received 'ServiceStart' message from android  *
    *************************************************/
    void OnMsgServiceStart(PncEvent evt)
    {
      SetServiceState(evt.owner, EVT_SERVICE_START);

    }

    /************************************************
    * received 'ServiceStop' message from android   *
    *************************************************/
    void OnMsgServiceStop(PncEvent evt)
    {
      SetServiceState(evt.owner, EVT_SERVICE_STOP);
    }


    /************************************************
    * the hands updated message from android        *
    *************************************************/
    void OnMsgGestureUpdated(string jsonParam) 
    {
      EventHandsResult evt;
      //if(DEBUG) Debug.Log("AndroidServiceBridge OnMsgGestureUpdated starts");
      try {
        evt = PncJson<EventHandsResult>.deserialize(jsonParam);
      } catch (PncException e) {
        Debug.LogError($":::::     [ERROR] json param has been occurred Exception. what={e.Message}     :::::");
        return;
      }

      //simon 220812 서비스로부터 받은 Mediapipe 결과값만 전달하고 처리는 ProcessGesture에서 한다.
      processGesture?.Execute(evt.getHandsResult());
    }

        void OnMsgFeatureUpdated(string jsonParam)
        {
            EventFeatureResult evt;
            Debug.Log("AndroidServiceBridge OnMsgFeatureUpdated starts :: " + jsonParam);
            try
            {
                evt = PncJson<EventFeatureResult>.deserialize(jsonParam);
            }
            catch (PncException e)
            {
                Debug.LogError($"json param has been occurred Exception. what={e.Message}");
                return;
            }
            if (evt.getFeatureResult() != null)
            {
                //processFeatureDetection.Execute(evt.getFeatureResult());
                ComparedFeatureResult.SetFeatureResult(evt.getFeatureResult());
            }
        }

    /************************************************
    * the SLAM updated message from android        *
    *************************************************/
    void OnMsgSlamUpdated(string jsonParam) 
    {
      Pnc.Model.Slam.EventSlamResult evt;
      if(DEBUG) Debug.Log("AndroidServiceBridge::OnMsgSlamUpdated");

      try {
        evt = PncJson<EventSlamResult>.deserialize(jsonParam);
      } catch (PncException e) {
        Debug.LogError($":::::     [ERROR] json param has been occurred Exception. what={e.Message}     :::::");
        return;
      }

      SlamResult newSlamResult = evt.getSlamResult();

      if(newSlamResult==null) {
        Debug.LogError(":::::     [ERROR] invalid slam result.     :::::");
        return;
      }

      //todo: 이전상태와 비교해서 변경이 있으면 matrix 초기화할 것     
      if(_lastSlamResult==null) {
        _lastSlamResult = newSlamResult;
      } /* else if(newSlamResult.getTrackingResult()==SlamResult.SLAM_ON && newSlamResult.getPlaneDetect()==SlamResult.PLANE_DETECTED) {
          _needMatrixUpdated = true;
      } */

      //if(_lastSlamResult?.getTrackingResult()!=SlamResult.SLAM_ON) {
          if(_lastSlamResult.getTrackingResult()!=newSlamResult.getTrackingResult() || _lastSlamResult.getPlaneDetect() != newSlamResult.getPlaneDetect()) {
            _needMatrixUpdated = true;
          }
      //}
      

      _lastSlamResult = newSlamResult;
//      _needMatrixUpdated = true;

      if(_needMatrixUpdated) {
        initMatrix();
        _needMatrixUpdated = false;
      }

      

        //sjy get viewmatrix from android aar(slam)
        //fTemp_view = ajc2.CallStatic<float[]>("getViewMatrix");
        fTemp_view = _lastSlamResult.getView()?.ToArray();

        Matrix4x4 viewMat = new Matrix4x4();
        //sjy convert android matrix to unity matrix(reverse row & column)
        for (int i = 0; i < 4; i++)
        {
            viewMat.SetColumn(i, new Vector4(fTemp_view[0 + (4 * i)], fTemp_view[1 + (4 * i)], fTemp_view[2 + (4 * i)], fTemp_view[3 + (4 * i)]));
        }

        //Vector3 modelv3 = Camera.main.ViewportToWorldPoint(new Vector3((fTemp_model[0 + (4 * 3)] + 1) / 2, (fTemp_model[1 + (4 * 3)] + 1) / 2, fTemp_model[2 + (4 * 3)]));
        //Vector3 viewv3 = Camera.main.ViewportToWorldPoint(new Vector3((fTemp_view[0 + (4 * 3)] + 1) / 2, (fTemp_view[1 + (4 * 3)] + 1) / 2, fTemp_view[2 + (4 * 3)]));

        //modelMat.SetColumn(3, new Vector4(fTemp_model[0 + (4 * 3)] * 2 * p.x, fTemp_model[1 + (4 * 3)] * 2 * p.y, fTemp_model[2 + (4 * 3)] * p.z, fTemp_model[3 + (4 * 3)]));
        //viewMat.SetColumn(3, new Vector4(fTemp_view[0 + (4 * 3)] * 2 * p.x, fTemp_view[1 + (4 * 3)] * 2 * p.y, fTemp_view[2 + (4 * 3)] * p.z, fTemp_view[3 + (4 * 3)]));
        tViewMat = viewMat;
        Matrix4x4 tempPV = Camera.main.projectionMatrix * tViewMat;
        // Matrix4x4 tempPV = tProjectionMat * tViewMat;
        // if(DEBUG) Debug.Log("ViewM0 :: \n" + tempPV.ToString());
        Matrix4x4 tempM = tempPV * tModelMat;   // 이데이터 사용할 것.
        // Matrix4x4 tempM = tempPV * aa;   // 이데이터 사용할 것.
        // Matrix4x4 tempM = tViewMat * tModelMat;   // 이데이터 사용할 것.

        // if(DEBUG) Debug.Log("ViewM1 :: \n" + tempM.ToString());

        Vector3 tPositionVector = new Vector3((tempM.m03 + 1) / 2.0f, (tempM.m13 + 1) / 2.0f, tempM.m23);

        // if(DEBUG) Debug.Log("ViewM2 :: \n" + tPositionVector.ToString());

        Vector3 mvmatv3 = Camera.main.ViewportToWorldPoint(tPositionVector);
        //tempM.m03 = mvmatv3.x;
        //tempM.m13 = mvmatv3.y;
        //tempM.m23 = -mvmatv3.z;
        Shader.SetGlobalMatrix("_MviewMatrix", tempM);

        // if(DEBUG) Debug.Log("ViewM3 :: \n" + tempM.ToString());
        // if(DEBUG) Debug.Log("ViewM4 :: \n" + tempM.m03 + " , " + tempM.m13  + " , " +  tempM.m23 + " /// " + mvmatv3.ToString());

        
      // GameObject slamCube = GameObject.Find("CubeSlamTest");
      // if(slamCube!=null) {
        
      // }

    }


    /************************************************
    * the speech to text updated message from android        *
    *************************************************/
    void OnMsgSttUpdated(string jsonParam) 
    {
      EventSttResult evt;
      // if(DEBUG) Debug.Log("AndroidServiceBridge OnMsgSttUpdated starts");

      try {
        evt = PncJson<EventSttResult>.deserialize(jsonParam);
        
        if(DEBUG) Debug.Log(evt.info());

        _resultDText = evt.getSttResult().getResultText();

        string resultText = evt.getSttResult().getResultText();

        // STT 결과 문자열 : string
        if(DEBUG) Debug.Log($"AndroidServiceBridge OnMsgSttUpdated _resultDText:{_resultDText}");

        util.AndroidUtils.showToast(_resultDText); //명령어 토스트 출력
        
        foreach (var action in _sttActions) {
          if(resultText.Equals(action.Key)) {
            Func<int, int> runAction = action.Value; //Func 실행
            //runAction(0);
          }
        }
      } catch (PncException e) {
        Debug.LogError($":::::     [ERROR] json param has been occurred Exception. what={e.Message}     :::::");
      }
    }


    /************************************************
    * the volume up message from android        *
    *************************************************/
    void OnMsgVolumeUp(string jsonParam) 
    {
      //initMatrix();
      
    }

    /************************************************
    * the volume up message from android        *
    *************************************************/
    void OnMsgVolumeDown(string jsonParam) 
    {
      //initMatrix();
    }


    public void initMatrix()
    {
      if(DEBUG) {
        Debug.Log("#:::::     slam matrix updated     :::::");
      }

      if(_lastSlamResult==null) {
        Debug.LogError(":::::     [ERROR] slam result not found.     :::::");
      }
      fTemp_model = _lastSlamResult.getModel()?.ToArray();
      fTemp_projection = _lastSlamResult.getProjection()?.ToArray();
      
      Matrix4x4 modelMat = new Matrix4x4();
      Matrix4x4 projectionMat = new Matrix4x4();
      for (int i = 0; i < 4; i++)
      {
          modelMat.SetColumn(i, new Vector4(fTemp_model[0 + (4 * i)], fTemp_model[1 + (4 * i)], fTemp_model[2 + (4 * i)], fTemp_model[3 + (4 * i)]));
          projectionMat.SetColumn(i, new Vector4(fTemp_projection[0 + (4 * i)], fTemp_projection[1 + (4 * i)], fTemp_projection[2 + (4 * i)], fTemp_projection[3 + (4 * i)]));
      }
      Vector3 tMPositionVector = new Vector3((modelMat.m03 + 1) / 2.0f, (modelMat.m13 + 1) / 2.0f, modelMat.m23);
      Vector3 mmatv3 = Camera.main.ViewportToWorldPoint(tMPositionVector);
      modelMat.m03 *= 10;
      modelMat.m13 *= 10;
      modelMat.m23 *= 10;
      tModelMat = modelMat;
      tProjectionMat = projectionMat;
      // Camera.main.projectionMatrix = tProjectionMat;
    }
    private void getEulerAnglesFromMatrix(Matrix4x4 M, float[] outM)
    {
        outM[ 0] = 180.0f / Mathf.PI * Mathf.Atan2(M.m10, M.m11);
        outM[ 1] = 180.0f / Mathf.PI * Mathf.Atan2(-M.m02, M.m22);
        outM[ 2] = 180.0f / Mathf.PI * Mathf.Asin(-M.m12);
    }

    private AidlServiceContext AddSttAidlService() {
      //if(DEBUG) Debug.Log($":::::     AndroidServiceBridge::Add STT AidlService     :::::");
      
      // SERVICE_TYPE_STT_VVK or SERVICE_TYPE_STT_GCS
      AidlServiceContext context = new AidlServiceContext(SERVICE_TYPE_STT_GCS, AidlServiceContext.PNC_CAMERA_RGB);

      return context;

    }

    private void StopSttAidlService()
    {
      //if(DEBUG) Debug.Log($":::::     AndroidServiceBridge::Rmove STT AidlService     :::::");
      if (_aidlContextStt != null)
      {
        ServiceStop(_aidlContextStt.GetServiceType());
      }
    }

    private void TerminateSttAidlService()
    {
      //if(DEBUG) Debug.Log($":::::     AndroidServiceBridge::Rmove STT AidlService     :::::");
      if (_aidlContextStt != null)
      {
        ServiceTerm(_aidlContextStt.GetServiceType());
      }
    }

    public void RestartGestureService(string serviceType, string camId) {

    }


  } //AndroidService
} //namespace pnc
