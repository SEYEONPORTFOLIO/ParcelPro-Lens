using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pnc.UI.Button
{
    public class HighlightHandler : MonoBehaviour
    {
        public GameObject goButtonRoot;
        public bool is2D = false;
        MetaCoreButton mb = null;
        MetaCoreButton2D mb2D = null;
        // Start is called before the first frame update
        void Start()
        {
            if(goButtonRoot == null)
            {
                GameObject goFront = gameObject.transform.parent.gameObject;
                goButtonRoot = goFront.transform.parent.gameObject;
            }

            if(is2D)
                mb2D = goButtonRoot.GetComponent<MetaCoreButton2D>();
            else
                mb = goButtonRoot.GetComponent<MetaCoreButton>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("FingerTip") || other.CompareTag("HitRay"))
            {
                Debug.Log($"HighlightHandler OnTriggerEnter TouchBegin Icon obj:{this.transform.name}, other:{other.transform.name}");
                if(is2D)
                    mb2D.isTouched = true;
                else
                    mb.isTouched = true;
                Debug.Log($"HighlightHandler OnTriggerEnter TouchBegin Touched true");
            }
        }

        void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("FingerTip") || other.CompareTag("HitRay"))
            {
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("FingerTip") || other.CompareTag("HitRay"))
            {
                Debug.Log($"HighlightHandler OnTriggerExit TouchEnd FingerTip");
                // if(is2D)
                //     mb2D.isTouched = false;
                // else
                //     mb.isTouched = false;
                //Debug.Log($"HighlightHandler OnTriggerExit TouchEnd:{mb.isTouched}");
            }
        }

    }
}
