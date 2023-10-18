using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using System;
namespace Pnc.UI.Slider
{
    public class MetaCoreSlider : MonoBehaviour
    {
        public bool enable = false;
        public float sliderVal = 0.0f;
        public GameObject validArea = null;
        public GameObject leftEnd = null;
        public GameObject rightEnd = null;
        public float length = 0.0f;
        private DateTime lastSoundTime;
        public int soundIntervalMs = 200;       //sound 자주 실행되지 않도록

        public UnityEvent SetValueFinished = new UnityEvent();

        public UnityEngine.UI.Slider slider = null;
        // Start is called before the first frame update
        void Start()
        {
            if(slider == null)
            {
                slider = GetComponent<UnityEngine.UI.Slider>();
            }
            if(leftEnd == null)
            {
                leftEnd = GameObject.Find("LeftEnd");
            }
            if(rightEnd == null)
            {
                rightEnd = GameObject.Find("LeftEnd");
            }
            if(validArea == null)
            {
                validArea = this.gameObject.transform.Find("Valid Area").gameObject;
            }

            length = rightEnd.transform.position.x - leftEnd.transform.position.x;
            lastSoundTime = DateTime.Now;

            //Debug.Log($"MetaCoreSlider Start validArea length:{length}");
            //Debug.Log($"MetaCoreSlider Start validArea position x:{validArea.transform.position.x}, y: {validArea.transform.position.y}, z:{validArea.transform.position.z}");
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnValueChanged()
        {
            sliderVal = slider.value;
            Debug.Log($"MetaCoreSlider OnValueChanged sliderVal:{sliderVal}");
        }
        
        public void SetAudioSource(string audioName)
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
    }
}
