using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Unity.VectorGraphics;

public class AppButtonManager : MonoBehaviour
{

    public GameObject appButtonPrefab; //instance target Prefab

    public List<GameObject> iconList = new List<GameObject>();

    private int iconIndex;      //List size

    struct AppPref
    {
        public string _packageName;
        public string _labelName;
        public Sprite _appIcon;
        public int _index;
    }

    List<AppPref> _appPrefs = new List<AppPref>();
    // Start is called before the first frame update
    void Start()
    {
        InitAppList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitAppList()
    {
        AndroidJavaClass _jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject _currentActivity = _jc.GetStatic<AndroidJavaObject>("currentActivity");
        //int flag = new AndroidJavaClass("android.content.pm.PackageManager").GetStatic<int>("GET_META_DATA");
        AndroidJavaObject _pm = _currentActivity.Call<AndroidJavaObject>("getPackageManager");
        AndroidJavaObject _packages = _pm.Call<AndroidJavaObject>("getInstalledApplications", 0);
        int count = _packages.Call<int>("size");
        string[] names = new string[count];
        string[] packagesname = new string[count];
        for (int i=0; i<count; i++)
        {
            AndroidJavaObject currentObject = _packages.Call<AndroidJavaObject>("get", i);
            try
            {
                names[i] = _pm.Call<string>("getApplicationLabel", currentObject);
                var plugin = new AndroidJavaClass("kr.co.pncsolution.packagelib.CustomPackageManager");
                packagesname[i] = plugin.CallStatic<string>("getPackageName", currentObject);

                // if ((plugin.CallStatic<bool>("isSystem", currentObject) && packagesname[i].Contains("kr.co.pncsolution.ARLauncher") == true) ||
                //     packagesname[i].Contains("com.qualcomm") == true ||                  //simon 220922 launcher 화면에 표시 제외 앱들
                //     packagesname[i].Contains("kr.co.pncsolution.ARLauncher") == true )
                // {
                //     ii++;
                //     continue;
                // }

                //simon 230310 SDK로 개발된 앱(manifest에 "addlist" 항목)이거나, pncsolution의 앱만 표시함.
                int addList = _currentActivity.Call<int>("getAddList", packagesname[i]);
                if(addList > 0)
                {
                    Debug.Log($"AppButtonManager InitAppList:: addList:{addList} packagesname:{packagesname[i]}");
                }
                else if((packagesname[i].Contains("kr.co.pncsolution") == false &&
                    packagesname[i].Contains("com.pncsolution") == false))
                {
                    continue;
                }

                // if((packagesname[i].Contains("co.") == false &&
                //     packagesname[i].Contains("com.") == false) ||
                //     plugin.CallStatic<bool>("isSystem", currentObject) ||
                //     packagesname[i].Contains("com.qualcomm") == true)                                  //시스템 앱 등을 걸러내기 위함
                //     continue;

                if(packagesname[i].Contains("kr.co.pncsolution.arlauncher") == true || 
                    packagesname[i].Contains("kr.co.pncsolution.ARLauncher") == true || 
                    packagesname[i].Contains("kr.co.pncsolution.service") == true)          //simon 230220 런처와 서비스는 추가 대상이 아님
                {
                    continue;
                }
                
                Debug.Log("AppButtonManager InitAppList:: [" + i.ToString() + "] " + names[i]);

                byte[] decodedBytes = plugin.CallStatic<byte[]>("getIcon", _pm, currentObject);
                Texture2D text = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                text.LoadImage(decodedBytes);
                Sprite sprite = Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(.5f, .5f));

                AppPref appPref = new AppPref();
                appPref._packageName = packagesname[i];

                string appName = names[i];
                int len = appName.Length;
                if(len > 10)
                {
                    appName = appName.Substring(0, 10);
                    appName = appName + "..";
                }
                appPref._labelName = appName;
                appPref._appIcon = sprite;
                appPref._index = i;

                _appPrefs.Add(appPref);

                //image.sprite = sprite;
            }
            catch (Exception e)
            {
                Debug.Log("########## :: ERROR!!" + i.ToString());

                Debug.LogError(e, this);
            }
        }

        Debug.Log($"AppButtonManager InitAppList count:{_appPrefs.Count}");
        for (int j = 0; j<_appPrefs.Count; j++)
        {
            //Debug.Log("########## :: List[" + j.ToString() + "]" + _appPrefs[j]._labelName.ToString());
            //Debug.Log("########## :: List[" + j.ToString() + "]" + _appPrefs[j]._packageName.ToString());
            //Debug.Log("########## :: List[" + j.ToString() + "]" + _appPrefs[j]._index.ToString());

            AppIcon_AutoGenerate(_currentActivity, _pm, _appPrefs[j], j);
        }
    }

