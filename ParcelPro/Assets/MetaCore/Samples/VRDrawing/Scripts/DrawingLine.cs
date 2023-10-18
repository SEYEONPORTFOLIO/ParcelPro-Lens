using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Pnc.Model.Hands;

public class DrawingLine : MonoBehaviour
{
    public GameObject canvas; // 선 그릴 캔버스
    public GameObject point; // 그리는 point
    public Material defaultMaterial; // 선 material

    GameObject line;
    LineRenderer lr;
    MeshCollider ms;

    [SerializeField] private float minDist; // 라인 점 찍힐 최소 거리
    [SerializeField] private float brushSize; // 선 크기
    [SerializeField] private float MaxBrushSize = 0.02f; // 선 최대 크기

    private util.ProcessGesture processGesture;

    public bool isStraight = false; // 직선모드
    public bool isEndPointMode = false; // endpoint 모드
    public bool nowDrawing = false;
    private List<Vector3> points; // 선을 이루는 점 List

    private Vector3 LastPoint // 가장 마지막에 찍힌 점
    {
        get
        {
            if (points == null)
                return point.transform.position;
            return points[points.Count - 1];
        }
    }

    private void Start()
    {
        processGesture = util.ProcessGesture.Instance();
    }

    private void Awake()
    {
        //SetBrushColor(Color.white);
        SetBrushColor(new Color32(255,255,0,255));
        minDist = 0.0002f;
        brushSize = 0.02f;
    }

    void FixedUpdate()
    {
        
        if(processGesture == null)
        {
            Debug.Log("DrawingLine FixedUpdate processGesture is NULL!");
            return;
        }

        int nCurGesture = processGesture.GetGesture(false);
        if (nCurGesture == (int)util.GestureType.PICK)
        {
            GameObject curObject = GameObject.Find("landmark8");

            //if(curObject && curObject.transform.position.x > -1.0f)         //simon 220614 화면 밖의 영역(비활성 상태)은 제외
            if(curObject)                           //simon 220805 Head tracking 적용 후에는 화면이 제한적이지 않다.
            {
                point.transform.position = curObject.transform.position;
            
                Debug.Log("DrawingLine pos: " + curObject.transform.position.x + "..." + curObject.transform.position.y + "..." + curObject.transform.position.z);

                if(nowDrawing)                  //그리는 중
                {
                    ConnectLine(curObject.transform.position);
                }
                else
                {
                    //Debug.Log("DrawingLine CreateLine");
                    nowDrawing = true;          //그리기 시작
                    CreateLine(curObject.transform.position, new GameObject("Line"));
                }
            }
            // else
            // {                
            //     Debug.Log("DrawingLine pos2: " + curObject.transform.position.x + "..." + curObject.transform.position.y + "..." + curObject.transform.position.z);
            // }
        }
        else if (nowDrawing)                    //그리기 완료
        {
            //Debug.Log("DrawingLine Drawing end");
            line.transform.position = Vector3.zero;
            //SetMesh();                    //simon 220726 error!
            GetComponent<UndoRedoInvoke>().AddRecentDraw(line);

            nowDrawing = false;
        }
    }
    // 선 생성
    public void CreateLine(Vector3 drawPoint, GameObject newLine)
    {
        line = newLine;
        line.tag = "Line";
        line.layer = LayerMask.NameToLayer("Line");
        canvas = GameObject.FindWithTag("DrawingCanvas");
        line.transform.parent = canvas.transform;
        line.AddComponent<ObjState>();
        lr = line.AddComponent<LineRenderer>();
        ms = line.AddComponent<MeshCollider>();

        points = new List<Vector3>();

        SetLine();

        points.Add(drawPoint);
        lr.SetPosition(0, drawPoint);
        points.Add(drawPoint);
        lr.SetPosition(1, drawPoint);
    }
    // 생성된 선에 점 추가
    public void ConnectLine(Vector3 drawPoint)
    {
        if (points.Count > 0 && Mathf.Abs(Vector3.Distance(LastPoint, drawPoint)) < minDist)
            return;
        else if (!isStraight)
        {
            points.Add(drawPoint);
            lr.positionCount = points.Count;
            lr.SetPosition(points.Count - 1, drawPoint);
        }
        else
        {
            if (points.Count < 3)
            {
                points.Add(drawPoint);
                lr.positionCount = points.Count;
            }
            else
                points[points.Count - 1] = drawPoint;
            lr.SetPosition(points.Count - 1, drawPoint);
        }
    }
    // Mesh 설정 (지우기 충돌체크 위한 Mesh)
    public void SetMesh()
    {
        Mesh mesh = new Mesh();
        lr.BakeMesh(mesh, true);
        ms.sharedMesh = mesh;
    }
    // 선 설정
    public void SetLine()
    {
        lr.startWidth = brushSize;
        lr.endWidth = brushSize;
        lr.numCornerVertices = 5;
        lr.numCapVertices = 5;
        lr.material = new Material(defaultMaterial);
    }
    // 선 굵기 조절
    public float SetBrushSize()
    {
        brushSize += 0.005f;
        if (brushSize > MaxBrushSize)
            brushSize = 0.005f;
        point.transform.localScale = new Vector3(brushSize, brushSize, brushSize);
        return brushSize;
    }
    // 선 색상 설정
    public void SetBrushColor(Color sColor)
    {
        defaultMaterial.color = sColor;
        point.GetComponent<Renderer>().material = defaultMaterial;
    }
    // 브러쉬 material 설정
    public void SetBrushMat(Material mat)
    {
        mat.color = defaultMaterial.color;
        defaultMaterial = mat;
        point.GetComponent<Renderer>().material = defaultMaterial;
    }
    // Straight Mode
    public void SwapStraightMode()
    {
        isStraight = !isStraight;
    }
}
