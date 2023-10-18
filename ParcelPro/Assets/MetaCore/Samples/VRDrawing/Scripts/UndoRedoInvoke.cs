using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UndoRedoInvoke : MonoBehaviour
{
    [SerializeField] private List<GameObject> recentDraw; // 최근 그린 기록
    [SerializeField] private List<GameObject> undoHist; // undo 기록
    private readonly int recentLength = 10; // 기록 저장 개수

    void Start()
    {
        recentDraw = new List<GameObject>();
        undoHist = new List<GameObject>();
    }

    // Undo - recentDraw 리스트 마지막 객체 SetActive 바꿈, undoHist에 객체 추가
    public void Undo() 
    {
        if (recentDraw.Count > 0)
        {
            GameObject targetLine = recentDraw[recentDraw.Count - 1];
            ObjState obj = targetLine.GetComponent<ObjState>();
            
            targetLine.SetActive(!targetLine.activeSelf);
            undoHist.Add(targetLine);
            recentDraw.RemoveAt(recentDraw.Count - 1);

            if (obj.GetState() == ObjState.State.canvas)
                obj.GetNextCanvas().SetActive(false);
        }
    }

    // Redo - undoHist 마지막 객체 SetActive 바꿈, recentDraw에 객체 추가
    public void Redo() 
    {
        if (undoHist.Count > 0)
        {
            GameObject targetLine = undoHist[undoHist.Count - 1];
            ObjState obj = targetLine.GetComponent<ObjState>();

            targetLine.SetActive(!targetLine.activeSelf);
            recentDraw.Add(targetLine);
            undoHist.RemoveAt(undoHist.Count - 1);

            if (obj.GetState() == ObjState.State.canvas)
                obj.GetNextCanvas().SetActive(true);
        }
    }

    // recentDraw 추가
    public void AddRecentDraw(GameObject line)
    {
        if (recentDraw.Count > recentLength)
        {
            CheckDelLine(recentDraw[0]);
            recentDraw.RemoveAt(0);
        }
        if (undoHist.Count > 0)
        {
            CheckDelLine(undoHist);
            undoHist.Clear();
        }
        recentDraw.Add(line);
    }

    // 리스트 비우기 (리스트 내 삭제할 Line 체크 후 완전히 제거)
    public void CheckDelLine(List<GameObject> objs)
    {
        foreach (GameObject obj in objs)
        {
            ObjState nowState = obj.GetComponent<ObjState>();
            if (nowState.GetState() == ObjState.State.canvas)
            {
                Destroy(nowState.GetNextCanvas());
                nowState.SetNextCanvas(null);
            }
            else if (obj.activeSelf == false && obj.GetComponent<ObjState>().GetState() != ObjState.State.erase)
                Destroy(obj);
        }
    }

    // 객체 비우기 (삭제할 객체일 경우 완전히 제거)
    public void CheckDelLine(GameObject obj)
    {
        if (obj.activeSelf == false && obj.GetComponent<ObjState>().GetState() != ObjState.State.erase)
            Destroy(obj);
    }
}
