using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Unity.VectorGraphics;
using TMPro;

using UnityEngine.Android;

// �� ��ó ���� �� ��Ʈ�� ���� ��� ������
public class IntroManager : MonoBehaviour
{
    public GameObject videoRenderer;
    public GameObject mainViewport;
    public VideoPlayer v_player;
    public Canvas mainCanvas;
    public RectTransform mainViewRectTr;
    public AudioSource sfxPlayer;

    public Transform initialTr;

    public static bool isPlayingVideo = true;

    private static AndroidJavaObject _mainActivity = null;
    private static AndroidJavaObject _androidAudioManager = null;

    private static AndroidJavaObject _aidlPlugin = null;
    
    [SerializeField] private GameObject goLightTxt = null;
    [SerializeField] private GameObject goWifi = null;
    [SerializeField] private GameObject goWifiTxt = null;
    [SerializeField] private GameObject goBattery = null;
    [SerializeField] private GameObject goBatteryRemain = null;
    [SerializeField] private GameObject goVolume = null;
    [SerializeField] private GameObject goVolTxt = null;

    private static int _maxVolume = 0;

    private static int wifiSignalLevel = -2;
    private static int batteryLevel = -2;
    private static int volumeLevel = -2;

    private Sprite spriteWifi;
    private Sprite spriteBattery;
    private Sprite spriteVolume;
    private int intervalWifi = 120;
    private int sleepCntWifi = 0;
    private int intervalBattery = 180;
    private int sleepCntBattery = 0;
    private int intervalVol = 30;
    private int sleepCntVol = 0;
    private int intervalBrightness = 30;
    private int sleepCntBrightness = 0;

