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
        float verticalInput = Input.GetAxis("Vertical"); // ���̽�ƽ�� ���� �Է�

        if (verticalInput > 0) // �������� ���̽�ƽ�� �����̸�
        {
            ResizeUI(resizeSpeed); // �ǳڰ� �ؽ�Ʈ�� Ȯ��
        }
        else if (verticalInput < 0) // �Ʒ������� ���̽�ƽ�� �����̸�
        {
            ResizeUI(-resizeSpeed); // �ǳڰ� �ؽ�Ʈ�� ���
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
