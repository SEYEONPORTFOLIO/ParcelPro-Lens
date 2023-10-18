using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackgroundScript : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Text description;

    Color color;

    void Awake()
    {
        background = GetComponent<Image>();
        color = background.color;
        description = GetComponentInChildren<Text>();

    }
    // Background 켜기
    public void Select()
    {
        color.a = 0.3f;
        background.color = color;
    }
    // Background 끄기
    public void Deselect()
    {
        color.a = 0f;
        background.color = color;
    }
    public Text GetDescription()
    {
        return description;
    }
}
