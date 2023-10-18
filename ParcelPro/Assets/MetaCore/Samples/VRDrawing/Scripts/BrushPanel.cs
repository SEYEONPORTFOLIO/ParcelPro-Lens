using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BrushPanel : MonoBehaviour, IPanel
{
    [SerializeField] private Canvas canvas; // canvas inspector
    [SerializeField] private MenuManager menuManager; // MenuManager Script
    [SerializeField] private GameObject drawingManager; // DrawingManager GameObject
    [SerializeField] private MenuBackgroundScript[] menus; // Menu Background Script
    
    public Material[] mat; // Brush Material

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        menus = GetComponentsInChildren<MenuBackgroundScript>();
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
        foreach (Collider coll in colls)
            coll.enabled = false;
        canvas.enabled = false;
    }

    // Button - on click()
    // Select Brush
    public void SetBrushTexture(int menu)
    {
        drawingManager.GetComponent<DrawingLine>().SetBrushMat(mat[menu]);
        MovePointer mp = drawingManager.GetComponentInChildren<MovePointer>();
        mp.SetPrevMode(SelectMode.Mode.Brush);
        menuManager.ChangePanels(MenuManager.Menu.Main);
    }

    // Button - on click()
    // Straight Mode
    public void SetStraightMode()
    {
        DrawingLine lineScript = drawingManager.GetComponent<DrawingLine>();
        lineScript.SwapStraightMode();

        Text desc = menus[2].GetDescription();
        if (lineScript.isStraight)
            desc.text = "Straight\nOff";
        else
            desc.text = "Straight\nOn";
        menuManager.ChangePanels(MenuManager.Menu.Main);
    }

    // Button - on click()
    // EndPoint Mode
    public void SetEndPointMode()
    {
        MovePointer mp = drawingManager.GetComponentInChildren<MovePointer>();
        mp.SetEndPointMode();
        Text desc = menus[3].GetDescription();
        if (mp.GetEndPointMode())
            desc.text = "EndPoint\nOff";
        else
            desc.text = "EndPoint\nOn";
        menuManager.ChangePanels(MenuManager.Menu.Main);
    }
}