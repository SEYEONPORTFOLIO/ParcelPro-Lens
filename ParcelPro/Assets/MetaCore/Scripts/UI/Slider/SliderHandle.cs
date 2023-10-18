using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.VectorGraphics;
using Pnc.Model.Hands;

namespace Pnc.UI.Slider
{
    public class SliderHandle : MonoBehaviour
    {
        private util.ProcessGesture processGesture;
        int preGesture = (int)util.GestureType.UNKNOWN;

        MetaCoreSlider mSlider;
        SVGImage img;
        Camera mainCamera;

        // Start is called before the first frame update
        void Start()
        {
            if(mSlider == null)
            {
                GameObject sliderRoot = this.gameObject.transform.parent.gameObject.transform.parent.gameObject;

                mSlider = sliderRoot.GetComponent<MetaCoreSlider>();

            }

            mainCamera = GameObject.FindGameObjectWithTag("MainCam").GetComponent<Camera>();
            if (mainCamera == null)
            {
                mainCamera = GameObject.Find("Eye Left")?.GetComponent<Camera>();
            }
            SVGImage img = GetComponent<SVGImage>();
        }

        // Update is called once per frame
        void Update()
        {
            if(processGesture == null)
            {
                //Debug.Log($"SliderHandle Update processGesture is null");
                processGesture = util.ProcessGesture.Instance();
            }
            
            Hand rightHand = processGesture.GetHand(false);
            if(rightHand == null)
            {
                //Debug.Log($"PinControl Update rightHand is null");
                return;
            }
            
            if(mSlider.enable)
            {
                if(preGesture == (int)util.GestureType.PICK &&
                    rightHand.gesture != (int)util.GestureType.PICK)
                {
                    Debug.Log($"SliderHandle Update mSlider.enable FALSE!!");
                    OnEndSlider();
                    // mSlider.enable = false;
                    // mSlider.SetAudioSource("set");

                    // //핸들 이미지 변경
                    // if(!img)
                    //     img = GetComponent<SVGImage>();

                    // string strSprite = "Sprite/Slider/sliderbtn";
                    // Sprite sprite = Resources.Load<Sprite>(strSprite) as Sprite;
                    // img.sprite = sprite;
                }

                //손가락 위치로 슬라이더 값 계산
                List<Landmark> landmarks = rightHand.getLandmarks();
                Vector3 v8 = new Vector3(landmarks[8].x, landmarks[8].y, landmarks[8].z);
                
                float fVal = CalculateValue(v8);
                if(fVal >= 0.0f)
                {
                    Debug.Log($"SliderHandle OnTriggerStay fVal:{fVal}");

                    mSlider.slider.value = fVal;                            //손으로 슬라이더를 움직였으므로 값을 주고 Invoke
                    mSlider.slider.onValueChanged.Invoke(fVal);
                }
            }

            preGesture = rightHand.gesture;
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("FingerTip"))
            {
            }
        }

