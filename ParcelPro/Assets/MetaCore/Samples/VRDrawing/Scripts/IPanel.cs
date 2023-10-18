using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 패널 인터페이스
public interface IPanel
{
    void Setup(MenuManager menuManager, GameObject drawingManager);
    void Show();
    void Hide();
}
