using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class SvrManager : MonoBehaviour, SvrManager.SvrEventListener
{
    static bool DEBUG_LOG = false;

    public static SvrManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<SvrManager>();
            if (instance == null) {
              Debug.LogError("SvrManager object component not found");
            }
            return instance;
        }
    }
    private static SvrManager instance;

    static public int EyeLayerMax = 8;  // svrApi SVR_MAX_EYE_LAYERS
    static public int OverlayLayerMax = 8;  // svrApi SVR_MAX_OVERLAY_LAYERS
    static public int RenderLayersMax = 16;  // svrApi SVR_MAX_RENDER_LAYERS

    public class EnumFlagsAttribute : PropertyAttribute
    {
        public EnumFlagsAttribute() { }
    }

    public enum svrEventType
	{
        kEventNone = 0,
        kEventSdkServiceStarting = 1,
        kEventSdkServiceStarted = 2,
        kEventSdkServiceStopped = 3,
        kEventControllerConnecting = 4,
        kEventControllerConnected = 5,
        kEventControllerDisconnected = 6,
        kEventThermal = 7,
        kEventVrModeStarted = 8,
        kEventVrModeStopping = 9,
        kEventVrModeStopped = 10,
        kEventSensorError = 11,
        kEventMagnometerUncalibrated = 12,
        kEventBoundarySystemCollision = 13,
        kEvent6dofRelocation = 14,
        kEvent6dofWarningFeatureCount = 15,
        kEvent6dofWarningLowLight = 16,
        kEvent6dofWarningBrightLight = 17,
        kEvent6dofWarningCameraCalibration = 18,
        kEventProximity = 19,
    };

    //public enum LayerType
    //{
    //    RenderTexture = 0,
    //    StandardTexture = 1,
    //    EglTexture = 2,
    //    CameraTexture = 3,
    //};

    [Serializable]
    public class SvrSettings
    { 
        public enum eAntiAliasing
        {
            k1 = 1,
            k2 = 2,
            k4 = 4,
        };
		
        public enum eDepth
        {
            k16 = 16,
            k24 = 24
        };
		
        public enum eChromaticAberrationCorrection
        {
            kDisable = 0,
            kEnable = 1
        };
		
        public enum eVSyncCount
        {
            k1 = 1,
            k2 = 2,
        };
		
        public enum eMasterTextureLimit
        {
            k0 = 0, // full size
            k1 = 1, // half size
            k2 = 2, // quarter size
            k3 = 3, // ...
            k4 = 4  // ...
        };
		
        public enum ePerfLevel
        { 
            Minimum = 1,
            Medium = 2,
            Maximum = 3
        };

        [Flags] public enum eOptionFlags
        {
            ProtectedContent = (1 << 0),
            MotionAwareFrames = (1 << 1),
            FoveationSubsampled = (1 << 2),
            EnableCameraLayer = (1 << 3),
            Enable3drOcclusion = (1 << 4),
        }

        public enum eFoveationLevel
        {
            None = -1,
            Low = 0,
            Med = 1,
            High = 2
        }

        public enum eFrustumType
        {
            Camera = 0,
            Device = 1
        }

        public enum eEyeBufferType
        {
            //Mono = 0,
            StereoSeperate = 1,
            //StereoSingle = 2,
            //Array = 3,
        }

        public enum eCameraPassThruVideo
        {
            Disabled = 0,
            Enabled = 1,
        }

        public enum eTrackingRecenterMode
        {
            Disabled,
            Application,
            Device
        }

        [Tooltip("Use head tracking")]
        public bool UseHeadTracking = false;
        [Tooltip("If head tracking lost, fade the display")]
        public bool poseStatusFade = true;
        [Tooltip("Use eye tracking (if available)")]
        public bool trackEyes = true;
        [Tooltip("Use position tracking (if available)")]
        public bool trackPosition = true;
        [Tooltip("Track position conversion from meters")]
        public float trackPositionScale = 1;
        [Tooltip("Height of the eyes from base of head")]
        public float headHeight = 0.0750f;
        [Tooltip("Depth of the eyes from center of head")]
        public float headDepth = 0.0805f;
        [Tooltip("Distance between the eyes")]
        public float interPupilDistance = 0.064f;
        //[Tooltip("Distance of line-of-sight convergence (0 disabled)")]
        //public float stereoConvergence = 0;
        //[Tooltip("Pitch angle to the horizon in degrees")]
        //public float horizonElevation = 0;
        [Tooltip("Eye field of view render target reprojection margin (% of fov) [0..]")]
        public float eyeFovMargin = 0.0f;
        [Tooltip("Eye render target scale factor")]
        public float eyeResolutionScaleFactor = 1.0f;
        [Tooltip("Eye render target depth buffer")]
        public eDepth eyeDepth = eDepth.k24;
        [Tooltip("Eye render target MSAA value")]
        public eAntiAliasing eyeAntiAliasing = eAntiAliasing.k2;
        [Tooltip("Overlay render target scale factor")]
        public float overlayResolutionScaleFactor = 1.0f;
        [Tooltip("Overlay render target depth buffer")]
        public eDepth overlayDepth = eDepth.k16;
        [Tooltip("Overlay render target MSAA value")]
        public eAntiAliasing overlayAntiAliasing = eAntiAliasing.k1;
        [Tooltip("Limit refresh rate")]
        public eVSyncCount vSyncCount = eVSyncCount.k1;
        [Tooltip("Chromatic Aberration Correction")]
        public eChromaticAberrationCorrection chromaticAberationCorrection = eChromaticAberrationCorrection.kEnable;
        [Tooltip("QualitySettings TextureQuality FullRes, HalfRes, etc.")]
        public eMasterTextureLimit masterTextureLimit = eMasterTextureLimit.k0;
        [Tooltip("CPU performance level")]
        public ePerfLevel cpuPerfLevel = ePerfLevel.Medium;
        [Tooltip("GPU performance level")]
        public ePerfLevel gpuPerfLevel = ePerfLevel.Medium;
        [Tooltip("Initialization Option Flags: Protected, SpaceWarp")]
        [EnumFlags] public eOptionFlags optionFlags;
        [Tooltip("Foveation level: None, Low, Med, High")]
        public eFoveationLevel foveationLevel = eFoveationLevel.None;
        [Tooltip("Foveated render gain [1..], 0/feature disabled")]
        public Vector2 foveationGain = new Vector2(0.0f, 0.0f);
        [Tooltip("Foveated render hires area [0..]")]
        public float foveationArea = 0.0f;
        [Tooltip("Foveated render min pixel density [1/16..1]")]
        public float foveationMinimum = 0.0f;
        [Tooltip("Use perspective of unity camera or device frustum data")]
        public eFrustumType frustumType = eFrustumType.Camera;
        [Tooltip("Display buffer type (default stereo seperate)")]
        public eEyeBufferType displayType = eEyeBufferType.StereoSeperate;
        [Tooltip("Display camera pass-thru video")]
        public eCameraPassThruVideo cameraPassThruVideo = eCameraPassThruVideo.Disabled;
        [Tooltip("Tracking recenter mode")]
        public eTrackingRecenterMode trackingRecenterMode = eTrackingRecenterMode.Application;
    }
    [SerializeField]
    public SvrSettings settings;

    [Serializable]
    public class SvrStatus
    {
        [Tooltip("SnapdragonVR SDK Initialized")]
        public bool initialized = false;
        [Tooltip("SnapdragonVR SDK Running")]
        public bool running = false;
        [Tooltip("SnapdragonVR SDK Pose Status: 0/None, 1/Rotation, 2/Position, 3/RotationAndPosition")]
        public int pose = 0;
    }
    [SerializeField]
    public SvrStatus status;

    public enum eFadeState { FadeIn, FadeOut }
    [NonSerialized]
    public eFadeState fadeState = eFadeState.FadeIn;
    [NonSerialized]
    public float fadeDuration = .5f;

    [Header("Camera Rig")]
    public Transform head;
    public Transform gaze;
    public Camera monoCamera;
    public Camera leftCamera;
    public Camera rightCamera;
    public Camera leftOverlay;
    public Camera rightOverlay;
    public Camera monoOverlay;
    public SvrOverlay fadeOverlay;
    public SvrOverlay reticleOverlay;

    public bool Initialized { get { return status.initialized; } }
    public bool IsRunning { get { return status.running; } }
    public SvrPlugin.HeadPose HeadPose { get { return headPose; } }
    public SvrPlugin.EyePose EyePose { get { return eyePose; } }
    public Vector3 EyeDirection { get { return eyeDirection; } }
    //public Vector2 EyeFocus { get { return eyeFocus; } }

    public int FrameCount { get { return frameCount; } }
	
    private int	frameCount = 0;
    private int renderCount = 0;
	private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
	private SvrPlugin plugin = null;
    //private float sensorWarmupDuration = 0.25f;
    private List<SvrEye> eyes = new List<SvrEye>(EyeLayerMax);
	private List<SvrOverlay> overlays = new List<SvrOverlay>(OverlayLayerMax);
    private bool disableInput = false;
    private Coroutine onResume = null;
    private Coroutine submitFrame = null;
    private SvrPlugin.HeadPose headPose = new SvrPlugin.HeadPose { timestamp = 0, orientation = Quaternion.identity, position = Vector3.zero };
    private SvrPlugin.EyePose eyePose;
    private SvrPlugin.Foveation foveation;
    private Vector3 eyeDirection = Vector3.forward;
    private Vector2 eyeFocus = Vector2.zero;
    private Vector3 headRecenterPosition = Vector3.zero;
    private Quaternion headRecenterRotation = Quaternion.identity;
    private float eyeSmoother = 0.2f;
    private float[] frameDelta = new float[9];

    /// <summary>
    /// Svr event listener.
    /// </summary>
    public interface SvrEventListener {
		/// <summary>
		/// Raises the svr event event.
		/// </summary>
		/// <param name="ev">Ev.</param>
		void OnSvrEvent (SvrEvent ev);
	};

    public enum svrThermalLevel
    {
        kSafe,
        kLevel1,
        kLevel2,
        kLevel3,
        kCritical,
        kNumThermalLevels
    };

    public enum svrThermalZone
    {
        kCpu,
        kGpu,
        kSkin,
        kNumThermalZones
    };

    public struct svrEventData_Thermal
    {
        public svrThermalZone zone;               //!< Thermal zone
        public svrThermalLevel level;             //!< Indication of the current zone thermal level
    };

    public struct svrEventData_Proximity
    {
        public float distance;               //!< Proximity value in cm
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct svrEventData
    {
        [FieldOffset(0)]
        public svrEventData_Thermal thermal;
        [FieldOffset(0)]
        public svrEventData_Proximity proximity;
        [FieldOffset(0)]
        public Int64 data;
    }

	public struct SvrEvent
	{
		public svrEventType eventType;      //!< Type of event
		public uint deviceId;               //!< An identifier for the device that generated the event (0 == HMD)
		public float eventTimeStamp;        //!< Time stamp for the event in seconds since the last svrBeginVr call
        public svrEventData eventData;      //!< Event specific data payload
    };

	private List<SvrEventListener> 	eventListeners = new List<SvrEventListener>();

    public bool DisableInput
    {
        get { return disableInput; }
        set { disableInput = value; }
    }

	void Awake()
	{
		if (!ValidateReferencedComponents ())
		{
			enabled = false;
			return;
		}
        RegisterListeners();
        Input.backButtonLeavesApp = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = -1;
    }
	
    bool ValidateReferencedComponents()
	{
		plugin = SvrPlugin.Instance;
		if(plugin == null)
		{
			Debug.LogError("Svr Plugin failed to load. Disabling...");
			return false;
		}

		if(head == null)
		{
			Debug.LogError("Required head gameobject not found! Disabling...");
			return false;
		}

		if(monoCamera == null && (leftCamera == null || rightCamera == null))
		{
			Debug.LogError("Required eye components are missing! Disabling...");
			return false;
		}

		return true;
	}
#if UNITY_2019
    void OnEnable()
    {
        if (GraphicsSettings.renderPipelineAsset)
        {
            RenderPipelineManager.beginCameraRendering += OnBeginRender;
            RenderPipelineManager.endCameraRendering += OnEndRender;
        }
    }

    void OnDisable()
    {
        if (GraphicsSettings.renderPipelineAsset)
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginRender;
            RenderPipelineManager.endCameraRendering -= OnEndRender;
        }
    }

    private void OnBeginRender(ScriptableRenderContext context, Camera camera)
    {
        camera.SendMessage("OnPreRender", SendMessageOptions.DontRequireReceiver);
    }

    private void OnEndRender(ScriptableRenderContext context, Camera camera)
    {
        camera.SendMessage("OnPostRender", SendMessageOptions.DontRequireReceiver);
    }