    void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        PlayVideo();
    }
    
    void Init()
    {        
        mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        videoRenderer = GameObject.FindGameObjectWithTag("MainCanvas").transform.GetChild(0).gameObject;
        mainViewport = GameObject.FindGameObjectWithTag("MainCanvas").transform.GetChild(1).gameObject;
        mainViewRectTr = GameObject.FindGameObjectWithTag("MainCanvas").transform.GetChild(1).GetComponent<RectTransform>();
        initialTr = GameObject.FindGameObjectWithTag("MainCam").GetComponent<Transform>();
        
        sfxPlayer = GetComponent<AudioSource>();
        sfxPlayer.playOnAwake = false;
        sfxPlayer.loop = false;
        sfxPlayer.time = 0.0f;
        v_player = GetComponent<VideoPlayer>();
        v_player.isLooping = false;
        v_player.time = 0.0f;
        //mainCanvas.renderMode = RenderMode.WorldSpace;

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
    }
    bool isBooted = false;
    
    void PlayVideo()
    {
        AndroidJavaClass _jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject _currentActivity = _jc.GetStatic<AndroidJavaObject>("currentActivity");
        var plugin = new AndroidJavaClass("kr.co.pncsolution.packagelib.CustomPackageManager");
        isBooted = plugin.CallStatic<bool>("getBoolean", _currentActivity);

        if (isBooted)
        {
            mainViewport.SetActive(false);
            v_player.Play();
        }
        StartCoroutine(PlayOff());
    }

    IEnumerator CheckVideoPlaying()
    {
        yield return new WaitForSeconds(1f);

        while (isPlayingVideo)
        {
    
            if (v_player.isPlaying == false || isBooted == false)
                isPlayingVideo = false;

            yield return null;
        }
    }

    void Update(){
        SetTime();
        SetBattery();
        SetWifiSignalLevel();
        SetVolume();
        SetBrightness();
    }

    IEnumerator PlayOff()
    {
        yield return StartCoroutine(CheckVideoPlaying());

        // Render Texture������� ���� ��� �� ���
        videoRenderer.SetActive(false); 
        v_player.enabled = false;

        mainViewport.SetActive(true);
        sfxPlayer.Play();

        yield return UIAppear();
    }

    // UI����(ZoomIn����)
    IEnumerator UIAppear()
    {
        Vector3 targetPos = mainViewRectTr.anchoredPosition3D;
        //mainViewport.SetActive(true);
        sfxPlayer.Play();
        
        while (mainViewRectTr.anchoredPosition3D.z > 0f)
        {
            mainViewRectTr.anchoredPosition3D = targetPos;
            targetPos.z -= 1500 * Time.deltaTime;

            if (mainViewRectTr.anchoredPosition3D.z <= 0f)
            {
                mainViewRectTr.anchoredPosition3D = new Vector3(0, 0, 0f);
                yield break;
            }
            //Debug.Log($"@@@@@@ TrInfo MainView :: {mainViewRectTr.anchoredPosition3D}");

            yield return null;
        }

        sfxPlayer.Stop();
        sfxPlayer.time = 0.0f;
    }


    public static AndroidJavaObject GetMainActivity()
    {
        if (_mainActivity == null)
        {
            var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _mainActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        return (_mainActivity);
    }

    public static AndroidJavaObject GetAndroidAudioManager()
    {
        if (_androidAudioManager == null)
        {
            _androidAudioManager = GetMainActivity().Call<AndroidJavaObject>("getSystemService", "audio");
            _maxVolume = _androidAudioManager.Call<int>("getStreamMaxVolume", 3);
        }
        return (_androidAudioManager);
    }

    private void SetTime()
    {        
        GameObject time = GameObject.Find("time");
        if(time)
        {
            DateTime t = System.DateTime.Now;
            
            //Debug.Log($"IntroManager time hour:{t.Hour} Min:{t.Minute} sec:{t.Second}");

            TextMeshProUGUI text = time.GetComponent<TextMeshProUGUI>();
            text.text = t.ToString("h:mm");
        }
    }

    // 볼륨 확인
    public int GetVolume()
    {
        AndroidJavaObject audioManager = GetAndroidAudioManager();
        return audioManager.Call<int>("getStreamVolume", 3);
    }

    // 볼륨 설정, 인자는 0부터 1 사이 값, _maxVolume과 비율로 설정해야 할 것으로 보임
    // public void SetVolume(float fVolume_)
    // {
    //     AndroidJavaObject audioManager = GetAndroidAudioManager();
    //     audioManager.Call("setStreamVolume", 3, (int)fVolume_, 0);
    // }

    private void SetBrightness()
    {
        if(sleepCntBrightness> 0)                //프레임마다 계산할 필요가 없으므로 매번(Update마다) 실행하지 않는다.
        {
            //Debug.Log($"IntroManager SetBrightness sleepCntBrightness:{sleepCntBrightness}");
            sleepCntBrightness--;
            return;
        }
        else
        {
            sleepCntBrightness = intervalBrightness;
        }
        
        if (_aidlPlugin == null)
        {
            var plugin = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _aidlPlugin = plugin.GetStatic<AndroidJavaObject>("currentActivity");
        }

        // float fBrightness = _aidlPlugin.Call<float>("getBrightness");
        // if(fBrightness < 0.0f)
        // {
        //     Debug.Log($"IntroManager SetBrightness result:{fBrightness}");
        //     return;
        // }
        int nResult = _aidlPlugin.Call<int>("getBrightness", _aidlPlugin);
        if(nResult < 0)
        {
            Debug.Log($"IntroManager SetBrightness result:{nResult}");
            return;
        }

        int nBrightness = nResult;//(int)(nResult * 100.0f / 255.0f);

        //int nBrightness = (int)(fBrightness * 100.0f);
        Debug.Log($"IntroManager SetBrightness nBrightness:{nBrightness}");
        // int nResult = _aidlPlugin.Call<int>("changeScreenBrightness", 50);
        // Debug.Log($"IntroManager changeScreenBrightness result:{nResult}");
        
        if(goLightTxt == null)
            goLightTxt = GameObject.Find("light");
        
        TextMeshProUGUI textLight = goLightTxt.GetComponent<TextMeshProUGUI>();
        textLight.text = nBrightness.ToString() + "%";
    }

    private void SetVolume()
    {
        if(sleepCntVol > 0)                //프레임마다 계산할 필요가 없으므로 이 매번(Update마다) 실행하지 않는다.
        {
            //Debug.Log($"IntroManager Update sleepCnt:{sleepCnt}");
            sleepCntVol--;
            return;
        }
        else
        {
            sleepCntVol = intervalVol;
        }

        int volume = GetVolume();
        float tmp = ((float)volume*100.0f / (float)_maxVolume);
        int nVol = (int)tmp;
        //Debug.Log($"IntroManager SetVolume volume={volume}, max={_maxVolume}, nVol={nVol}");

        if(goVolTxt == null)
            goVolTxt = GameObject.Find("volume");
        if(goVolTxt)
        {
            TextMeshProUGUI volText = goVolTxt.GetComponent<TextMeshProUGUI>();
            volText.text = nVol.ToString();
        }

        if(goVolume == null)
            goVolume = GameObject.Find("volumeIcon");
        if(goVolume)
        {
            SVGImage img = goVolume.GetComponent<SVGImage>();
            if(img)
            {
                int curLevel = -1;
                string strSprite = "";

                if(nVol > 66)
                {
                    curLevel = 1;
                    strSprite = "Sprite/Launcher/08. volume2";
                }
                else if(nVol > 33)
                {
                    curLevel = 2;
                    strSprite = "Sprite/Launcher/08. volume3";
                }
                else if(nVol > 10)
                {
                    curLevel = 3;
                    strSprite = "Sprite/Launcher/08. volume4";
                }
                else if(nVol > 1)
                {
                    curLevel = 4;
                    strSprite = "Sprite/Launcher/08. volume5";
                }
                else
                {
                    curLevel = 5;
                    strSprite = "Sprite/Launcher/08. volume1";
                }

                if(volumeLevel != curLevel)                //simon 221020 변화가 있을 때에만 이미지를 로드한다.
                {
                    Debug.Log($"IntroManager SetVolume volume={nVol}, curLevel={curLevel}, strSprite={strSprite}");
                    spriteVolume = Resources.Load<Sprite>(strSprite) as Sprite;
                    img.sprite = spriteVolume;

                    volumeLevel = curLevel;
                }
            }
        }
    }

    private void SetBattery()
    {
        if(sleepCntBattery > 0)                //프레임마다 계산할 필요가 없으므로 이 매번(Update마다) 실행하지 않는다.
        {
            //Debug.Log($"IntroManager Update sleepCnt:{sleepCnt}");
            sleepCntBattery--;
            return;
        }
        else
        {
            sleepCntBattery = intervalBattery;
        }

        float battery = SystemInfo.batteryLevel;
        BatteryStatus batteryStatus = SystemInfo.batteryStatus;
        //Debug.Log($"IntroManager Init battery={battery}, Status={batteryStatus}");

        if(goBattery == null)
            goBattery = GameObject.Find("battery");
        
        if(goBattery)
        {
            SVGImage img = goBattery.GetComponent<SVGImage>();
            
            if(img)
            {
                if(goBatteryRemain == null)
                    goBatteryRemain = GameObject.Find("battery remaining");
                int batteryRemain = (int)(battery*100.0f);

                TextMeshProUGUI textBattery = goBatteryRemain.GetComponent<TextMeshProUGUI>();
                textBattery.text = batteryRemain.ToString() + "%";

                int curLevel = -1;
                string strSprite = "";

                if(batteryRemain > 90)
                {
                    curLevel = 1;
                    strSprite = "Sprite/Launcher/03. battery1";
                }
                else if(batteryRemain > 60)
                {
                    curLevel = 2;
                    strSprite = "Sprite/Launcher/03. battery2";
                }
                else if(batteryRemain > 40)
                {
                    curLevel = 3;
                    strSprite = "Sprite/Launcher/03. battery3";
                }
                else if(batteryRemain > 20)
                {
                    curLevel = 4;
                    strSprite = "Sprite/Launcher/03. battery4";
                }
                else if(batteryRemain > 10)
                {
                    curLevel = 5;
                    strSprite = "Sprite/Launcher/03. battery5";
                }
                else
                {
                    curLevel = 6;
                    strSprite = "Sprite/Launcher/03. battery6";
                }

                if(batteryStatus.Equals(BatteryStatus.Charging) == true)                //충전 중 이미지 색 변경
                {
                    //Debug.Log($"IntroManager battery Charging");
                    img.color = new Color32(139,238,247,255);
                }
                else if(curLevel == 6)
                {
                    img.color = Color.red;
                }
                else
                {
                    img.color = Color.white;
                }

                if(batteryLevel != curLevel)                //simon 221020 배터리 잔량 변화가 있을 때에만 이미지를 로드한다.
                {
                    spriteBattery = Resources.Load<Sprite>(strSprite) as Sprite;
                    img.sprite = spriteBattery;

                    batteryLevel = curLevel;
                }
            }                    
        }
    }

    // Wifi 신호세기 확인
    public int GetWifi()
    {
        if (_aidlPlugin == null)
        {
            var plugin = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        
            _aidlPlugin = plugin.GetStatic<AndroidJavaObject>("currentActivity");
        }

        int nResult = _aidlPlugin.Call<int>("onWifiCheck", _aidlPlugin);
        Debug.Log($"IntroManager GetWifi onWifiCheck result:{nResult}");

        if(nResult >= 0)
        {
            string SSID = _aidlPlugin.Call<string>("getWifiSSID", _aidlPlugin);
            
            if(goWifiTxt == null)
                goWifiTxt = GameObject.Find("wifi");
            TextMeshProUGUI textWifi = goWifiTxt.GetComponent<TextMeshProUGUI>();
            if(wifiSignalLevel > 0)
            {
                textWifi.text = SSID;
            }
            else
            {
                textWifi.text = "Not Connected";
            }
        }
        
        return nResult;
    }

    private void SetWifiSignalLevel()
    {
        if(sleepCntWifi > 0)                //프레임마다 계산할 필요가 없으므로 이 매번(Update마다) 실행하지 않는다.
        {
            //Debug.Log($"IntroManager SetWifiSignalLevel sleepCnt:{sleepCntWifi}");
            sleepCntWifi--;
            return;
        }
        else
        {
            sleepCntWifi = intervalWifi;
        }

        int curLevel = GetWifi();
        if(wifiSignalLevel == curLevel)
        {
            return;
        }

        wifiSignalLevel = curLevel;

        // if(goWifiTxt == null)
        //     goWifiTxt = GameObject.Find("wifi connected");
        // TextMeshProUGUI textWifi = goWifiTxt.GetComponent<TextMeshProUGUI>();
        // if(wifiSignalLevel > 0)
        // {
        //     textWifi.text = "Connected";
        // }
        // else
        // {
        //     textWifi.text = "Not Connected";
        // }

        Debug.Log($"IntroManager SetWifiSignalLevel level:{curLevel}");

        if(goWifi == null)
            goWifi = GameObject.Find("wifiIcon");

        if(!goWifi)
        {
            Debug.Log($"IntroManager SetWifiSignalLevel goWifi is null");
            return;
        }

        SVGImage img = goWifi.GetComponent<SVGImage>();
        
        if(img)
        {
            switch(wifiSignalLevel)
            {
                case 0:
                    spriteWifi = Resources.Load<Sprite>("Sprite/wifi 시안 1 (5)") as Sprite;
                    break;
                
                case 1:
                    spriteWifi = Resources.Load<Sprite>("Sprite/wifi 시안 1 (4)") as Sprite;
                    break;
                
                case 2:
                    spriteWifi = Resources.Load<Sprite>("Sprite/wifi 시안 1 (3)") as Sprite;
                    break;
                
                case 3:
                    spriteWifi = Resources.Load<Sprite>("Sprite/wifi 시안 1 (2)") as Sprite;
                    break;
                
                case 4:
                    spriteWifi = Resources.Load<Sprite>("Sprite/wifi 시안 1 (1)") as Sprite;
                    break;

                default:
                    spriteWifi = Resources.Load<Sprite>("Sprite/wifi 시안 1 (5)") as Sprite;
                    break;
            }
            
            Debug.Log($"IntroManager sprite name:{spriteWifi.name}");
            img.sprite = spriteWifi;
        }
    }
}
