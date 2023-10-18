using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjState : MonoBehaviour
{
    // 선, 캔버스의 상태 저장
    public enum State // 선(그려짐, 지워짐 상태), 캔버스 구분
    {
        draw,
        erase,
        canvas
    }

    [SerializeField] private State nowState;
    [SerializeField] private GameObject nextCanvas;

    public void Awake()
    {
        if (gameObject.CompareTag("DrawingCanvas"))
            SetCanvas();
        else
            SetDraw();
    }

    public void SetDraw() { nowState = State.draw; }
    public void SetErase() { nowState = State.erase; }
    public void SetCanvas() { nowState = State.canvas; }
    public void SetNextCanvas(GameObject canvas) { nextCanvas = canvas; }
    public State GetState() { return nowState; }
    public GameObject GetNextCanvas() { return nextCanvas; }
}
