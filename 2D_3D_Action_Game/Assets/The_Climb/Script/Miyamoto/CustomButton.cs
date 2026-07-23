using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Image))]

public class CustomButton : MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("ボタンの設定")]
    [SerializeField] private float pressedScale = 0.9f;
    [SerializeField] private float presseTransparent = 0.6f;

    private Vector3 defaultScale;
    private Image image;
    private Color defaultColor;

    public Action onClickCallBack;

    void Awake()
    {
        image = GetComponent<Image>();
        defaultScale = transform.localScale;
        defaultColor = image.color;
    }

    // 押した瞬間
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = defaultScale * pressedScale;

        Color c = defaultColor;
        c.a = presseTransparent;
        image.color = c;
    }

    // 離した瞬間
    public void OnPointerUp(PointerEventData eventData)
    {
        ResetVisual();
    }

    // クリック確定
    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallBack?.Invoke();
    }

    private void ResetVisual()
    {
        transform.localScale = defaultScale;
        image.color = defaultColor;
    }
}


