using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMode : MonoBehaviour
{
    public GameObject pointer;
    [SerializeField] private DrawingLine drawingLine;
    [SerializeField] private ErasingLine erasingLine;
    [SerializeField] private TargetingMenu targetingMenu;
    [SerializeField] private ErasingAll erasingAll;
    [SerializeField] private MovePointer movePointer;
    [SerializeField] private Mode mode;
    public enum Mode
    {
        Brush,
        Eraser,
        Clear,
        UI
    }
    void Start()
    {
        drawingLine = GetComponent<DrawingLine>();
        erasingLine = GetComponent<ErasingLine>();
        erasingAll = GetComponent<ErasingAll>();
        targetingMenu = GetComponent<TargetingMenu>();
        movePointer = pointer.GetComponent<MovePointer>();
        movePointer.SetPointer(this, drawingLine);
        mode = Mode.Brush; // 시작 시 Brush 모드 기본
        SetMode(mode);
    }
    // mode에 따라 Drawing, Erasing swap
    public void SetMode(int mode)
    {
        switch ((Mode)mode)
        {
            case Mode.Brush:
                this.mode = (Mode)mode;
                drawingLine.enabled = true;
                erasingLine.enabled = false;
                targetingMenu.enabled = false;
                break;
            case Mode.Eraser:
                this.mode = (Mode)mode;
                drawingLine.SetBrushColor(Color.white);
                drawingLine.enabled = false;
                erasingLine.enabled = true;
                targetingMenu.enabled = false;
                break;
            case Mode.Clear:
                erasingAll.ErasingAllLine();
                break;
            case Mode.UI:
                this.mode = (Mode)mode;
                drawingLine.enabled = false;
                erasingLine.enabled = false;
                targetingMenu.enabled = true;
                break;
        }
    }
    // Overloarding
    public void SetMode(Mode mode)
    {
        switch ((Mode)mode)
        {
            case Mode.Brush:
                this.mode = mode;
                drawingLine.enabled = true;
                erasingLine.enabled = false;
                targetingMenu.enabled = false;
                break;
            case Mode.Eraser:
                this.mode = mode;
                drawingLine.SetBrushColor(Color.white);
                drawingLine.enabled = false;
                erasingLine.enabled = true;
                targetingMenu.enabled = false;
                break;
            case Mode.Clear:
                erasingAll.ErasingAllLine();
                break;
            case Mode.UI:
                this.mode = mode;
                drawingLine.enabled = false;
                erasingLine.enabled = false;
                targetingMenu.enabled = true;
                break;
        }
    }

    public Mode GetMode()
    {
        return mode;
    }
}
