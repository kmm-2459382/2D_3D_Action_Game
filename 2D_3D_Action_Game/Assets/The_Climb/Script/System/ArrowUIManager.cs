using UnityEngine;
using System.Collections.Generic;


public class ArrowUIManager : MonoBehaviour
{
    RectTransform arrowRectTransform = null;

    private Transform target;  //指すもの
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform arrowRect;
    private GameObject player;

    private float rateScale = 4.5f;
    private float scaleMin = 0.5f;
    private float scaleMax = 1.5f;

    [SerializeField] private Vector2 edgeBuffer = new Vector2(50f, 50f);  //画面端からの余白

    [SerializeField] private List<Transform> buttons;
    private HashSet<Transform> pressedButtons = new HashSet<Transform>();

    void Start()
    {
        arrowRectTransform = GetComponent<RectTransform>();

        if (GameObject.FindWithTag("Player") != null)
        {
            player = GameObject.FindWithTag("Player");
        }

        arrowRectTransform.localScale = Vector3.one;
        target = buttons[0].transform;
    }

    void Update()
    {
        if (player.transform.position.x > 600f)
        {
            foreach (Transform btn in buttons)
            {
                PressureButton pb = btn.GetComponent<PressureButton>();
                if (pb.pressCount != 0 && !pressedButtons.Contains(btn))
                {
                    OnSwitchPressed(btn);
                }
            }

            //ターゲット設定
            SetTarget();

            //ターゲットが無ければ非表示
            if (target == null)
            {
                arrowRect.gameObject.SetActive(false);
                return;
            }

            //ターゲットのスクリーン座標取得
            Vector3 screenPos = uiCamera.WorldToScreenPoint(target.position);

            //画面内判定
            bool isOnScreen =
            screenPos.z > 0 &&
            screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height;

            //画面内なら非表示
            if (isOnScreen)
            {
                arrowRect.gameObject.SetActive(false);
                ScreenInArrow(screenPos);
            }
            else
            {
                ScreenOutArrow(screenPos);
            }
        }
        else
        {
            arrowRect.gameObject.SetActive(false);
        }
    }
    //各ボタンの距離を調べ、ターゲットを設定
    void SetTarget()
    {
        float minDistance = float.MaxValue;
        Transform nearest = null;

        foreach (Transform btn in buttons)
        { 
            // 押されたスイッチは無視
            if (pressedButtons.Contains(btn)) 
                continue; 

            //距離計算
            Vector3 toTarget = btn.position - uiCamera.transform.position; 
            float distance = toTarget.magnitude; 

            //最短距離更新
            if (distance < minDistance) 
            { 
                minDistance = distance; 
                nearest = btn; 
            } 
        }
        
        target = nearest;
    }

    //スイッチが押されたときに呼び出される
    private void OnSwitchPressed(Transform pressedSwitch)
    {
        if (!pressedButtons.Contains(pressedSwitch))
        {
            pressedButtons.Add(pressedSwitch);
        }
    }

    //画面内矢印表示
    void ScreenInArrow(Vector3 pos)
    {
        //画面外なら表示
        arrowRect.gameObject.SetActive(true);

        //Vector3 toTarget = target.position - uiCamera.transform.position;
        //float distance = toTarget.magnitude;
        //float scale = Mathf.Clamp(rateScale / distance, scaleMin, scaleMax);

        arrowRect.rotation = Quaternion.Euler(0f, 0f, 0f);

        pos.y += 200f;  //少し上にずらす

        //位置変換
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            pos,
            null,
            out localPos
        );

        arrowRect.localPosition = localPos;
    }

    //画面外矢印表示
    void ScreenOutArrow(Vector3 pos)
    {
        //画面外なら表示
        arrowRect.gameObject.SetActive(true);

        //Vector3 toTarget = target.position - uiCamera.transform.position;
        //float distance = toTarget.magnitude;
        //float scale = Mathf.Clamp(rateScale / distance, scaleMin, scaleMax);

        //画面端クランプ
        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) * 0.5f;
        Vector3 fromCenterToTarget = (pos - screenCenter).normalized;

        pos = screenCenter + fromCenterToTarget * ((Mathf.Min(Screen.width, Screen.height) * 0.5f) - edgeBuffer.magnitude);
        pos.x = Mathf.Clamp(pos.x, edgeBuffer.x, Screen.width - edgeBuffer.x);
        pos.y = Mathf.Clamp(pos.y, edgeBuffer.y, Screen.height - edgeBuffer.y);

        //角度計算と回転
        float angle = Mathf.Atan2(
            pos.y - 2.5f - (Screen.height / 2),
            pos.x - (Screen.width / 2)
        ) * Mathf.Rad2Deg;
        arrowRect.rotation = Quaternion.Euler(0, 0, angle + 90f);

        //位置変換
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            pos,
            null,
            out localPos
        );

        arrowRect.localPosition = localPos;
    }
}
