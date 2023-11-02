using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Joystick : MonoBehaviour
{
    public RectTransform panel;
    public RectTransform text;
    public float resizeSpeed = 0.5f;
    //public AudioSource sound;

    //private bool soundPlayed = false;

    void Start()
    {
        //StartCoroutine(PlayDelayedSound(60f));
    }

    //private IEnumerator PlayDelayedSound(float delay)
    //{
    //    yield return new WaitForSeconds(delay);

    //    if (sound != null && !soundPlayed)
    //    {
    //        sound.Play();
    //        soundPlayed = true;
    //    }
    //}

    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical"); // 조이스틱의 수직 입력

        if (verticalInput > 0) // 위쪽으로 조이스틱을 움직이면
        {
            ResizeUI(resizeSpeed); // 판넬과 텍스트를 확대
        }
        else if (verticalInput < 0) // 아래쪽으로 조이스틱을 움직이면
        {
            ResizeUI(-resizeSpeed); // 판넬과 텍스트를 축소
        }
    }

    void ResizeUI(float scale)
    {
        Vector3 beforePanel = panel.localScale;
        Vector3 beforeText = text.localScale;
        panel.localScale = new Vector3(beforePanel.x + scale, beforePanel.y + scale, beforePanel.z + scale);
        text.localScale = new Vector3(beforeText.x + scale, beforeText.y + scale, beforeText.z + scale);
    }
}
