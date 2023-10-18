using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pnc.Model.Hands;

// 그리기 pointer 이동 스크립트
public class MovePointer : MonoBehaviour
{
    public float pointerDist = 0.1f;
    public GameObject hitLine;
    public float closeRange = 0.001f; // end point 인식 범위

    [SerializeField] private GameObject nowMenu;
    [SerializeField] private SelectMode selectMode;
    [SerializeField] private SelectMode.Mode prevMode;
    [SerializeField] private DrawingLine drawingLine;
    [SerializeField] private bool isEndPointMode;

    private util.ProcessGesture processGesture;

    RaycastHit hitInfo;
    
    GameObject line;
    LineRenderer lr;

    Vector3 vPrePick = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 vPreNew = new Vector3(0.0f, 0.0f, 0.0f);

    void Start()
    {
        Debug.Log("MovePointer Start");
        processGesture = util.ProcessGesture.Instance();
        if(processGesture == null)
        {
            Debug.LogError($"MovePointer processGesture is null");
        }
    }

    void Awake()
    {
        prevMode = 0;
        nowMenu = null;
        hitLine = null;

        GameObject curObject = GameObject.Find("landmark8");            //오른손 손가락 끝점
        transform.position = curObject.transform.position;
        isEndPointMode = false;
    }
    // RHand에서 일정 거리만큼 raycast 발생
    // Terrain, Line, UI 있을 경우 해당 위치로 조정
    void FixedUpdate()
    {
        if(processGesture == null)
        {
            Debug.LogError($"MovePointer FixedUpdate processGesture is null");
            processGesture = util.ProcessGesture.Instance();
        }

        Hand rightHand = processGesture.GetHand(false);
        if(rightHand == null)
        {
            return;
        }

        Vector3 vPick;
        Vector3 vNew;
        
        // if(processGesture.fixRaycast)           //손가락이 PICK 동작 중일 때는 Ray를 움직이지 않는다.
        // if(false)
        // {
        //     vPick = vPrePick;
        //     vNew = vPreNew;
        // }
        // else
        {
            List<Landmark> landmarks = rightHand.getLandmarks();

            //손바닥 식별
            Vector3 v9 = new Vector3(landmarks[9].x, landmarks[9].y, landmarks[9].z);
            Vector3 v0 = new Vector3(landmarks[0].x, landmarks[0].y, landmarks[0].z);
            //Vector3 v13 = new Vector3(landmarks[13].x, landmarks[13].y, landmarks[13].z);
            
            Vector3 v4 = new Vector3(landmarks[4].x, landmarks[4].y, landmarks[4].z);
            //Vector3 v1 = new Vector3(landmarks[1].x, landmarks[1].y, landmarks[1].z);
            Vector3 v8 = new Vector3(landmarks[8].x, landmarks[8].y, landmarks[8].z);
            Vector3 v17 = new Vector3(landmarks[17].x, landmarks[17].y, landmarks[17].z);
            vPick = GetCenter(v4, v8);

            //외적(법선벡터)
            vNew = GetNormal(v17, v9, v0);
            var quaternion = Quaternion.Euler(0, 30, 0);
            Vector3 newDirection = quaternion * vNew;
            vNew = newDirection;
        }
        //특정 방향으로 회전(15도 위쪽)
        //var quaternion = Quaternion.Euler(-15, 0, 0);

        vPrePick = vPick;
        vPreNew = vNew;
        
        // List<Landmark> landmarks = rightHand.getLandmarks();
        
        // //손바닥 식별
        // Vector3 v9 = new Vector3(landmarks[9].x, landmarks[9].y, landmarks[9].z);
        // Vector3 v0 = new Vector3(landmarks[0].x, landmarks[0].y, landmarks[0].z);
        
        // Vector3 v4 = new Vector3(landmarks[4].x, landmarks[4].y, landmarks[4].z);
        // Vector3 v8 = new Vector3(landmarks[8].x, landmarks[8].y, landmarks[8].z);
        // Vector3 v17 = new Vector3(landmarks[17].x, landmarks[17].y, landmarks[17].z);
        // Vector3 vPick = GetCenter(v4, v8);

        // //외적(법선벡터)
        // Vector3 vNew = GetNormal(v17, v9, v0);

        int mask = (1 << 0) | (1 << 5);         // Default or UI layer.

        Ray ray = new Ray(vPick, vNew);
        if (Physics.Raycast(ray, out hitInfo, pointerDist, mask))
        {
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("UI"))       // UI에 맞았을 때
            {
                if (drawingLine.nowDrawing) // 현재 라인 그리는 중이면 UI 패널 통과
                {
                    Debug.Log("MovePointer Raycast drawingLine.nowDrawing");
                    transform.position = vPick + transform.forward * pointerDist;
                    nowMenu = null;
                }
                else
                {
                    Debug.Log("MovePointer FixedUpdate hit name: " + hitInfo.transform.name);
                    transform.position = hitInfo.point;
                    if (hitInfo.transform.name.StartsWith("ButtonColl") == true)
                    {
                        nowMenu = hitInfo.transform.gameObject;
                        if (selectMode.GetMode() != SelectMode.Mode.UI) // UI모드가 아니면 - UI모드 On
                        {
                            prevMode = selectMode.GetMode();
                            selectMode.SetMode(SelectMode.Mode.UI);
                        }
                    }
                }
            }
            else
            {
                transform.position = new Vector3((vPick.x+(vNew.x*pointerDist)), (vPick.y+(vNew.y*pointerDist)), (vPick.z+(vNew.z*pointerDist)));
            }
        }
        else // 맞은 물체가 없을 경우
        {
            transform.position = new Vector3((vPick.x+(vNew.x*pointerDist)), (vPick.y+(vNew.y*pointerDist)), (vPick.z+(vNew.z*pointerDist)));

            if (selectMode.GetMode() == SelectMode.Mode.UI)
                selectMode.SetMode(prevMode);
        }
    }

    public void SetPointer(SelectMode modeScript, DrawingLine drawingLine)
    {
        this.selectMode = modeScript;
        this.drawingLine = drawingLine;
    }

    public void SetPrevMode(SelectMode.Mode mode)
    {
        prevMode = mode;
    }

    public void SetEndPointMode()
    {
        isEndPointMode = !isEndPointMode;
    }

    public bool GetEndPointMode()
    {
        return isEndPointMode;
    }
    public GameObject GetNowMenu()
    {
        return nowMenu;
    }
    private void CheckEndPoint()
    {
        Vector3 nowPos = transform.position;
        Collider[] hitObjects = Physics.OverlapSphere(nowPos, closeRange, 1 << LayerMask.NameToLayer("Line"));

        if (hitObjects.Length <= 0) return; // 주변에 선 없으면 함수 종료

        List<Vector3> nearPoints = new List<Vector3>();
        LineRenderer line;

        // 근처의 모든 선에서 시작점, 끝점 추출
        for (int i = 0; i < hitObjects.Length; i++)
        {
            line = hitObjects[i].GetComponent<LineRenderer>(); 
            nearPoints.Add(line.GetPosition(0));
            nearPoints.Add(line.GetPosition(line.positionCount - 1));
        }

        // 가장 가까운 점 추출
        int closest = -1;
        float nowDist = closeRange;
        for (int i = 0; i < nearPoints.Count; i++)
        {
            float nextDist = Vector3.Distance(nearPoints[i], nowPos); // 끝점과 현재의 거리
            if (nowDist > nextDist)
            {
                nowDist = nextDist;
                closest = i;
            }
        }

        if (closest >= 0)
            transform.position = nearPoints[closest];
    }

    // Get the normal to a triangle from the three corner points, a, b and c.
    Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        // Find vectors corresponding to two of the sides of the triangle.
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        //Debug.Log("Raycast GetNormal: side1: " + side1.x + ", " + side1.y +  ", " + side1.z);
        //Debug.Log("Raycast GetNormal: side2: " + side2.x + ", " + side2.y +  ", " + side2.z);
        // Cross the vectors to get a perpendicular vector, then normalize it.
        return Vector3.Cross(side1, side2).normalized;
    }
    
    Vector3 GetCenter(Vector3 a, Vector3 b)
    {
        Debug.Log("Raycast GetCenter: aaa: " + a.x + ", " + a.y +  ", " + a.z);
        Debug.Log("Raycast GetCenter: bbb: " + b.x + ", " + b.y +  ", " + b.z);

        Vector3 c = new Vector3((a.x + b.x)/2, (a.y + b.y)/2, (a.z + b.z)/2);
        Debug.Log("Raycast GetCenter: ccc: " + c.x + ", " + c.y +  ", " + c.z);

        return new Vector3((a.x + b.x)/2, (a.y + b.y)/2, (a.z + b.z)/2);
    }
}
