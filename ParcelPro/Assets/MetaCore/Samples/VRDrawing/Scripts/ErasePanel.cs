using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErasePanel : MonoBehaviour, IPanel
{
    [SerializeField] private Canvas canvas; // canvas inspector
    [SerializeField] private MenuManager menuManager; // 메뉴 관리 스크립트
    [SerializeField] private GameObject drawingManager; // 그리기 기능 관리 GameObject

    void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
    // Panel Setting
    public void Setup(MenuManager menuManager, GameObject drawingManager)
    {
        this.menuManager = menuManager;
        this.drawingManager = drawingManager;
        Hide();
    }
    // Panel Show
    public void Show()
    {
        Collider[] colls = GetComponentsInChildren<Collider>();
        foreach (Collider coll in colls)
            coll.enabled = true;
        canvas.enabled = true;
    }
    // Panel Hide
    public void Hide()
    {
        Collider[] colls = GetComponentsInChildren<Collider>();
        foreach (Collider coll in colls)
            coll.enabled = false;
        canvas.enabled = false;
    }
    // Button - on click()
    // Eraser Mode
    public void SetEraser()
    {
        MovePointer mp = drawingManager.GetComponentInChildren<MovePointer>();
        mp.SetPrevMode(SelectMode.Mode.Eraser);
        menuManager.ChangePanels(MenuManager.Menu.Main);
    }
    // Button - on click()
    // Erase All
    public void SetEraseAll()
    {
        drawingManager.GetComponent<SelectMode>().SetMode(SelectMode.Mode.Clear);
        menuManager.ChangePanels(MenuManager.Menu.Main);
    }
}
