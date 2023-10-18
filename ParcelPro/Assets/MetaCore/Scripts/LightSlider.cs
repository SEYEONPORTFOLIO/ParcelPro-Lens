using UnityEngine;
using Unity.VectorGraphics;

using TMPro;
namespace Pnc.UI.Slider
{
    public class LightSlider : MonoBehaviour
    {
        public GameObject goTitle = null;
        public GameObject UIButtonSquareIcon = null;

        private static AndroidJavaObject _mainActivity = null;
        private static AndroidJavaObject _androidAudioManager = null;
        private static float _maxBrightness = 255.0f;

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
            
            int brightness = GetBrightness();               //초기값
            float tmp = ((float)brightness*100.0f / _maxBrightness);

            mSlider.slider.onValueChanged.Invoke(tmp);
        }

        void Awake()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void ChangeBrightness()
        {
            bool enable = mSlider.enable;

            // if(!enable)                 //enable은 슬라이더를 움직이는 중이므로 놓고 나면 값을 세팅한다.
            // {
            //     Debug.Log($"LightSlider ChangeBrightness enable:{enable}, val:{mSlider.sliderVal}");
            //     SetBrightness(mSlider.sliderVal);
            // }
            
            if(goTitle)
            {
                int nCurVal = (int)mSlider.sliderVal;
                if(nCurVal != nPreVal)
                {
                    Debug.Log($"LightSlider ChangeBrightness nCurVal:{nCurVal}, nPreVal:{nPreVal}");
                    //볼륨 표시
                    string title = "LIGHT SETTING  |  " + nCurVal + "%";
                    
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

        private int GetBrightness()
        {
            int nResult = GetMainActivity().Call<int>("getBrightness", _mainActivity);
            if(nResult < 0)
            {
                Debug.Log($"LightSlider GetBrightness result:{nResult}");
                return -1;
            }

            int nBrightness = (int)(nResult * 100.0f / _maxBrightness);
            Debug.Log($"LightSlider GetBrightness nBrightness:{nBrightness}, nResult:{nResult}");
            return nBrightness;
        }
        
        private void SetBrightness(float fBrightness)
        {            
            Debug.Log($"LightSlider SetBrightness nBrightness:{fBrightness}");
            int nResult = GetMainActivity().Call<int>("changeScreenBrightness", _mainActivity, (int)fBrightness);
            Debug.Log($"LightSlider SetBrightness changeScreenBrightness result:{nResult}");
        }
        public void OnSetValueFinished()
        {
            int nVal = (int)mSlider.sliderVal;
            Debug.Log($"LightSlider OnSetValueFinished nCurVal:{nVal}");
            
            SetBrightness(mSlider.sliderVal);

            if(!UIButtonSquareIcon)
            {
                GameObject goButton = GameObject.Find("LightButton");
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
                strSprite = "Sprite/Launcher/06. light";
            }
            else if(nVal > 33)
            {
                strSprite = "Sprite/Launcher/06. light2";
            }
            else if(nVal > 10)
            {
                strSprite = "Sprite/Launcher/06. light3";
            }
            else if(nVal > 1)
            {
                strSprite = "Sprite/Launcher/06. light4";
            }
            else
            {
                strSprite = "Sprite/Launcher/06. light5";
            }

            Debug.Log($"LightSlider OnSetValueFinished nVal={nVal}, strSprite={strSprite}");
            Sprite sprite = Resources.Load<Sprite>(strSprite) as Sprite;
            img.sprite = sprite;
        }
    }
}
