using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPanel : MonoBehaviour, IPanel
{
    [SerializeField] private Canvas canvas; // canvas inspector
    [SerializeField] private MenuManager menuManager; // 메뉴 관리 스크립트
    [SerializeField] private GameObject drawingManager; // 그리기 기능 관리 GameObject
    
    List<Image> colors; // ui image

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        colors = new List<Image>();
        CheckChildren(GetComponentsInChildren<Image>());
    }
    // Panel Setting
    public void Setup(MenuManager menuManager, GameObject drawingManager)
    {
        this.menuManager = menuManager;
        this.drawingManager = drawingManager;
        Hide();
    }
    // Color Menu List 설정
    private void CheckChildren(Image[] children)
    {
        foreach(Image child in children)
        {
            if (child.CompareTag("Color"))
                colors.Add(child);
        }
    }
    // Show Panel
    public void Show()
    {
        Collider[] colls = GetComponentsInChildren<Collider>();
        foreach (Collider coll in colls)
            coll.enabled = true;
        canvas.enabled = true;
    }
    // Hide Panel
    public void Hide()
    {
        Collider[] colls = GetComponentsInChildren<Collider>();
        foreach (Collider coll in colls)
            coll.enabled = false;
        canvas.enabled = false;
    }
    // Button - on click()
    // Set Brush Color
    public void SetColor(int menu)
    {
        Image sColor = colors[menu];
        drawingManager.GetComponent<DrawingLine>().SetBrushColor(sColor.color);
        MovePointer mp = drawingManager.GetComponentInChildren<MovePointer>();
        mp.SetPrevMode(SelectMode.Mode.Brush);
        menuManager.ChangePanels(MenuManager.Menu.Main);
    }
}
