using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;

namespace Pnc.UI.Button
{
    public class ScrollEvent : MonoBehaviour
    {
        public bool isUp = false;
        bool isPressed = false;
        public float scrollMove = 20.0f;

        public GameObject UIButtonSquareIcon;
        SVGImage img = null;

        // Start is called before the first frame update
        void Start()
        {
            if(UIButtonSquareIcon == null)
            {
                GameObject iconAndText = this.gameObject.transform.Find("IconAndText").gameObject;
                UIButtonSquareIcon = iconAndText.transform.Find("UIButtonSquareIcon").gameObject;
            }
            
            img = UIButtonSquareIcon.GetComponent<SVGImage>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
        public void SetScroll()
        {
            ScrollRect scrollRect = GameObject.Find("AppList_Scroll View").GetComponent<ScrollRect>();

            RectTransform scroll = scrollRect.content.GetComponent<RectTransform>();
            Debug.Log($"ScrollEvent SetScroll isUp:{isUp} y:{scroll.anchoredPosition.y}");

            if(!isPressed)                  //simon 230227 [scrollMove] 만큼 스크롤하고, 화살표 아이콘을 2.0f 만큼 움직인다.
            {
                string strSprite = "Sprite/Launcher/위쪽 스크롤 화살표_눌렀을때";

                if(isUp)
                {
                    scrollRect.content.anchoredPosition = new Vector2(scroll.anchoredPosition.x, scroll.anchoredPosition.y - scrollMove);
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y+2.0f, this.transform.localPosition.z);
                }
                else
                {
                    scrollRect.content.anchoredPosition = new Vector2(scroll.anchoredPosition.x, scroll.anchoredPosition.y + scrollMove);
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y-2.0f, this.transform.localPosition.z);
                
                    strSprite = "Sprite/Launcher/아래쪽 스크롤 화살표_눌렀을때";
                }
                Sprite pressed = Resources.Load<Sprite>(strSprite) as Sprite;
                img.sprite = pressed;

                isPressed = true;
            }
        }
        
        public void OnRelease()
        {
            Debug.Log($"ScrollEvent OnRelease isUp:{isUp} local x:{this.transform.localPosition.x}, y:{this.transform.localPosition.y}, y:{this.transform.localPosition.z}");

            if(isPressed)
            {
                string strSprite = "Sprite/Launcher/위쪽 스크롤 화살표";

                if(isUp)
                {
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y-2.0f, this.transform.localPosition.z);
                }
                else
                {
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y+2.0f, this.transform.localPosition.z);
                    strSprite = "Sprite/Launcher/아래쪽 스크롤 화살표";
                }
                Sprite released = Resources.Load<Sprite>(strSprite) as Sprite;
                img.sprite = released;

                isPressed = false;
            }
        }
    }
}
