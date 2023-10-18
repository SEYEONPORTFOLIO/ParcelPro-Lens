using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErasingAll : MonoBehaviour
{
    [SerializeField] private GameObject drawingCanvas; // 선 그려지는 캔버스
    [SerializeField] private ObjState stateScript; // 현재 상태(그리기/지우기 모드) 스크립트
    private readonly string canvasTag = "DrawingCanvas";

    // 캔버스 전체 지우고 새 캔버스 생성
    public void ErasingAllLine()
    {
        drawingCanvas = GameObject.FindGameObjectWithTag(canvasTag);
        drawingCanvas.SetActive(false);
        GetComponent<UndoRedoInvoke>().AddRecentDraw(drawingCanvas);

        if (!(GameObject.FindWithTag(canvasTag)))
        {
            GameObject newCanvas = new GameObject(canvasTag);
            newCanvas.tag = canvasTag;
            stateScript = newCanvas.AddComponent<ObjState>();
            stateScript.SetCanvas();

            drawingCanvas.GetComponent<ObjState>().SetNextCanvas(newCanvas);

            GetComponent<DrawingLine>().canvas = newCanvas;
        }
    }
}
