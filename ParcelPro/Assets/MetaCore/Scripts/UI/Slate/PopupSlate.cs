
using UnityEngine;
using DG.Tweening;

public class PopupSlate : MonoBehaviour
{
    public GameObject goPopup = null;
    public Camera mainCamera = null;
    public float centerDepth = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        if(mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCam").GetComponent<Camera>();
            if(mainCamera == null)
                mainCamera = GameObject.Find("Eye Left")?.GetComponent<Camera>();
        }
    }

    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Popup()
    {
        Debug.Log($"PopupSlate Popup start");
        if(!goPopup)
        {
            Debug.Log($"PopupSlate Popup is null");
            return;
        }

        if(goPopup.activeSelf == true)
        {
            Debug.Log($"PopupSlate Popup is active! return.");
            return;
        }

        goPopup.SetActive(true);
        SetAudioSource();
        
        // Vector3 vNew = mainCamera.ViewportToWorldPoint(new Vector3(0.5f,0.5f,centerDepth));
        // Debug.Log($"PopupSlate Popup new x:{vNew.x}, y:{vNew.y}, z:{vNew.z}");
        
        // goPopup.transform.DOMove(vNew, 1);
    }
    
    private void SetAudioSource()
    {
        Debug.Log($"PopupSlate SetAudioSource");

        AudioSource audio = gameObject.GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
        audio.time = 0.0f;

        audio.Play();
    }
}