    void AppIcon_AutoGenerate(AndroidJavaObject currentActivity, AndroidJavaObject pm, AppPref apps, int index)
    {
        string objName = "App_" + index.ToString();
        appButtonPrefab.name = objName;

        GameObject canvas = appButtonPrefab.transform.Find("Canvas").gameObject;
        GameObject goBtn = canvas.transform.Find("Button").gameObject;
        if(goBtn == null)
        {
            Debug.Log($"AppButtonManager AppIcon_AutoGenerate goBtn is null");
            return;
        }

        Pnc.UI.Button.MetaCoreButton2D mb = goBtn.GetComponent<Pnc.UI.Button.MetaCoreButton2D>();
        if(mb == null)
        {
            Debug.Log($"AppButtonManager AppIcon_AutoGenerate mb is null");
            return;
        }
        mb.packageName = apps._packageName;

        //Button btn = goBtn.GetComponent<Button>();
        //btn.onClick.AddListener(delegate { OpenAppwithPackageName(currentActivity, pm, apps._packageName); });

        goBtn.GetComponent<Image>().sprite = apps._appIcon;
        goBtn.transform.Find("Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = apps._labelName;
        // Transform trLabel = goBtn.transform.Find("Label");
        // if(trLabel == null)
        // {
        //     Debug.Log($"AppButtonManager AppIcon_AutoGenerate trLabel is null");
            
        //     GameObject goLabel = goBtn.transform.Find("Label").gameObject;
        //     goLabel.GetComponent<TMPro.TextMeshProUGUI>().text = apps._labelName;
        // }
        // else
        // {
        //     Debug.Log($"AppButtonManager AppIcon_AutoGenerate trLabel is not null");
        //     GameObject goLabel = goBtn.transform.Find("Label").gameObject;
        //     Debug.Log($"AppButtonManager AppIcon_AutoGenerate trLabel test 111");
        //     goLabel.GetComponent<TMPro.TextMeshProUGUI>().text = apps._labelName;

        //     Debug.Log($"AppButtonManager AppIcon_AutoGenerate trLabel test 222");
        //     //goBtn.transform.Find("Label").GetComponent<TMPro.TextMeshProUGUI>().text = apps._labelName;
        // }

        Debug.Log($"AppButtonManager AppIcon_AutoGenerate index:{index}, name:{objName}, label:{apps._labelName}");

        iconList.Add(Instantiate(appButtonPrefab, transform).GetComponent<GameObject>());
        //Debug.Log($"AppButtonManager AppIcon_AutoGenerate index:{index} end");
    }

    void CheckedCurrentIndex()
    {
        for (int i = 0; i < iconList.Count; i++)
        {
            if (iconList[i] != null)
                iconIndex++;
        }
    }

    void OpenAppwithPackageName(AndroidJavaObject currentActivity, AndroidJavaObject pm, string packagename)
    {

        //Debug.Log("########## SKIP111___onclicked!!!");
//        AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", packagename);
//        currentActivity.Call("startActivity", intent);
        AndroidJavaObject param1 = new AndroidJavaObject("java.lang.String", packagename);
        AndroidJavaObject param2 = new AndroidJavaObject("java.lang.String", "");

        Debug.Log($":::::     goApp, package={packagename}, param1={param1}, param2={param2}     :::::");
        currentActivity.CallStatic("goApp", currentActivity, param1, param2);
        //Application.Quit();
    }
}
