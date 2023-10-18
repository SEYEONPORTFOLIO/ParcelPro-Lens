
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TargetingMenu : MonoBehaviour
{
    [SerializeField] private MovePointer movePointer;
    [SerializeField] private GameObject nowMenu;
    [SerializeField] private Button nowPanel;
    [SerializeField] private GameObject prevMenu;

    private util.ProcessGesture processGesture; 
    int preRightGesture = -1;

    void Start()
    {
        processGesture = util.ProcessGesture.Instance();

        movePointer = GetComponentInChildren<MovePointer>();
        prevMenu = null;
    }
    void Update()
    {
        nowMenu = movePointer.GetNowMenu();
        if (nowMenu != null)
        {
            nowPanel = nowMenu.GetComponentInParent<Button>();

            if (nowPanel != null)
            {
                if (prevMenu == null)
                    nowMenu.GetComponent<MenuBackgroundScript>().Select();
                else if (nowMenu != prevMenu)
                {
                    prevMenu.GetComponent<MenuBackgroundScript>().Deselect();
                    nowMenu.GetComponent<MenuBackgroundScript>().Select();
                }
                prevMenu = nowMenu;
            }
            else if (prevMenu != null)
            {
                prevMenu.GetComponent<MenuBackgroundScript>().Deselect();
                prevMenu = null;
            }
        }
        
        int nCurGesture = processGesture.GetGesture(false);
        if (preRightGesture != 6 && nCurGesture == 6)
            SelectUIButton();

        preRightGesture = nCurGesture;
    }

    void SelectUIButton()
    {
        Debug.Log("TargetingMenu SelectUIButton Menu: " + nowMenu.transform.name);
        if (nowMenu != null && nowMenu.CompareTag("Button"))
            nowMenu.GetComponentInParent<Button>().onClick.Invoke();
    }
}
