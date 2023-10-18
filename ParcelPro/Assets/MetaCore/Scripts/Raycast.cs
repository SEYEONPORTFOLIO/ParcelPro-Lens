using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Pnc.Model.Hands;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
    private RaycastHit hit;
    private GameObject hitObj;
    //private GameObject selObj;
    private string selObjName;
    private float dist = 2.0f;

    private Renderer rendHit;
    private Color clrNormal;

    private LineRenderer line;

    private util.ProcessGesture processGesture;

    private DateTime lastHitTime;                       //simon 220920 마지막 실행 시각(중복 실행 방지)
    
    // Start is called before the first frame update
    void Start()
    {
        lastHitTime = DateTime.Now;

        hitObj = GameObject.Find("HitRay");          //Ray가 닿았다는 표시

        Color clrOrg = new Color32(50,50,50,175);
        rendHit = hitObj.GetComponent<Renderer>();
        rendHit.material.color = clrOrg;

        selObjName = "";

        createLine();
        DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), Color.white);
        
        processGesture = util.ProcessGesture.Instance();
        if(processGesture == null)
        {
            Debug.Log($"Raycast processGesture is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(processGesture == null)
        {
            Debug.Log($"Raycast Update processGesture is null");
            processGesture = util.ProcessGesture.Instance();
        }

        Hand rightHand = processGesture.GetHand(false);
        if(rightHand == null)
        {
            //Debug.Log($"Raycast Update rightHand is null");
            return;
        }

        List<Landmark> landmarks = rightHand.getLandmarks();

        //손바닥 식별
        Vector3 v9 = new Vector3(landmarks[9].x, landmarks[9].y, landmarks[9].z);
        Vector3 v0 = new Vector3(landmarks[0].x, landmarks[0].y, landmarks[0].z);
        //Vector3 v13 = new Vector3(landmarks[13].x, landmarks[13].y, landmarks[13].z);
        
        Vector3 v4 = new Vector3(landmarks[4].x, landmarks[4].y, landmarks[4].z);
        //Vector3 v1 = new Vector3(landmarks[1].x, landmarks[1].y, landmarks[1].z);
        Vector3 v8 = new Vector3(landmarks[8].x, landmarks[8].y, landmarks[8].z);
        Vector3 v17 = new Vector3(landmarks[17].x, landmarks[17].y, landmarks[17].z);
        Vector3 vPick = GetCenter(v4, v8);

        //외적(법선벡터)
        //Vector3 vNew = GetNormal(v13, v9, v0);
        Vector3 vNew = GetNormal(v17, v9, v0);
        
        //test
        // Vector3 v111 = new Vector3(landmarks[5].x, landmarks[5].y, landmarks[5].z);
        // Vector3 vTmp = new Vector3(vPick.x-v111.x, vPick.y-v111.y, vPick.z-v111.z);

        // Vector3 vNew = vTmp.normalized;
        //test end

        //특정 방향으로 회전(15도 위쪽)
        var quaternion = Quaternion.Euler(-15, 0, 0);
        Vector3 newDirection = quaternion * vNew;
        
        if(line == null)
            createLine();

        //int mask = (1 << 0) | (1 << 5);         // Default or UI layer.

        Ray ray = new Ray(vPick, vNew);
        //Ray ray = new Ray(vPick, vPick-v1);
        if (Physics.Raycast(ray, out hit, dist))
        //if (Physics.Raycast(ray, out hit, dist, mask))
        {
            //Debug.Log("Raycast Update hit tag: " + hit.transform.tag);
            

            //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))       // UI에 맞았을 때
            if(hit.transform.gameObject.CompareTag("Icon"))
            //if (hit.transform.name == "Icon_(Clone)")       // UI에 맞았을 때            
            {
                DrawLine(vPick, hit.transform.position, new Color32(59,135,192,255));
                OnFocusIcon(hit.transform.gameObject.name);

                // hitObj = GameObject.Find("HitRay");          //Ray가 닿았다는 표시
                // hitObj.transform.position = hit.transform.position;
                // //hitObj.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                // rendHit = hitObj.GetComponent<Renderer>();
                // rendHit.material.color = Color.blue;

                Debug.Log("Raycast Update icon selObjName: " + selObjName + ", current: " + hit.transform.name);
                if(selObjName != "" && hit.transform.name.Equals(selObjName) == false)
                {
                    GameObject selObj = GameObject.Find(selObjName);    //이전에 선택된 아이콘 색은 복구
                    if(selObj != null)
                    {
                        Image preImage = selObj.GetComponent<Image>();
                        preImage.color = new Color32(59,135,192,255);
                        Debug.Log("Raycast Update icon preImage: " + selObjName);
                    }

                }

                GameObject go = GameObject.Find(hit.transform.name);
                if(go != null)
                {
                    selObjName = hit.transform.name;
                    Image hitImage = go.GetComponent<Image>();
                    hitImage.color = Color.green;
                    Debug.Log("Raycast Update icon hitImage: " + hit.transform.name);
                }
                
                //Image iconImage = hit.transform.gameObject.GetComponent<Image>();
                // Image panelImage = this.GetComponent<Image>();
                // panelImage.color = Color.green;

                if(rightHand.gesture == (int)util.GestureType.PICK)
                {
                    Debug.Log("Raycast Update Hit PICK!!" + hit.transform.gameObject.name);
                    
                    TimeSpan diff = DateTime.Now - lastHitTime;

                    if(diff.TotalMilliseconds <= 200)                       //이미 실행됨
                    {
                        Debug.Log("Raycast Update already launched");
                        return;
                    }

                    Button button = hit.transform.gameObject.GetComponent<Button>();

                    if (button != null)
                    {
                        processGesture.sleepCnt = 10;               //실행 후에 밀린 데이터들 무시
                        processGesture.DisableLandmarks(false, false);

                        lastHitTime = DateTime.Now;
                        button.onClick.Invoke();
                    }

                    TextMesh textObject = GameObject.Find("Trigger test").GetComponent<TextMesh>();
                    if(textObject != null)
                    {
                        textObject.text = hit.transform.name;
                        textObject.color = Color.white;
                    }
                }
            }
            else
            {
                if(selObjName.Equals("") == false)
                {
                    GameObject selObj = GameObject.Find(selObjName);
                    if(selObj != null)
                    {
                        Debug.Log("Raycast Update icon preImage: " + selObjName);
                        Image preImage = selObj.GetComponent<Image>();
                        preImage.color = new Color32(59,135,192,255);
                        selObjName = "";
                    }
                }

                Vector3 vEnd = new Vector3((vPick.x+(vNew.x*dist)), (vPick.y+(vNew.y*dist)), (vPick.z+(vNew.z*dist)));
                
                DrawLine(vPick, vEnd, Color.white);
                
                hitObj = GameObject.Find("HitRay");
                hitObj.transform.position = vEnd;
            }
        }
        else
        {
            if(rightHand.gesture == (int)util.GestureType.THREE)                      //Two 제스처일 때 현재 카메라의 화면 중앙으로 이동
            {
                Debug.Log("Raycast Update Hit TWO!!");
                Camera camera = GameObject.FindGameObjectWithTag("MainCam").GetComponent<Camera>();
                if (camera == null)
                {
                    camera = GameObject.Find("Eye Left")?.GetComponent<Camera>();
                }

                Vector3 vCenter = camera.ViewportToWorldPoint(new Vector3(0.5f,0.5f,1.0f));
                Debug.Log($"Raycast Update Hit Center x:{vCenter.x}, y:{vCenter.y}, z:{vCenter.z}");

                Canvas mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
                mainCanvas.transform.position = vCenter;
                mainCanvas.transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward*3.0f);
            }

            Vector3 vEnd = new Vector3((vPick.x+(vNew.x*dist)), (vPick.y+(vNew.y*dist)), (vPick.z+(vNew.z*dist)));
            
            DrawLine(vPick, vEnd, Color.white);

            hitObj = GameObject.Find("HitRay");
            hitObj.transform.position = vEnd;

            Color clrOrg = new Color32(50,50,50,175);
            rendHit = hitObj.GetComponent<Renderer>();
            rendHit.material.color = clrOrg;
        }

        //simon 220922 스크롤은 Raycast와 관계없이 동작
        if(rightHand.gesture == (int)util.GestureType.TWO)                      //Scroll Down
        {
            ScrollRect scrollRect = GameObject.Find("AppList_Scroll View").GetComponent<ScrollRect>();

            RectTransform scroll = scrollRect.content.GetComponent<RectTransform>();
            Debug.Log("Raycast Update Hit TWO!!" + "x: " + scroll.anchoredPosition.x + ", y: " + scroll.anchoredPosition.y);
            scrollRect.content.anchoredPosition = new Vector2(scroll.anchoredPosition.x, scroll.anchoredPosition.y + 10);
        }
        else if(rightHand.gesture == (int)util.GestureType.FOUR)                      //Scroll Up
        {
            ScrollRect scrollRect = GameObject.Find("AppList_Scroll View").GetComponent<ScrollRect>();

            RectTransform scroll = scrollRect.content.GetComponent<RectTransform>();
            Debug.Log("Raycast Update Hit FOUR!!" + "x: " + scroll.anchoredPosition.x + ", y: " + scroll.anchoredPosition.y);
            scrollRect.content.anchoredPosition = new Vector2(scroll.anchoredPosition.x, scroll.anchoredPosition.y - 10);
        }
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
        // Debug.Log("Raycast GetCenter: aaa: " + a.x + ", " + a.y +  ", " + a.z);
        // Debug.Log("Raycast GetCenter: bbb: " + b.x + ", " + b.y +  ", " + b.z);

        Vector3 c = new Vector3((a.x + b.x)/2, (a.y + b.y)/2, (a.z + b.z)/2);
        //Debug.Log("Raycast GetCenter: ccc: " + c.x + ", " + c.y +  ", " + c.z);

        return new Vector3((a.x + b.x)/2, (a.y + b.y)/2, (a.z + b.z)/2);
    }
    
    private void createLine()
    {
        line = new GameObject("Line").AddComponent<LineRenderer>();
        //line.transform.parent = hitObj.transform;
        line.useWorldSpace = true;
    }

    private void DrawLine(Vector3 vStart, Vector3 vEnd, Color color)
    {
        //return;         //simon for test

        //line.SetWidth(0.003f, 0.003f);
        line.startWidth = 0.001f;
        line.endWidth = 0.001f;
        
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = color;
        //line.sharedMaterial.SetColor("Color", Color.blue);
        line.positionCount = 2;
        line.SetPosition(0, vStart);
        line.SetPosition(1, vEnd);
    }

    private void OnFocusIcon(string iconName)
    {
        GameObject icon = GameObject.Find(iconName);
        if(icon)
        {
            //icon.GetComponent<Renderer>().material.color = Color.red;

            Image img = icon.GetComponent<Image>();
            if(img)
            {
                Debug.Log($"Raycast OnFocusIcon icon prefab name={iconName}, change color Img={img.color.r},{img.color.g},{img.color.b}");
//                img.material.color = Color.green;
//                img.color = Color.red;
                img.color = new Color32(200,135,193,255);
                Debug.Log($"Raycast OnFocusIcon icon prefab2 name={iconName}, change color Img={img.color.r},{img.color.g},{img.color.b}");
            }
        }

        // Debug.Log($"Raycast OnFocusIcon Start icon={icon.transform.name}");
        
        // TMPro.TextMeshProUGUI txt = icon.GetComponent<TMPro.TextMeshProUGUI>();
        // if(txt)
        // {
        //     Debug.Log($"Raycast OnFocusIcon text={txt.text}");
        // }

        // Image img = icon.GetComponent<Image>();
        // if(img)
        // {
        //     Debug.Log($"Raycast OnFocusIcon icon name={icon.transform.name}, change color Img={img.color.r},{img.color.g},{img.color.b}");
        //     img.color = Color.red;
        // }

        // GameObject[] gos = GameObject.FindGameObjectsWithTag("Icon");
        // foreach(GameObject go in gos)
        // {
        //     TextMesh txt = go.GetComponent<TextMesh>();

        //     if(go == icon)
        //     {
        //         Debug.Log($"Raycast OnFocusIcon icon found={go.transform.name}, text={txt.text}");
        //     }
        //     if(txt)
        //     {
        //         Debug.Log($"Raycast OnFocusIcon icon name={go.transform.name}, text={txt.text}");
        //     }
        // }
        
        Debug.Log($"Raycast OnFocusIcon End");
    }
}
