using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageNode : MonoBehaviour
{
    [Header("Stage")]
    public int stageId;

#if UNITY_EDITOR
    [Header("Editor専用")]
    public SceneAsset sceneAsset;
#endif

    [Header("Visual")]
    public Renderer nodeRenderer;
    public Material lockedMat;
    public Material availableMat;
    public Material clearedMat;

    [Header("UI")]
    public TextMeshProUGUI stageNameText;
    public GameObject promptUI;
    public Vector3 uiOffset = new Vector3(0, 2, 0);

    [Header("Preview Canvas")]
    public Image stageImage;
    public RawImage stageRawImage;

    [Header("Unknown")]
    public Sprite unknownSprite;
    public Texture unknownTexture;

    [Header("External")]
    public StageRandomizer stageRandomizer;
    public StageSelectManager stageSelectManager;

    [Header("Detection")]
    public float detectionRange = 5f; // プレイヤーを検知する距離（インスペクターで編集可能）

    private bool playerNearby;
    public bool isInteractable;
    private Transform playerTransform; // プレイヤーのTransformをキャッシュ

    // ===============================
    // Unity
    // ===============================
    private void Awake()
    {
        if (nodeRenderer == null)
            nodeRenderer = GetComponentInChildren<Renderer>(true);

        SetLocked();

        if (promptUI)
            promptUI.SetActive(false);

        HideImage();
    }

    private void Update()
    {
        if (promptUI)
            promptUI.transform.position = transform.position + uiOffset;

        // ロード中は以下の処理をスキップする
        if (System.Loading.SceneLoader.IsLoading)
            return;

        // 距離ベースのプレイヤー検知
        CheckPlayerDistance();

        if (isInteractable && playerNearby && Input.GetKeyDown(KeyCode.Space))
        {
            if (stageSelectManager != null)
                stageSelectManager.OnStageSelected(stageId);
        }
    }

    // プレイヤーとの距離をチェックしてUIの状態を更新する
    private void CheckPlayerDistance()
    {
        // プレイヤーのTransformを探す（一度だけ）
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        bool isInside = dist <= detectionRange;

        if (isInside && !playerNearby)
        {
            OnPlayerEnter();
        }
        else if (!isInside && playerNearby)
        {
            OnPlayerExit();
        }
    }

    // ===============================
    // 状態制御
    // ===============================
    public void SetLocked()
    {
        isInteractable = false;
        if (nodeRenderer && lockedMat)
            nodeRenderer.material = lockedMat;
    }

    public void SetAvailable()
    {
        isInteractable = true;
        if (nodeRenderer && availableMat)
            nodeRenderer.material = availableMat;
    }

    public void SetCleared()
    {
        isInteractable = true; // クリア後も再度入れるように変更
        if (nodeRenderer && clearedMat)
            nodeRenderer.material = clearedMat;
    }

    // ===============================
    // Trigger
    // ===============================
    // ===============================
    // Proximity Events (Internal)
    // ===============================
    private void OnPlayerEnter()
    {
        playerNearby = true;

        if (promptUI)
            promptUI.SetActive(true);

        if (stageNameText)
        {
            if (!isInteractable)
                stageNameText.text = "???";
            else if (stageRandomizer && stageId - 1 < stageRandomizer.StageName.Length)
                stageNameText.text = stageRandomizer.StageName[stageId - 1];
        }

        ShowImage();
    }

    private void OnPlayerExit()
    {
        playerNearby = false;

        if (promptUI)
            promptUI.SetActive(false);

        if (stageNameText)
            stageNameText.text = "";

        HideImage();
    }

    // ===============================
    // 画像制御
    // ===============================
    private void ShowImage()
    {
        if (!isInteractable)
        {
            if (stageImage && unknownSprite)
            {
                stageImage.sprite = unknownSprite;
                stageImage.gameObject.SetActive(true);
            }

            if (stageRawImage && unknownTexture)
            {
                stageRawImage.texture = unknownTexture;
                stageRawImage.gameObject.SetActive(true);
            }
            return;
        }

        if (stageRandomizer == null) return;

        Sprite sprite = stageRandomizer.GetStagePreviewSprite(stageId);
        if (sprite)
        {
            stageImage.sprite = sprite;
            stageImage.gameObject.SetActive(true);
        }
    }

    private void HideImage()
    {
        if (stageImage)
            stageImage.gameObject.SetActive(false);

        if (stageRawImage)
            stageRawImage.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    // ===============================
    // Editor補助
    // ===============================
    public void RefreshSceneName()
    {
        if (sceneAsset != null)
        {
            string _ = sceneAsset.name;
        }
    }

    // エディタ上で検知範囲を視覚化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
#endif
}