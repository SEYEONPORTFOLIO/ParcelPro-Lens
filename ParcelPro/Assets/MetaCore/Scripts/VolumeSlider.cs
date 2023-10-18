using UnityEngine;
using Unity.VectorGraphics;

using TMPro;
namespace Pnc.UI.Slider
{
    public class VolumeSlider : MonoBehaviour
    {
        public GameObject goTitle = null;
        public GameObject UIButtonSquareIcon = null;


        private static AndroidJavaObject _mainActivity = null;
        private static AndroidJavaObject _androidAudioManager = null;
        private static AndroidJavaObject audioManager = null;
        private static int _maxVolume = 0;

        SVGImage img = null;
        int nPreVal = 0;

        MetaCoreSlider mSlider;

        // Start is called before the first frame update
        void Start()
        {
            if(mSlider == null)
            {
                mSlider = gameObject.GetComponent<MetaCoreSlider>();
            }
            
            int volume = GetVolume();               //초기값
            float tmp = ((float)volume*100.0f / (float)_maxVolume);

            mSlider.slider.onValueChanged.Invoke(tmp);
        }

        void Awake()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void ChangeVolume()
        {
            bool enable = mSlider.enable;

            // if(!enable)                 //enable은 슬라이더를 움직이는 중이므로 놓고 나면 값을 세팅한다.
            // {
            //     Debug.Log($"VolumeSlider ChangeVolume enable:{enable}, volume:{mSlider.sliderVal}");
            //     SetVolume(mSlider.sliderVal);
            // }
            
            if(goTitle)
            {
                int nCurVal = (int)mSlider.sliderVal;
                if(nCurVal != nPreVal)
                {
                    Debug.Log($"VolumeSlider ChangeVolume nCurVal:{nCurVal}, nPreVal:{nPreVal}");
                    //볼륨 표시
                    string title = "VOLUME SETTING  |  " + nCurVal + "%";
                    
                    TextMeshProUGUI text = goTitle.GetComponent<TextMeshProUGUI>();
                    text.text = title;
                }
            }

            nPreVal = (int)mSlider.sliderVal;
        }
        
        private static AndroidJavaObject GetMainActivity()
        {
            if (_mainActivity == null)
            {
                var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _mainActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            }

            return (_mainActivity);
        }

        private static AndroidJavaObject GetAndroidAudioManager()
        {
            if (_androidAudioManager == null)
            {
                _androidAudioManager = GetMainActivity().Call<AndroidJavaObject>("getSystemService", "audio");
                _maxVolume = _androidAudioManager.Call<int>("getStreamMaxVolume", 3);
            }
            return (_androidAudioManager);
        }

        private int GetVolume()
        {
            audioManager = GetAndroidAudioManager();
            return audioManager.Call<int>("getStreamVolume", 3);
        }
        
        // 볼륨 설정, 인자는 0부터 1 사이 값, _maxVolume과 비율로 설정해야 할 것으로 보임
        private void SetVolume(float fVolume)
        {
            float tmp = ((float)fVolume*(float)_maxVolume / 100.0f);
            Debug.Log($"VolumeSlider SetVolume tmp:{tmp}, fVolume:{fVolume}, _maxVolume:{_maxVolume}");

            //audioManager = GetAndroidAudioManager();
            audioManager.Call("setStreamVolume", 3, (int)tmp, 0);
        }

        public void OnSetValueFinished()
        {
            int nVal = (int)mSlider.sliderVal;
            Debug.Log($"VolumeSlider OnSetValueFinished nCurVal:{nVal}");
            
            SetVolume(mSlider.sliderVal);

            if(!UIButtonSquareIcon)
            {
                GameObject goButton = GameObject.Find("VolumeButton");
                GameObject iconAndText = goButton.transform.Find("IconAndText").gameObject;
                UIButtonSquareIcon = iconAndText.transform.Find("UIButtonSquareIcon").gameObject;
            }
            
            if(!img)
            {
                img = UIButtonSquareIcon.GetComponent<SVGImage>();
            }

            string strSprite = "";

            if(nVal > 66)
            {
                strSprite = "Sprite/Launcher/08. volume2";
            }
            else if(nVal > 33)
            {
                strSprite = "Sprite/Launcher/08. volume3";
            }
            else if(nVal > 10)
            {
                strSprite = "Sprite/Launcher/08. volume4";
            }
            else if(nVal > 1)
            {
                strSprite = "Sprite/Launcher/08. volume5";
            }
            else
            {
                strSprite = "Sprite/Launcher/08. volume1";
            }

            Debug.Log($"VolumeSlider OnSetValueFinished nVal={nVal}, strSprite={strSprite}");
            Sprite sprite = Resources.Load<Sprite>(strSprite) as Sprite;
            img.sprite = sprite;
        }
    }
}
