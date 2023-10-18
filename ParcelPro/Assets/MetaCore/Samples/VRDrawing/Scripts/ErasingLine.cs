using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pnc.Model;
using Pnc.Model.Hands;

public class ErasingLine : MonoBehaviour
{
    public GameObject point; // drawing point
    public float eraseDist = 0.005f; // 지우개 거리
    [SerializeField] private GameObject eraseLine; // 지울 선
    [SerializeField] private MovePointer mv; // drawing point 관련 스크립트(raycasting 진행 스크립트)

    private util.ProcessGesture processGesture;

    private void Start()
    {
        processGesture = util.ProcessGesture.Instance();
    }
    
    private void Awake()
    {
        mv = point.GetComponent<MovePointer>();
    }

    // 선택한 선 지우기
    void FixedUpdate()
    {
        int nCurGesture = processGesture.GetGesture(false);

        if (nCurGesture == 6)
        {
            eraseLine = mv.hitLine;
            if (eraseLine != null)
            {
                eraseLine.GetComponent<ObjState>().SetErase();
                eraseLine.SetActive(false);
                GetComponent<UndoRedoInvoke>().AddRecentDraw(eraseLine);
            }
        }
    }
}
