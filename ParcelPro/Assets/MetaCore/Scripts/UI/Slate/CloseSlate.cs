using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
public class CloseSlate : MonoBehaviour
{
    public GameObject goPopup = null;
    public Camera mainCamera = null;
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

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Close()
    {
        Debug.Log($"CloseSlate Close start");
        if(!goPopup)
        {
            Debug.Log($"CloseSlate Popup is null");
            return;
        }

        SetAudioSource();
        goPopup.SetActive(false);
    }
    
    private void SetAudioSource()
    {
        Debug.Log($"CloseSlate SetAudioSource");

        AudioSource audio = gameObject.GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
        audio.time = 0.0f;

        audio.Play();
    }
}
