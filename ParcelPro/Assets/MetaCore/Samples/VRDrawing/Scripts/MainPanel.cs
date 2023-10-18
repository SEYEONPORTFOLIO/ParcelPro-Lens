using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : MonoBehaviour, IPanel
{
    [SerializeField] private Canvas canvas; // canvas inspector
    [SerializeField] private MenuManager menuManager; // 메뉴 관리 스크립트
    [SerializeField] private GameObject drawingManager; // 그리기 기능 관리 GameObject
    
    public GameObject brushSize; // 브러쉬 사이즈 오브젝트
    
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
        foreach(Collider coll in colls)
            coll.enabled = false;
        canvas.enabled = false;
    }

    // Button - on click()
    // Brush Color
    public void SetColor()
    {
        menuManager.ChangePanels(MenuManager.Menu.Color);
    }

    // Button - on click()
    // Brush Size
    public void SizeUp()
    {
        float bSize = drawingManager.GetComponent<DrawingLine>().SetBrushSize();
        brushSize.GetComponent<RectTransform>().sizeDelta = new Vector2(bSize * 10, bSize * 10);
    }

    // Button - on click()
    // Program Exit
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Button - on click()
    // Undo
    public void Undo()
    {
        drawingManager.GetComponent<UndoRedoInvoke>().Undo();
    }

    // Button - on click()
    // Redo
    public void Redo()
    {
        drawingManager.GetComponent<UndoRedoInvoke>().Redo();
    }

    // Button - on click()
    // Erase Menu
    public void Erasing()
    {
        menuManager.ChangePanels(MenuManager.Menu.Erase);
    }

    // Button - on click()
    // Brush Menu
    public void SelectBrush()
    {
        menuManager.ChangePanels(MenuManager.Menu.Brush);
    }
}