#elif UNITY_2018
    void OnEnable()
    {
        if (GraphicsSettings.renderPipelineAsset)
        {
            UnityEngine.Experimental.Rendering.RenderPipeline.beginCameraRendering += OnBeginRender;
            //RenderPipeline.endCameraRendering += OnEndRender;
        }
    }

    void OnDisable()
    {
        if (GraphicsSettings.renderPipelineAsset)
        {
            UnityEngine.Experimental.Rendering.RenderPipeline.beginCameraRendering -= OnBeginRender;
            //RenderPipeline.endCameraRendering -= OnEndRender;
        }
    }

    private void OnBeginRender(Camera camera)
    {
        camera.SendMessage("OnPreRender", SendMessageOptions.DontRequireReceiver);
    }

    private void OnEndRender(Camera camera)
    {
        camera.SendMessage("OnPostRender", SendMessageOptions.DontRequireReceiver);
    }
#endif
    IEnumerator Start ()
	{
		yield return StartCoroutine(Initialize());
        status.initialized = plugin.IsInitialized();

        SetOverlayFade(eFadeState.FadeIn);

        yield return StartCoroutine(plugin.BeginVr((int)settings.cpuPerfLevel, (int)settings.gpuPerfLevel, (int)settings.optionFlags));

        yield return null;  // Wait one frame - fix for multi-threaded rendering

        if (!plugin.IsRunning())
        {
            Debug.LogError("Svr Start Failed");
            Application.Quit();
            yield return null;  // Wait one frame
        }

        float recenterTimeout = 1f;
        while (!RecenterTracking() && recenterTimeout > 0f)
        {
            yield return null;  // Wait one frame
            recenterTimeout -= Time.deltaTime;
        }

        //yield return new WaitForSecondsRealtime(sensorWarmupDuration);

        submitFrame = StartCoroutine(SubmitFrame());

        //Register for SvrEvents
        AddEventListener(this);

        status.running = true;

        Debug.Log("Svr initialized!");

        if (SystemInfo.graphicsMultiThreaded)
        {
            Debug.Log("sxrUnity MultiThreaded Rendering is Enabled");
        }
    }

	IEnumerator Initialize()
	{
        Debug.Log("SvrManager.Initialize()");

        // Plugin must be initialized OnStart in order to properly
        // get a valid surface
        GameObject mainCameraGo = GameObject.FindWithTag("MainCamera");
        if (mainCameraGo)
        {
            mainCameraGo.SetActive(false);

            Debug.Log("Camera with MainCamera tag found.");
            if (!disableInput)
            {
                Debug.Log("Will use translation and orientation from the MainCamera.");
                transform.position = mainCameraGo.transform.position;
                transform.rotation = mainCameraGo.transform.rotation;
            }

            Debug.Log("Disabling Camera with MainCamera tag");
        }

        GL.Clear(false, true, Color.black);

		yield return StartCoroutine(plugin.Initialize());
        InitializeCameras();
		InitializeEyes();
        InitializeOverlays();
        InitializeFoveation();

        int trackingMode = (int)SvrPlugin.TrackingMode.kTrackingOrientation;
        if (settings.trackPosition)
            trackingMode |= (int)SvrPlugin.TrackingMode.kTrackingPosition;
        if (settings.trackEyes)
            trackingMode |= (int)SvrPlugin.TrackingMode.kTrackingEye;

        plugin.SetTrackingMode(trackingMode);

        if (settings.cameraPassThruVideo == SvrSettings.eCameraPassThruVideo.Enabled)
            settings.optionFlags |= SvrSettings.eOptionFlags.EnableCameraLayer;

        plugin.SetVSyncCount((int)settings.vSyncCount);
        QualitySettings.vSyncCount = (int)settings.vSyncCount;
    }

    private void InitializeFoveation()
    {
        foveation = GetFoveationValues();

        Debug.LogFormat("Foveation Parameters: Area {0} Gain {1}, {2} Minimum {3}", foveation.Area, foveation.Gain.x, foveation.Gain.y, foveation.Minimum);
    }

    private void InitializeCameras()
    {
        float stereoConvergence = plugin.deviceInfo.targetFrustumConvergence; //settings.stereoConvergence
        float horizonElevation = plugin.deviceInfo.targetFrustumPitch; //settings.horizonElevation
        float convergenceAngle = 0f;
        if (stereoConvergence > Mathf.Epsilon) convergenceAngle = Mathf.Rad2Deg * Mathf.Atan2(0.5f * settings.interPupilDistance, stereoConvergence);
        else if (stereoConvergence < -Mathf.Epsilon) convergenceAngle = -Mathf.Rad2Deg * Mathf.Atan2(0.5f * settings.interPupilDistance, -stereoConvergence);

        Vector3 eyePos;
        Quaternion eyeRot;
        // left
        eyePos = plugin.deviceInfo.targetFrustumLeft.position;
        if (Mathf.Approximately(eyePos.x, 0)) eyePos.x = -0.5f * settings.interPupilDistance;
        if (!settings.trackPosition) eyePos.y += settings.headHeight;
        if (!settings.trackPosition) eyePos.z += settings.headDepth;
        eyePos += head.transform.localPosition;

        eyeRot = plugin.deviceInfo.targetFrustumLeft.rotation * Quaternion.Euler(horizonElevation, convergenceAngle, 0);

        if (leftCamera != null)
        {
            leftCamera.transform.localPosition = eyePos;
            leftCamera.transform.localRotation = eyeRot;
        }
        if (leftOverlay != null)
        {
            leftOverlay.transform.localPosition = eyePos;
            leftOverlay.transform.localRotation = eyeRot;
        }

        // right
        eyePos = plugin.deviceInfo.targetFrustumRight.position;
        if (Mathf.Approximately(eyePos.x, 0)) eyePos.x = 0.5f * settings.interPupilDistance;
        if (!settings.trackPosition) eyePos.y += settings.headHeight;
        if (!settings.trackPosition) eyePos.z += settings.headDepth;
        eyePos += head.transform.localPosition;

        eyeRot = plugin.deviceInfo.targetFrustumRight.rotation * Quaternion.Euler(horizonElevation, -convergenceAngle, 0);

        if (rightCamera != null)
        {
            rightCamera.transform.localPosition = eyePos;
            rightCamera.transform.localRotation = eyeRot;
        }
        if (rightOverlay != null)
        {
            rightOverlay.transform.localPosition = eyePos;
            rightOverlay.transform.localRotation = eyeRot;
        }

        // mono
        eyePos = Vector3.zero;
        if (!settings.trackPosition) eyePos.y += settings.headHeight;
        if (!settings.trackPosition) eyePos.z += settings.headDepth;
        eyePos += head.transform.localPosition;

        eyeRot = Quaternion.Euler(horizonElevation, 0, 0);

        if (monoCamera != null)
        {
            monoCamera.transform.localPosition = eyePos;
            monoCamera.transform.localRotation = eyeRot;
        }
        if (monoOverlay != null)
        {
            monoOverlay.transform.localPosition = eyePos;
            monoOverlay.transform.localRotation = eyeRot;
        }
    }

    private void AddEyes(Camera cam, SvrPlugin.EyeMask side)
    {
        bool enableCamera = false;
        var eyesFound = cam.gameObject.GetComponents<SvrEye>();
        for (int i = 0; i < eyesFound.Length; i++)
        {
            eyesFound[i].Side = side;
            if (eyesFound[i].imageType == SvrEye.eType.RenderTexture) enableCamera = true;
        }
        eyes.AddRange(eyesFound);
        if (eyesFound.Length == 0)
        {
            var eye = cam.gameObject.AddComponent<SvrEye>();
            eye.Side = side;
            eyes.Add(eye);
            enableCamera = true;
        }
#if UNITY_5_4 || UNITY_5_5
        cam.hdr = false;
#else // UNITY_5_6 plus
        cam.allowHDR = false;
        cam.allowMSAA = false;
#endif
        cam.enabled = enableCamera;
    }

	private void InitializeEyes()
	{
        eyes.Clear();
        if (monoCamera != null && monoCamera.gameObject.activeSelf)
        {
            AddEyes(monoCamera, SvrPlugin.EyeMask.kBoth);
        }
        if (leftCamera != null && leftCamera.gameObject.activeSelf)
        {
            AddEyes(leftCamera, SvrPlugin.EyeMask.kLeft);
        }
        if (rightCamera != null && rightCamera.gameObject.activeSelf)
        {
            AddEyes(rightCamera, SvrPlugin.EyeMask.kRight);
        }
        for (int i = 0; i < SvrEye.Instances.Count; i++)
        {
            var eye = SvrEye.Instances[i];
            if (!eyes.Contains(eye))
            {
                eyes.Add(eye); // Add eyes found outside of svr camera hierarchy
            }
        }

        SvrPlugin.DeviceInfo info = plugin.deviceInfo;

		foreach(SvrEye eye in eyes)
		{
            if (eye == null) continue;

            eye.FovMargin               = settings.eyeFovMargin;
			eye.Format					= RenderTextureFormat.Default;
			eye.Resolution  			= new Vector2(info.targetEyeWidthPixels, info.targetEyeHeightPixels);
            eye.ResolutionScaleFactor   = settings.eyeResolutionScaleFactor;
			eye.Depth 					= (int)settings.eyeDepth;
			eye.AntiAliasing 			= (int)settings.eyeAntiAliasing;	// hdr not supported with antialiasing
            eye.FrustumType             = (int)settings.frustumType;
            eye.OnPreRenderListener     = OnPreRenderListener;
            eye.OnPostRenderListener    = OnPostRenderListener;
            /*if (plugin.Is3drOcclusion())*/ eye.OnOccludeRenderListener = OnOccludeRenderListener;

            eye.Initialize();
		}
	}

    private void AddOverlays(Camera cam, SvrPlugin.EyeMask side)
    {
        bool enableCamera = false;
        var overlaysFound = cam.gameObject.GetComponents<SvrOverlay>();
        for (int i = 0; i < overlaysFound.Length; i++)
        {
            overlaysFound[i].Side = side;
            if (overlaysFound[i].imageType == SvrOverlay.eType.RenderTexture) enableCamera = true;
        }
        overlays.AddRange(overlaysFound);
        if (overlaysFound.Length == 0)
        {
            var overlay = cam.gameObject.AddComponent<SvrOverlay>();
            overlay.Side = side;
            overlays.Add(overlay);
            enableCamera = true;
        }
#if UNITY_5_4 || UNITY_5_5
        cam.hdr = false;
#else // UNITY_5_6 plus
        cam.allowHDR = false;
        cam.allowMSAA = false;
#endif
        cam.enabled = enableCamera;
    }

    private void InitializeOverlays()
    {
        overlays.Clear();
        if (leftOverlay != null && leftOverlay.gameObject.activeSelf)
        {
            AddOverlays(leftOverlay, SvrPlugin.EyeMask.kLeft);
        }
        if (rightOverlay != null && rightOverlay.gameObject.activeSelf)
        {
            AddOverlays(rightOverlay, SvrPlugin.EyeMask.kRight);
        }
        if (monoOverlay != null && monoOverlay.gameObject.activeSelf)
        {
            AddOverlays(monoOverlay, SvrPlugin.EyeMask.kBoth);
        }
        for (int i = 0; i < SvrOverlay.Instances.Count; i++)
        {
            var overlay = SvrOverlay.Instances[i];
            if (!overlays.Contains(overlay))
            {
                overlays.Add(overlay); // Add overlays found outside of svr camera hierarchy
            }
        }

        SvrPlugin.DeviceInfo info = plugin.deviceInfo;

        foreach (SvrOverlay overlay in overlays)
        {
            if (overlay == null) continue;

            overlay.Format                  = RenderTextureFormat.Default;
            overlay.Resolution              = new Vector2(info.targetEyeWidthPixels, info.targetEyeHeightPixels);
            overlay.ResolutionScaleFactor   = settings.overlayResolutionScaleFactor;
            overlay.Depth                   = (int)settings.overlayDepth;
            overlay.AntiAliasing            = (int)settings.overlayAntiAliasing;  // hdr not supported with antialiasing
            overlay.FrustumType             = (int)settings.frustumType;
            //overlay.OnPreRenderListener     = OnPreRenderListener;
            //overlay.OnPostRenderListener    = OnPostRenderListener;

            overlay.Initialize();
        }
    }

    public void SetOverlayFade(eFadeState fadeValue)
    {
        fadeState = fadeValue;
        var startAlpha = fadeState == eFadeState.FadeIn ? 1f : 0f;
        UpdateOverlayFade(startAlpha);
    }

    public bool IsOverlayFading()
    {
        return !Mathf.Approximately((float)fadeState, fadeAlpha);
    }

    private float fadeAlpha = 0f;
    private void UpdateOverlayFade(float targetAlpha, float rate = 0)
    {
        if (fadeOverlay == null) return;

        fadeAlpha = rate > 0 ? Mathf.MoveTowards(fadeAlpha, targetAlpha, rate) : targetAlpha;

        var fadeTexture = fadeOverlay.imageTexture as Texture2D;
        if (fadeTexture != null)
        {
            var fadeColors = fadeTexture.GetPixels();
            for (int i = 0; i < fadeColors.Length; ++i)
            {
                fadeColors[i].a = fadeAlpha;
            }
            fadeTexture.SetPixels(fadeColors);
            fadeTexture.Apply(false);
        }

        var isActive = fadeAlpha > 0.0f;
        if (fadeOverlay.enabled != isActive)
        {
            fadeOverlay.enabled = isActive;
        }
    }

    private SvrPlugin.Foveation GetFoveationValues()
    {
        SvrPlugin.Foveation foveationValues = new SvrPlugin.Foveation();

        // Table lookup by level index
        switch (settings.foveationLevel)
        {
            case SvrSettings.eFoveationLevel.Low:
                foveationValues = plugin.deviceInfo.lowFoveation;
                break;
            case SvrSettings.eFoveationLevel.Med:
                foveationValues = plugin.deviceInfo.medFoveation;
                break;
            case SvrSettings.eFoveationLevel.High:
                foveationValues = plugin.deviceInfo.highFoveation;
                break;
            case SvrSettings.eFoveationLevel.None:
                foveationValues.Area = 0.0f;
                foveationValues.Gain = Vector2.one;
                foveationValues.Minimum = 0.0f;
                break;
        }

        // Apply overrides: SvrOverrideSettings, SvrManager.Settings
        if (SvrOverrideSettings.FoveateArea > 0f)
            foveationValues.Area = SvrOverrideSettings.FoveateArea;
        else if (settings.foveationArea > 0f)
            foveationValues.Area = settings.foveationArea;

        if (SvrOverrideSettings.FoveateGain != Vector2.zero)
            foveationValues.Gain = SvrOverrideSettings.FoveateGain;
        else if (settings.foveationGain != Vector2.zero)
            foveationValues.Gain = settings.foveationGain;

        if (SvrOverrideSettings.FoveateMinimum > 0f)
            foveationValues.Minimum = SvrOverrideSettings.FoveateMinimum;
        else if (settings.foveationMinimum > 0f)
            foveationValues.Minimum = settings.foveationMinimum;

        //Debug.LogFormat("Foveation: Area {0} Gain {1}, {2} Minimum {3}", foveationValues.Area, foveationValues.Gain.x, foveationValues.Gain.y, foveationValues.Minimum);

        return foveationValues;
    }

    IEnumerator SubmitFrame ()
	{
        Vector3 frustumSize = Vector3.zero;
        frustumSize.x = 0.5f * (plugin.deviceInfo.targetFrustumLeft.right - plugin.deviceInfo.targetFrustumLeft.left);
        frustumSize.y = 0.5f * (plugin.deviceInfo.targetFrustumLeft.top - plugin.deviceInfo.targetFrustumLeft.bottom);
        frustumSize.z = plugin.deviceInfo.targetFrustumLeft.near;
        //Debug.LogFormat("SubmitFrame: Frustum Size ({0:F2}, {1:F2}, {2:F2})", frustumSize.x, frustumSize.y, frustumSize.z);

        while (true)
		{
			yield return waitForEndOfFrame;

            // Todo: Remove
            //if (SystemInfo.graphicsMultiThreaded == false)
            //{
            //    if ((plugin.GetTrackingMode() & (int)SvrPlugin.TrackingMode.kTrackingEye) != 0) // Request eye pose
            //    {
            //        status.pose |= plugin.GetEyePose(ref eyePose);
            //    }
            //    var eyePoint = Vector3.zero;
            //    if ((status.pose & (int)SvrPlugin.TrackingMode.kTrackingEye) != 0) // Valid eye pose
            //    {
            //        //Debug.LogFormat("Left Eye Position: {0}, Direction: {1}", eyePose.leftPosition.ToString(), eyePose.leftDirection.ToString());
            //        //Debug.LogFormat("Right Eye Position: {0}, Direction: {1}", eyePose.rightPosition.ToString(), eyePose.rightDirection.ToString());
            //        //Debug.LogFormat("Combined Eye Position: {0}, Direction: {1}", eyePose.combinedPosition.ToString(), eyePose.combinedDirection.ToString());
            //        var combinedDirection = Vector3.zero;
            //        if ((eyePose.combinedStatus & (int)SvrPlugin.EyePoseStatus.kGazeVectorValid) != 0)
            //        {
            //            combinedDirection = eyePose.combinedDirection;
            //        }
            //        else
            //        {
            //            if ((eyePose.leftStatus & (int)SvrPlugin.EyePoseStatus.kGazeVectorValid) != 0) combinedDirection += eyePose.leftDirection;
            //            if ((eyePose.rightStatus & (int)SvrPlugin.EyePoseStatus.kGazeVectorValid) != 0) combinedDirection += eyePose.rightDirection;
            //        }
            //        if (combinedDirection.sqrMagnitude > 0f)
            //        {
            //            combinedDirection.Normalize();
            //            //Debug.LogFormat("Eye Direction: ({0:F2}, {1:F2}, {2:F2})", combinedDirection.x, combinedDirection.y, combinedDirection.z);

            //            float denominator = Vector3.Dot(combinedDirection, Vector3.forward);
            //            if (denominator > float.Epsilon)
            //            {
            //                // eye direction intersection with frustum near plane (left)
            //                eyePoint = combinedDirection * frustumSize.z / denominator;
            //                eyePoint.x /= frustumSize.x; // [-1..1]
            //                eyePoint.y /= frustumSize.y; // [-1..1]

            //                //Debug.LogFormat("Eye Point: ({0:F2}, {1:F2})", eyePoint.x, eyePoint.y);
            //            }

            //            for (int i = 0; i < renderCount; i++)
            //            {
            //                plugin.SetFoveationParameters(i, 0, 0, eyePoint.x, eyePoint.y, foveation.Gain.x, foveation.Gain.y, foveation.Area, foveation.Minimum);
            //                plugin.ApplyFoveation();
            //            }
            //        }
            //    }
            //}

            var horizontalFieldOfView = 0f;
            if (settings.eyeFovMargin > 0f)
            {
                horizontalFieldOfView = (monoCamera.enabled ? monoCamera.fieldOfView / monoCamera.aspect : leftCamera.fieldOfView / leftCamera.aspect) * Mathf.Deg2Rad;
            }
            plugin.SubmitFrame(frameCount, horizontalFieldOfView, (int)settings.displayType);
			
			frameCount++;
		}
	}

	public bool RecenterTracking()
	{
        if (!Initialized)
            return false;

        if (settings.trackingRecenterMode == SvrSettings.eTrackingRecenterMode.Disabled)
            return true;
        else if (settings.trackingRecenterMode == SvrSettings.eTrackingRecenterMode.Application)
            return SetHeadRecenter();
        else if (settings.trackingRecenterMode == SvrSettings.eTrackingRecenterMode.Device)
            return plugin.RecenterTracking();
        else
            return false;
	}

	void OnPreRenderListener (int sideMask, int textureId, int previousId)
	{
        if (!IsRunning)
            return;

        plugin.BeginEye(renderCount, sideMask, frameDelta);
        if ((plugin.GetTrackingMode() & (int)SvrPlugin.TrackingMode.kTrackingEye) != 0)
        {
            plugin.GetEyeFocalPoint(ref eyeFocus);
        }
        plugin.SetFoveationParameters(renderCount, textureId, previousId, eyeFocus.x, eyeFocus.y, foveation.Gain.x, foveation.Gain.y, foveation.Area, foveation.Minimum);
        plugin.ApplyFoveation();
    }

    void OnOccludeRenderListener (Matrix4x4 projMat, Matrix4x4 viewMat)
    {
        if (!IsRunning)
            return;

        if (!plugin.Is3drOcclusion())
            return;

        plugin.OccludeEye(renderCount, projMat, viewMat);
        //plugin.ApplyOcclusion();
    }

    void OnPostRenderListener (int sideMask, int layerMask)
	{
        if (!IsRunning)
            return;

        plugin.EndEye (renderCount, sideMask, layerMask);
        renderCount++;
	}

    public void SetPause(bool pause)
	{
        if (!Initialized)
			return;

        if (pause /*&& IsRunning*/)
		{
			OnPause();
		}
        else if (onResume == null /*&& IsRunning == false*/)
        {
            onResume = StartCoroutine(OnResume());
		}
    }

    void OnPause()
	{
        if(DEBUG_LOG) Debug.Log("SvrManager.Pause()");

        status.running = false;

        if (submitFrame != null) { StopCoroutine(submitFrame); submitFrame = null; }

        /*if (plugin.IsRunning())*/ plugin.EndVr();
        onResume = null;
    }

    IEnumerator OnResume()
	{
        if(DEBUG_LOG) Debug.Log("SvrManager.Resume()");

        SetOverlayFade(eFadeState.FadeIn);

        yield return StartCoroutine(plugin.BeginVr((int)settings.cpuPerfLevel, (int)settings.gpuPerfLevel, (int)settings.optionFlags));

        float recenterTimeout = 1f;
        while (!RecenterTracking() && recenterTimeout > 0f)
        {
            yield return null;  // Wait one frame
            recenterTimeout -= Time.deltaTime;
        }

        //yield return new WaitForSecondsRealtime(sensorWarmupDuration);

        if (submitFrame == null) { submitFrame = StartCoroutine(SubmitFrame()); }

        status.running = plugin.IsRunning();
	}

    public void SetSuspend(bool suspend)
    {
        if (suspend)
        {
            plugin.PauseXr();
        }
        else
        {
            plugin.ResumeXr();
        }
    }

    //sjy test for pause -> resume
    //bool bFirstdo = true;

    public void onclickfortest()
    {
      
        //if (bFirstdo)
        //{
        //    Debug.LogFormat("oncLIcked!!!!");
        //    SetPause(true);
        //    Debug.LogFormat("oncLIcked@@@@");

        //    SetPause(false);
        //    bFirstdo = false;
        //}
    }



    public bool SetHeadRecenter()
    {
        if (!IsRunning)
        {
            return false;
        }

        headRecenterPosition = -headPose.position;
        //headRecenterRotation = Quaternion.Inverse(headPose.orientation);
        var headRotation = Quaternion.Euler(0, headPose.orientation.eulerAngles.y, 0); // yaw only
        headRecenterRotation = Quaternion.Inverse(headRotation);

        return true;
    }

    void LateUpdate()
    {
        if (!IsRunning)
        {
            return;
        }

        int trackingMode = plugin.GetTrackingMode();
        var prevOrientation = headPose.orientation;

        status.pose = plugin.GetHeadPose(ref headPose, frameCount);

        if ((trackingMode & (int)SvrPlugin.TrackingMode.kTrackingEye) != 0)
        {
            status.pose |= plugin.GetEyePose(ref eyePose, frameCount);
        }

        //if (!disableInput&& !IntroManager.isPlayingVideo)
        if (!disableInput)
        {
            if ((status.pose & (int)SvrPlugin.TrackingMode.kTrackingOrientation) != 0)
            {
                //sjy head rotation
                if (settings.UseHeadTracking == true) head.transform.localRotation = headRecenterRotation * headPose.orientation;

                // delta orientation screen space x, y offset for foveated rendering
                var deltaOrientation = Quaternion.Inverse(prevOrientation) * headPose.orientation;
                var lookDirection = deltaOrientation * Vector3.forward;
                //Debug.LogFormat("Look Direction: {0}", lookDirection.ToString());
                lookDirection *= plugin.deviceInfo.targetFrustumLeft.near / lookDirection.z;
                float xTotal = 0.5f * (plugin.deviceInfo.targetFrustumLeft.right - plugin.deviceInfo.targetFrustumLeft.left);
                float xAdjust = (xTotal != 0.0f) ? lookDirection.x / xTotal : 0.0f;
                float yTotal = 0.5f * (plugin.deviceInfo.targetFrustumLeft.top - plugin.deviceInfo.targetFrustumLeft.bottom);
                float yAdjust = (yTotal != 0.0f) ? lookDirection.y / yTotal : 0.0f;
                //xAdjust *= 0.5f * plugin.deviceInfo.targetEyeWidthPixels;
                //yAdjust *= 0.5f * plugin.deviceInfo.targetEyeHeightPixels;

                // rotation around z [cos(z), sin(z), 0], [-sin(z), cos(z), 0], [0, 0, 1]
                Vector3 deltaEulers = deltaOrientation.eulerAngles;
                float cosZ = Mathf.Cos(deltaEulers.z * Mathf.Deg2Rad);
                float sinZ = Mathf.Sin(deltaEulers.z * Mathf.Deg2Rad);

                // Output rotation and translation
                frameDelta[0] = cosZ;
                frameDelta[1] = sinZ;
                frameDelta[2] = 0.0f;

                frameDelta[3] = -sinZ;
                frameDelta[4] = cosZ;
                frameDelta[5] = 0.0f;

                frameDelta[6] = xAdjust;
                frameDelta[7] = yAdjust;
                frameDelta[8] = 1.0f;
            }
            if (settings.trackPosition && (status.pose & (int)SvrPlugin.TrackingMode.kTrackingPosition) != 0)
            {
                //sjy head position
                head.transform.localPosition = headRecenterPosition + headPose.position * settings.trackPositionScale;
            }
            if ((status.pose & (int)SvrPlugin.TrackingMode.kTrackingEye) != 0)
            {
                //Debug.LogFormat("Left Eye Position: ({0:F2}, {1:F2}, {2:F2}), Direction: ({3:F2}, {4:F2}, {5:F2})",
                //    eyePose.leftPosition.x, eyePose.leftPosition.y, eyePose.leftPosition.z, eyePose.leftDirection.x, eyePose.leftDirection.y, eyePose.leftDirection.z);
                //Debug.LogFormat("Right Eye Position: ({0:F2}, {1:F2}, {2:F2}), Direction: ({3:F2}, {4:F2}, {5:F2})",
                //    eyePose.rightPosition.x, eyePose.rightPosition.y, eyePose.rightPosition.z, eyePose.rightDirection.x, eyePose.rightDirection.y, eyePose.rightDirection.z);
                //Debug.LogFormat("Combined Eye Position: ({0:F2}, {1:F2}, {2:F2}), Direction: ({3:F2}, {4:F2}, {5:F2})",
                //    eyePose.combinedPosition.x, eyePose.combinedPosition.y, eyePose.combinedPosition.z, eyePose.combinedDirection.x, eyePose.combinedDirection.y, eyePose.combinedDirection.z);
                var combinedDirection = Vector3.zero;
                if ((eyePose.combinedStatus & (int)SvrPlugin.EyePoseStatus.kGazeVectorValid) != 0)
                {
                    combinedDirection = eyePose.combinedDirection;
                }
                else
                {
                    if ((eyePose.leftStatus & (int)SvrPlugin.EyePoseStatus.kGazeVectorValid) != 0) combinedDirection += eyePose.leftDirection;
                    if ((eyePose.rightStatus & (int)SvrPlugin.EyePoseStatus.kGazeVectorValid) != 0) combinedDirection += eyePose.rightDirection;
                }
                if (combinedDirection.sqrMagnitude > 0f)
                {
                    combinedDirection.Normalize();
                    //Debug.LogFormat("Eye Direction: ({0:F2}, {1:F2}, {2:F2})", combinedDirection.x, combinedDirection.y, combinedDirection.z);

                    eyeDirection = eyeSmoother > 0.001f ? Vector3.Lerp(eyeDirection, combinedDirection, eyeSmoother) : combinedDirection;

                    //var combinedPosition = Vector3.zero;
                    //if ((eyePose.leftStatus & (int)SvrPlugin.EyePoseStatus.kGazePointValid) != 0) combinedPosition += eyePose.leftPosition;
                    //if ((eyePose.rightStatus & (int)SvrPlugin.EyePoseStatus.kGazePointValid) != 0) combinedPosition += eyePose.rightPosition;
                    ////if ((eyePose.combinedStatus & (int)SvrPlugin.EyePoseStatus.kGazePointValid) != 0) combinedPosition += eyePose.combinedPosition;

                    gaze.localPosition = monoCamera.transform.localPosition;
                    gaze.localRotation = Quaternion.LookRotation(eyeDirection, Vector3.up);

                    float denominator = Vector3.Dot(eyeDirection, Vector3.forward);
                    if (denominator > float.Epsilon)
                    {
                        // eye direction intersection with frustum near plane (left)
                        var eyePoint = eyeDirection * plugin.deviceInfo.targetFrustumLeft.near / denominator;

                        // size of the frustum near plane (left)
                        var nearSize = new Vector2(0.5f*(plugin.deviceInfo.targetFrustumLeft.right - plugin.deviceInfo.targetFrustumLeft.left),
                            0.5f*(plugin.deviceInfo.targetFrustumLeft.top - plugin.deviceInfo.targetFrustumLeft.bottom));

                        eyeFocus.Set(eyePoint.x / nearSize.x, eyePoint.y / nearSize.y);   // Normalized [-1,1]
                        //Debug.LogFormat("Eye Focus: {0}", eyeFocus.ToString());
                    }
                }
            }
        }

        var isValid = true;

        //sjy fade control
        //if (settings.poseStatusFade)
        //{
        //    isValid = !settings.trackPosition
        //        || ((trackingMode & (int)SvrPlugin.TrackingMode.kTrackingPosition) == 0)
        //        || (status.pose & (int)SvrPlugin.TrackingMode.kTrackingPosition) != 0;
        //}

        var targetAlpha = fadeState == eFadeState.FadeOut || !isValid ? 1f : 0f;
        UpdateOverlayFade(targetAlpha, Time.deltaTime / fadeDuration);

        foveation = GetFoveationValues();

        if (plugin.Is3drOcclusion())
        {
            plugin.GetOcclusionMesh();
        }

        renderCount = 0;
    }

    public void Shutdown()
	{
        Debug.Log("SvrManager.Shutdown()");

        status.running = false;

        if (submitFrame != null) { StopCoroutine(submitFrame); submitFrame = null; }

        if (plugin.IsRunning()) plugin.EndVr();

        if (plugin.IsInitialized()) plugin.Shutdown();

        status.initialized = false;
    }

    public void OnSvrEvent(SvrManager.SvrEvent ev)
    {
        switch (ev.eventType)
        {
            case SvrManager.svrEventType.kEventProximity:
                if(DEBUG_LOG) Debug.LogFormat("OnSvrEvent: Proximity Distance {0}, State {1}", ev.eventData.proximity.distance, ev.eventData.proximity.distance > 0.0f);
                //SetPause((bool)(ev.eventData.proximity.distance > 0.0f));
                //SetSuspend((bool)(ev.eventData.proximity.distance > 0.0f));
                //SetSuspend(false);              //simon 220722 근접 센서 동작 여부와 관계없도록 수정
                break;
            case SvrManager.svrEventType.kEventVrModeStarted:
                break;
        }
    }

    void OnDestroy()
    {
        Debug.Log("SvrManager.OnDestroy()");

        UnregisterListeners();

        Shutdown();
    }

    void OnApplicationPause(bool pause)
	{
        Debug.LogFormat("SvrManager.OnApplicationPause({0})",pause);

        SetPause(pause);

        //if(pause) Shutdown();
	}

	void OnApplicationQuit()
	{
        Debug.Log("SvrManager.OnApplicationQuit()");

        Shutdown();
	}

    static public Matrix4x4 Perspective(float left, float right, float bottom, float top, float near, float far)
    {
        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0F * far * near) / (far - near);
        float e = -1.0F;
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }

    void RegisterListeners()
    {
        SvrOverrideSettings.OnEyeAntiAliasingChangedEvent += OnEyeAntiAliasingChanged;
        SvrOverrideSettings.OnEyeDepthChangedEvent += OnEyeDepthChanged;
        SvrOverrideSettings.OnEyeResolutionScaleFactorChangedEvent += OnEyeResolutionScaleFactorChanged;
        SvrOverrideSettings.OnOverlayAntiAliasingChangedEvent += OnOverlayAntiAliasingChanged;
        SvrOverrideSettings.OnOverlayDepthChangedEvent += OnOverlayDepthChanged;
        SvrOverrideSettings.OnOverlayResolutionScaleFactorChangedEvent += OnOverlayResolutionScaleFactorChanged;
        SvrOverrideSettings.OnChromaticAberrationCorrectionChangedEvent += OnChromaticAberrationCorrectionChanged;
        SvrOverrideSettings.OnVSyncCountChangedEvent += OnVSyncCountChanged;
        SvrOverrideSettings.OnMasterTextureLimitChangedEvent += OnMasterTextureLimitChanged;
        SvrOverrideSettings.OnPerfLevelChangedEvent += OnPerfLevelChanged;
        SvrOverrideSettings.OnFoveateChangedEvent += OnFoveateChanged;
    }
	
    void UnregisterListeners()
    {
        SvrOverrideSettings.OnEyeAntiAliasingChangedEvent -= OnEyeAntiAliasingChanged;
        SvrOverrideSettings.OnEyeDepthChangedEvent -= OnEyeDepthChanged;
        SvrOverrideSettings.OnEyeResolutionScaleFactorChangedEvent -= OnEyeResolutionScaleFactorChanged;
        SvrOverrideSettings.OnOverlayAntiAliasingChangedEvent -= OnOverlayAntiAliasingChanged;
        SvrOverrideSettings.OnOverlayDepthChangedEvent -= OnOverlayDepthChanged;
        SvrOverrideSettings.OnOverlayResolutionScaleFactorChangedEvent -= OnOverlayResolutionScaleFactorChanged;
        SvrOverrideSettings.OnChromaticAberrationCorrectionChangedEvent -= OnChromaticAberrationCorrectionChanged;
        SvrOverrideSettings.OnVSyncCountChangedEvent -= OnVSyncCountChanged;
        SvrOverrideSettings.OnMasterTextureLimitChangedEvent -= OnMasterTextureLimitChanged;
        SvrOverrideSettings.OnPerfLevelChangedEvent -= OnPerfLevelChanged;
        SvrOverrideSettings.OnFoveateChangedEvent -= OnFoveateChanged;
    }

    void OnEyeAntiAliasingChanged(SvrOverrideSettings.eAntiAliasing antiAliasing)
    {
        foreach (SvrEye eye in eyes)
        {
            eye.AntiAliasing = antiAliasing == SvrOverrideSettings.eAntiAliasing.NoOverride ? 
                (int)settings.eyeAntiAliasing : (int)antiAliasing;
        }
    }
	
    void OnEyeDepthChanged(SvrOverrideSettings.eDepth depth)
    {
        foreach (SvrEye eye in eyes)
        {
            eye.Depth = depth == SvrOverrideSettings.eDepth.NoOverride ?
                (int)settings.eyeDepth : (int)depth;
        }
    }
	
    void OnEyeResolutionScaleFactorChanged(float scaleFactor)
    {
        foreach (SvrEye eye in eyes)
        {
            eye.ResolutionScaleFactor = scaleFactor <= 0 ? settings.eyeResolutionScaleFactor : scaleFactor;
        }
    }
	
    void OnOverlayAntiAliasingChanged(SvrOverrideSettings.eAntiAliasing antiAliasing)
    {
        foreach (SvrOverlay overlay in overlays)
        {
            overlay.AntiAliasing = antiAliasing == SvrOverrideSettings.eAntiAliasing.NoOverride ?
                (int)settings.overlayAntiAliasing : (int)antiAliasing;
        }
    }
	
    void OnOverlayDepthChanged(SvrOverrideSettings.eDepth depth)
    {
        foreach (SvrOverlay overlay in overlays)
        {
            overlay.Depth = depth == SvrOverrideSettings.eDepth.NoOverride ?
                (int)settings.overlayDepth : (int)depth;
        }
    }
	
    void OnOverlayResolutionScaleFactorChanged(float scaleFactor)
    {
        foreach (SvrOverlay overlay in overlays)
        {
            overlay.ResolutionScaleFactor = scaleFactor <= 0 ? settings.overlayResolutionScaleFactor : scaleFactor;
        }
    }
	
    void OnChromaticAberrationCorrectionChanged(SvrOverrideSettings.eChromaticAberrationCorrection aberrationCorrection)
    {
        if(aberrationCorrection == SvrOverrideSettings.eChromaticAberrationCorrection.kDisable)
        {
            plugin.SetFrameOption(SvrPlugin.FrameOption.kDisableChromaticCorrection);
        }
        else
        {
            plugin.UnsetFrameOption(SvrPlugin.FrameOption.kDisableChromaticCorrection);
        }
    }
	
    void OnVSyncCountChanged(SvrOverrideSettings.eVSyncCount vSyncCount)
    {
        if (vSyncCount == SvrOverrideSettings.eVSyncCount.NoOverride)
        {
            plugin.SetVSyncCount((int)settings.vSyncCount);
            QualitySettings.vSyncCount = (int)settings.vSyncCount;
        }
        else
        {
            plugin.SetVSyncCount((int)vSyncCount);
            QualitySettings.vSyncCount = (int)settings.vSyncCount;
        }
    }
	
    void OnMasterTextureLimitChanged(SvrOverrideSettings.eMasterTextureLimit masterTextureLimit)
    {
        QualitySettings.masterTextureLimit = masterTextureLimit == SvrOverrideSettings.eMasterTextureLimit.NoOverride ? 
            (int)settings.masterTextureLimit : (int)masterTextureLimit;
    }
	
    void OnPerfLevelChanged(SvrOverrideSettings.ePerfLevel cpuPerfLevel, SvrOverrideSettings.ePerfLevel gpuPerfLevel)
    {
        int currentCpuPerfLevel = cpuPerfLevel == SvrOverrideSettings.ePerfLevel.NoOverride ? 
            (int)settings.cpuPerfLevel : (int)SvrOverrideSettings.CpuPerfLevel;
        int currentGpuPerfLevel = gpuPerfLevel == SvrOverrideSettings.ePerfLevel.NoOverride ?
            (int)settings.gpuPerfLevel : (int)SvrOverrideSettings.GpuPerfLevel;
        plugin.SetPerformanceLevels(currentCpuPerfLevel, currentGpuPerfLevel);
    }

    void OnFoveateChanged(Vector2 gain, float area, float minPixelDensity)
    {
        foveation = GetFoveationValues();

        var point = Vector2.zero;
        for (int i = 0; i < renderCount; i++)
        {
            plugin.SetFoveationParameters(i, 0, 0, point.x, point.y, foveation.Gain.x, foveation.Gain.y, foveation.Area, foveation.Minimum);
        }

        Debug.LogFormat("Foveation: Area={0} Gain=({1}, {2}) Minimum={3}", foveation.Area, foveation.Gain.x, foveation.Gain.y, foveation.Minimum);
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    //---------------------------------------------------------------------------------------------
    void Update()
	{
		SvrEvent frameEvent = new SvrEvent ();

        while (plugin.PollEvent(ref frameEvent))
		{
            if(DEBUG_LOG) Debug.LogFormat("SvrEvent: {0}", frameEvent.eventType.ToString());
            for (int i = 0; i < eventListeners.Count; i++) {
				eventListeners [i].OnSvrEvent (frameEvent);
			}
		}
	}

	/// <summary>
	/// Adds the event listener.
	/// </summary>
	/// <param name="listener">Listener.</param>
	//---------------------------------------------------------------------------------------------
	public void AddEventListener(SvrEventListener listener)
	{
		if (listener != null) {
			eventListeners.Insert (0, listener);
		}
	}

    /// <summary>
    /// Generate an event.
    /// </summary>
    /// <param name="frameEvent">FrameEvent.</param>
    //---------------------------------------------------------------------------------------------
    public void SubmitEvent(SvrEvent frameEvent)
    {
        if(DEBUG_LOG) Debug.LogFormat("SubmitEvent: {0}", frameEvent.eventType.ToString());
        for (int i = 0; i < eventListeners.Count; i++)
        {
            eventListeners[i].OnSvrEvent(frameEvent);
        }
    }

    /// <summary>
    /// Start Tracking
    /// </summary>
    /// <returns>Handle to the controller</returns>
    /// <param name="desc">Desc.</param>
    //---------------------------------------------------------------------------------------------
    public int ControllerStartTracking(string desc)
    {
        return plugin.ControllerStartTracking(desc);
    }
    
	/// <summary>
	/// Stop Tracking
	/// </summary>
	/// <param name="handle">Handle.</param>
	//---------------------------------------------------------------------------------------------
	public void ControllerStopTracking(int handle)
    {
        plugin.ControllerStopTracking(handle);
    }
    
	/// <summary>
	/// Get current state
	/// </summary>
	/// <returns>Controller State.</returns>
	/// <param name="handle">Handle.</param>
	//---------------------------------------------------------------------------------------------
	public SvrControllerState ControllerGetState(int handle, int space = 0)
    {
		return plugin.ControllerGetState(handle, space);
    }

	/// <summary>
	/// Send an event to the controller
	/// </summary>
	/// <param name="handle">Handle.</param>
	/// <param name="what">What.</param>
	/// <param name="arg1">Arg1.</param>
	/// <param name="arg2">Arg2.</param>
	//---------------------------------------------------------------------------------------------
	public void ControllerSendMessage(int handle, SvrController.svrControllerMessageType what, int arg1, int arg2)
	{
		plugin.ControllerSendMessage (handle, what, arg1, arg2);
	}

	/// <summary>
	/// Controllers the query.
	/// </summary>
	/// <returns>The query.</returns>
	/// <param name="handle">Handle.</param>
	/// <param name="what">What.</param>
	/// <param name="mem">Mem.</param>
	/// <param name="size">Size.</param>
	//---------------------------------------------------------------------------------------------
	public object ControllerQuery(int handle, SvrController.svrControllerQueryType what)
	{
		return plugin.ControllerQuery (handle, what);
	}

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect _position, UnityEditor.SerializedProperty _property, GUIContent _label)
        {
            _property.intValue = UnityEditor.EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
        }
    }
#endif
}