        void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("FingerTip"))
            {
                if(mSlider.enable)          //이미 핸들을 움직이는 중
                {
                    return;
                }

                if(processGesture == null)
                {
                    //Debug.Log($"SliderHandle Update processGesture is null");
                    processGesture = util.ProcessGesture.Instance();
                }
                
                Hand rightHand = processGesture.GetHand(false);
                if(rightHand == null)
                {
                    //Debug.Log($"PinControl Update rightHand is null");
                    return;
                }
                
                if(rightHand.gesture == (int)util.GestureType.PICK &&
                    preGesture != (int)util.GestureType.PICK)
                {
                    Debug.Log($"SliderHandle OnTriggerStay mSlider.enable TRUE!!");
                    OnStartSlider();
                    // mSlider.enable = true;

                    // //핸들 이미지 변경
                    // if(!img)
                    //     img = GetComponent<SVGImage>();

                    // string strSprite = "Sprite/Slider/controller_grab";
                    // Sprite sprite = Resources.Load<Sprite>(strSprite) as Sprite;
                    // img.sprite = sprite;

                    // //audio 효과
                    // mSlider.SetAudioSource("pick");
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("FingerTip"))
            {
                Debug.Log($"SliderHandle OnTriggerExit mSlider.enable FALSE!!");

                if(mSlider.enable)
                {
                    Debug.Log($"SliderHandle OnTriggerExit mSlider.slider.value:{mSlider.slider.value}");//손이 핸들을 벗어나면 현재의 값으로 invoke
                    OnEndSlider();
                    // mSlider.SetAudioSource("set");

                    // //핸들 이미지 변경
                    // //핸들 이미지 변경
                    // if(!img)
                    //     img = GetComponent<SVGImage>();

                    // string strSprite = "Sprite/Slider/sliderbtn";
                    // Sprite sprite = Resources.Load<Sprite>(strSprite) as Sprite;
                    // img.sprite = sprite;

                    // mSlider.enable = false;
                    mSlider.slider.onValueChanged.Invoke(mSlider.slider.value);
                }
            }
        }

        bool HandInValidArea(Vector3 vFingerTip)
        {
            bool isValid = false;

            if(vFingerTip.x >= mSlider.leftEnd.transform.position.x &&
                vFingerTip.x <= mSlider.rightEnd.transform.position.x)
            {
                isValid = true;
            }
            else
            {
                Debug.Log($"SliderHandle HandInValidArea vFingerTip.x:{vFingerTip.x}, min.x:{mSlider.leftEnd.transform.position.x}, max.x:{mSlider.rightEnd.transform.position.x}");
            }

            // Camera camera = GameObject.FindGameObjectWithTag("MainCam").GetComponent<Camera>();
            // if (camera == null)
            // {
            //     camera = GameObject.Find("Eye Left")?.GetComponent<Camera>();
            // }
            // Rect rect = mSlider.validArea.GetComponent<RectTransform>().rect;

            // Vector3 min = new Vector3(rect.xMin, rect.yMin, 1.0f);
            // Vector3 max = new Vector3(rect.xMax, rect.yMax, 1.0f);
            // //Vector3 max = RectTransformUtility.WorldToScreenPoint(null, new Vector3(rect.max.x, rect.max.y, 0.0f));

            // // Vector2 min = (Vector2)mSlider.validArea.transform.position - (mSlider.validArea.GetComponent<RectTransform>().pivot * rect.size);
            // // Vector2 max = min + rect.size;
            // isValid = vFingerTip.x >= min.x && vFingerTip.x <= max.x && vFingerTip.y >= min.y && vFingerTip.y <= max.y;

            // //if(isValid)
            // {
            //     Debug.Log($"SliderHandle HandInValidArea vFingerTip.x:{vFingerTip.x}, min.x:{min.x}, max.x:{max.x}");
            //     Debug.Log($"SliderHandle HandInValidArea vFingerTip.y:{vFingerTip.y}, min.y:{min.y}, max.y:{max.y}");

            // }

            return isValid;
        }

        float CalculateValue(Vector3 vFingerTip)
        {
            // if(!HandInValidArea(vFingerTip))
            // {
            //     Debug.Log($"SliderHandle CalculateValue HandInValidArea result false");
            //     return -1.0f;
            // }

            //스크린 좌표로 변환하여 현재 값 계산
            Vector3 vFinger = mainCamera.WorldToViewportPoint(vFingerTip);
            Vector3 vleftEnd = mainCamera.WorldToViewportPoint(mSlider.leftEnd.transform.position);
            Vector3 vRightEnd = mainCamera.WorldToViewportPoint(mSlider.rightEnd.transform.position);

            if(vFinger.x < vleftEnd.x ||
                vFinger.x > vRightEnd.x)
            {
                return -1.0f;
            }

            float value = ((vFinger.x - vleftEnd.x) / (vRightEnd.x - vleftEnd.x)) * mSlider.slider.maxValue;
            Debug.Log($"SliderHandle CalculateValue value:{value}, vFinger.x:{vFinger.x}, vleftEnd.x:{vleftEnd.x}, vRightEnd.x:{vRightEnd.x}");

            return value;
        }

        void OnEndSlider()
        {
            mSlider.enable = false;
            mSlider.SetAudioSource("set");

            //핸들 이미지 변경
            if(!img)
                img = GetComponent<SVGImage>();
            
            string strSprite = "Sprite/Slider/sliderbtn";
            Sprite sprite = Resources.Load<Sprite>(strSprite) as Sprite;
            img.sprite = sprite;

            mSlider.SetValueFinished.Invoke();
        }

        void OnStartSlider()
        {
            mSlider.enable = true;
            mSlider.SetAudioSource("pick");

            //핸들 이미지 변경
            if(!img)
                img = GetComponent<SVGImage>();

            string strSprite = "Sprite/Slider/controller_grab";
            Sprite sprite = Resources.Load<Sprite>(strSprite) as Sprite;
            img.sprite = sprite;

            //mSlider.slider.onValueChanged.Invoke(fVal);
        }
    }
}
