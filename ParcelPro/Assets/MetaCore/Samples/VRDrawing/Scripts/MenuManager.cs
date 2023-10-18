
using UnityEngine.UI;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu; // 메인 메뉴
    public GameObject drawingManager; // drawingmanager

    [SerializeField] private BoxCollider menuCol; // 메뉴 콜라이더
    [SerializeField] private IPanel currentPanel; // 현재 패널
    [SerializeField] private IPanel[] panels; // 모든 패널 리스트
    [SerializeField] private int state; // 현재 선택된 패널

    private util.ProcessGesture processGesture; 

    int preLeftGesture = -1;

    public enum Menu
    {
        Main,
        Color,
        Brush,
        Erase
    }

    void Start()
    {
        processGesture = util.ProcessGesture.Instance();

        menuCol = GetComponentInParent<BoxCollider>();
        menuCol.enabled = false;
        GetComponent<Image>().enabled = false;
        SetupPanels();
        currentPanel = mainMenu.GetComponent<IPanel>();
        state = 0;
    }

    void Update()
    {
        int leftGesture = processGesture.GetGesture();
        
        if (preLeftGesture != 1 && leftGesture == 1)               //왼손 1 : 메뉴 on
        {
            if (state == 0) // 메뉴 켜기
                MenuOn();
        }

        // if (leftGesture == 2)               //왼손 2 : 메뉴 off
        // {
        //     if (state != 0) // 메뉴 닫기
        //         MenuOff();
        // }

        // if (preLeftGesture != 6 && leftGesture == 6)               //왼손 pick : undo
        // {
        //     drawingManager.GetComponent<UndoRedoInvoke>().Undo();
        // }

        preLeftGesture = leftGesture;
    }
    // 패널 초기화
    private void SetupPanels()
    {
        panels = GetComponentsInChildren<IPanel> ();
        foreach (IPanel panel in panels)
            panel.Setup(this, drawingManager);
    }
    // 패널 바꾸기
    public void ChangePanels(Menu sMenu)
    {
        currentPanel.Hide();
        currentPanel = panels[(int)sMenu];
        state = (int)sMenu;
        currentPanel.Show();
    }
    // 메뉴 켜기
    public void MenuOn()
    {
        GetComponent<Image>().enabled = true;
        menuCol.enabled = true;
        currentPanel.Show();
        state = 1;
    }
    // 메뉴 끄기
    public void MenuOff()
    {
        GetComponent<Image>().enabled = false;
        menuCol.enabled = false;
        currentPanel.Hide();
        currentPanel = mainMenu.GetComponent<IPanel>();
        state = 0;
    }
}