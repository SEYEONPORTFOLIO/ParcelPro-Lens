using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pnc.Model.Hands;

// 메뉴판 이동 스크립트
public class MoveMenu : MonoBehaviour
{
    public Camera player;
    public float timer = 0.3f;

    private util.ProcessGesture processGesture;

    int preGesture = -1;

    private void Start()
    {
      processGesture = util.ProcessGesture.Instance();
    }

    void LateUpdate()
    {
      if(processGesture == null)
      {
        return;
      }

      Hand leftHand = processGesture.GetHand();
      if(leftHand == null)
      {
          return;
      }

      int curGesture = leftHand.gesture;

      //simon 230112 이전 제스처와 다른 경우에만 동작하도록 한다.
      if (curGesture != preGesture && curGesture == 1)
      {
        List<Landmark> landmarks = leftHand.getLandmarks();
        
        Camera camera = GameObject.Find("Eye Left")?.GetComponent<Camera>();
        if (camera == null)
        {
            camera = GameObject.Find("left Camera")?.GetComponent<Camera>();
        }

        Vector3 vleftHand = new Vector3(landmarks[5].x, landmarks[5].y, landmarks[5].z);

        transform.position = vleftHand;
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward*3.0f);
      }

      preGesture = curGesture;
    }
}
