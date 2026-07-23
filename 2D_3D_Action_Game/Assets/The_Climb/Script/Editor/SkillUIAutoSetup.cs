using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// スキルUIの参照を自動設定するエディタツール
/// UI要素は手動で作成し、このツールで参照を自動設定します
/// </summary>
public class SkillUIAutoSetup : EditorWindow
{
    private GameObject skillUIPanel;

    [MenuItem("Tools/スキルUI自動設定")]
    public static void ShowWindow()
    {
        GetWindow<SkillUIAutoSetup>("スキルUI自動設定");
    }

    private void OnGUI()
    {
        GUILayout.Label("スキルUI完全自動セットアップ", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "このツールは、スキルUIを完全自動でセットアップします。\n\n" +
            "自動実行される内容:\n" +
            "1. UIキャンバスの作成(既存があれば使用)\n" +
            "2. SkillUIPanelの作成\n" +
            "3. スキルアイコンとクールタイムゲージの作成\n" +
            "4. SkillUIManagerコンポーネントの追加\n" +
            "5. 画像アセットの自動割り当て\n" +
            "6. すべての参照の自動設定\n\n" +
            "必要な準備:\n" +
            "- シーン内にPlayerAbilityManager等のスキルスクリプトが存在すること\n" +
            "- 画像アセットがAssets/Img/Image/Ability/に配置されていること",
            MessageType.Info
        );

        GUILayout.Space(20);

        if (GUILayout.Button("完全自動セットアップを開始", GUILayout.Height(50)))
        {
            FullAutoSetup();
        }

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "ボタンをクリックするだけで、すべてのUI要素が自動的に作成・設定されます。",
            MessageType.Info
        );
    }

    private void FullAutoSetup()
    {
        Debug.Log("=== スキルUI完全自動セットアップ開始 ===");

        // ステップ1: キャンバスを取得または作成
        Canvas canvas = GetOrCreateCanvas();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("エラー", "キャンバスの作成に失敗しました。", "OK");
            return;
        }

        // ステップ2: パネルを作成
        // 既存のパネルがあれば削除して作り直す
        Transform existingPanel = canvas.transform.Find("SkillUIPanel");
        if (existingPanel != null)
        {
            DestroyImmediate(existingPanel.gameObject);
        }

        GameObject panel = CreateSkillUIPanel(canvas.transform);
        if (panel == null)
        {
            EditorUtility.DisplayDialog("エラー", "パネルの作成に失敗しました。", "OK");
            return;
        }

        // ステップ3: UI要素を作成
        CreateUIElements(panel.transform);

        // ステップ4: SkillUIManagerコンポーネントを追加して設定
        skillUIPanel = panel;
        AutoSetup();

        Debug.Log("=== スキルUI完全自動セットアップ完了 ===");
    }

    private void AutoSetup()
    {
        Debug.Log("=== スキルUI自動設定開始 ===");

        // ステップ1: SkillUIManagerコンポーネントを追加
        Component uiManager = skillUIPanel.GetComponent("SkillUIManager");
        
        if (uiManager == null)
        {
            Debug.Log("SkillUIManagerコンポーネントを追加中...");
            
            // MonoBehaviourとして追加
            uiManager = AddSkillUIManagerComponent();
            
            if (uiManager == null)
            {
                bool continueSetup = EditorUtility.DisplayDialog(
                    "SkillUIManagerが見つかりません",
                    "SkillUIManagerコンポーネントを自動的に追加できませんでした。\n" +
                    "手動でSkillUIManagerコンポーネントをSkillUIPanelに追加してから、もう一度実行してください。\n\n" +
                    "または、このまま続行して参照設定のみを行いますか？\n" +
                    "(コンポーネントがないため、参照設定はスキップされます)",
                    "続行する", "キャンセル"
                );

                if (!continueSetup) return;
                
                Debug.LogWarning("SkillUIManagerコンポーネントなしで続行します");
            }
            else
            {
                Debug.Log("SkillUIManagerコンポーネントを追加しました");
            }
        }
        else
        {
            Debug.Log("SkillUIManagerコンポーネントは既に存在します");
        }

        if (uiManager != null)
        {
            SerializedObject so = new SerializedObject(uiManager);

            // ステップ2: UI要素への参照を設定
            Debug.Log("UI要素への参照を設定中...");
            SetupUIReferences(so);

            // ステップ3: 画像アセットを読み込んで設定
            Debug.Log("画像アセットを設定中...");
            SetupImageAssets(so);

            // ステップ4: スキルスクリプトへの参照を設定
            Debug.Log("スキルスクリプトへの参照を設定中...");
            SetupSkillReferences(so);

            so.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogError("SkillUIManagerコンポーネントがないため、参照設定をスキップしました。");
        }

        Debug.Log("=== スキルUI自動設定完了 ===");
        EditorUtility.DisplayDialog("完了", "スキルUIの自動設定が完了しました!", "OK");
    }

    private Component AddSkillUIManagerComponent()
    {
        // GameObjectに直接MonoBehaviourを追加
        // SkillUIManagerの完全修飾名を使用
        var gameObject = skillUIPanel;
        
        // Undo登録
        Undo.RecordObject(gameObject, "Add SkillUIManager");
        
        // MonoScriptを検索
        string[] guids = AssetDatabase.FindAssets("t:MonoScript SkillUIManager");
        if (guids.Length == 0)
        {
            Debug.LogError("SkillUIManager.csが見つかりません");
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        
        if (script == null)
        {
            Debug.LogError("SkillUIManagerスクリプトの読み込みに失敗しました");
            return null;
        }

        System.Type scriptType = script.GetClass();
        if (scriptType == null)
        {
            Debug.LogWarning("SkillUIManagerクラスの取得に失敗しました。手動でアタッチを試みます。");
            // クラスが取得できない場合は、ユーザーに手動アタッチを促すか、AddComponent<Component>で無理やり追加する(できない)
            // ここではnullを返して、呼び出し元で処理する
            return null;
        }

        Component component = gameObject.AddComponent(scriptType);
        return component;
    }


    private void SetupUIReferences(SerializedObject so)
    {
        Transform panelTransform = skillUIPanel.transform;

        // スキルアイコン
        Transform iconTransform = panelTransform.Find("SkillIconImage");
        if (iconTransform != null)
        {
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage != null)
            {
                so.FindProperty("skillIconImage").objectReferenceValue = iconImage;
                Debug.Log("✓ SkillIconImageを設定");
            }
        }
        else
        {
            Debug.LogWarning("SkillIconImageが見つかりません");
        }

        // Umedaクールタイムゲージ
        Transform umedaGauge = panelTransform.Find("UmedaCooldownGauge");
        if (umedaGauge != null)
        {
            Image gaugeImage = umedaGauge.GetComponent<Image>();
            if (gaugeImage != null)
            {
                so.FindProperty("umedaCooldownGauge").objectReferenceValue = gaugeImage;
                Debug.Log("✓ UmedaCooldownGaugeを設定");
            }
        }
        else
        {
            Debug.LogWarning("UmedaCooldownGaugeが見つかりません");
        }

        // Kitanoクールタイムゲージ
        Transform kitanoGauge = panelTransform.Find("KitanoCooldownGauge");
        if (kitanoGauge != null)
        {
            Image gaugeImage = kitanoGauge.GetComponent<Image>();
            if (gaugeImage != null)
            {
                so.FindProperty("kitanoCooldownGauge").objectReferenceValue = gaugeImage;
                Debug.Log("✓ KitanoCooldownGaugeを設定");
            }
        }
        else
        {
            Debug.LogWarning("KitanoCooldownGaugeが見つかりません");
        }

        // Nisiyamaクールタイムゲージ
        Transform nisiyamaGauge = panelTransform.Find("NisiyamaCooldownGauge");
        if (nisiyamaGauge != null)
        {
            Image gaugeImage = nisiyamaGauge.GetComponent<Image>();
            if (gaugeImage != null)
            {
                so.FindProperty("nisiyamaCooldownGauge").objectReferenceValue = gaugeImage;
                Debug.Log("✓ NisiyamaCooldownGaugeを設定");
            }
        }
        else
        {
            Debug.LogWarning("NisiyamaCooldownGaugeが見つかりません");
        }
    }

    private void SetupImageAssets(SerializedObject so)
    {
        // 画像アセットのパス
        string imagePath = "Assets/Img/Image/Ability/";

        Sprite umedaSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "IMG_3054.PNG");
        if (umedaSprite != null)
        {
            so.FindProperty("umedaSkillSprite").objectReferenceValue = umedaSprite;
            Debug.Log("✓ Umeda画像を設定");
        }
        else
        {
            Debug.LogWarning("Umeda画像が見つかりません: " + imagePath + "IMG_3054.PNG");
        }

        Sprite kitanoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "IMG_3056.PNG");
        if (kitanoSprite != null)
        {
            so.FindProperty("kitanoSkillSprite").objectReferenceValue = kitanoSprite;
            Debug.Log("✓ Kitano画像を設定");
        }
        else
        {
            Debug.LogWarning("Kitano画像が見つかりません: " + imagePath + "IMG_3056.PNG");
        }

        Sprite nisiyamaSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "IMG_3055.PNG");
        if (nisiyamaSprite != null)
        {
            so.FindProperty("nisiyamaSkillSprite").objectReferenceValue = nisiyamaSprite;
            Debug.Log("✓ Nisiyama画像を設定");
        }
        else
        {
            Debug.LogWarning("Nisiyama画像が見つかりません: " + imagePath + "IMG_3055.PNG");
        }
    }

    private void SetupSkillReferences(SerializedObject so)
    {
        // PlayerAbilityManager
        PlayerAbilityManager[] abilityManagers = FindObjectsOfType<PlayerAbilityManager>(true);
        if (abilityManagers.Length > 0)
        {
            so.FindProperty("abilityManager").objectReferenceValue = abilityManagers[0];
            Debug.Log("✓ PlayerAbilityManagerを設定");
        }
        else
        {
            Debug.LogWarning("PlayerAbilityManagerが見つかりません");
        }

        // SkillPlatformSpawner
        // Typeで検索する場合はResources.FindObjectsOfTypeAllを使うか、ループで探す必要があるが、
        // ここではPlayerFootPlatformSkillがアタッチされているオブジェクトを探す
        SkillPlatformSpawner[] spawners = FindObjectsOfType<SkillPlatformSpawner>(true);
        if (spawners.Length > 0)
        {
            so.FindProperty("skillPlatformSpawner").objectReferenceValue = spawners[0];
            Debug.Log("✓ SkillPlatformSpawnerを設定");
        }
        else
        {
            Debug.LogWarning("SkillPlatformSpawnerが見つかりません");
        }

        // GravityFlipManager
        GravityFlipManager[] gravityManagers = FindObjectsOfType<GravityFlipManager>(true);
        if (gravityManagers.Length > 0)
        {
            so.FindProperty("gravityFlipManager").objectReferenceValue = gravityManagers[0];
            Debug.Log("✓ GravityFlipManagerを設定");
        }
        else
        {
            Debug.LogWarning("GravityFlipManagerが見つかりません");
        }

        // TempDisableColliders (Kitano)
        TempDisableColliders[] tempColliders = FindObjectsOfType<TempDisableColliders>(true);
        if (tempColliders.Length > 0)
        {
            so.FindProperty("tempDisableColliders").objectReferenceValue = tempColliders[0];
            Debug.Log("✓ TempDisableCollidersを設定");
        }
        else
        {
            Debug.LogWarning("TempDisableCollidersが見つかりません");
        }
    }

    // === UI作成メソッド ===

    private Canvas GetOrCreateCanvas()
    {
        // 既存のキャンバスを検索
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        
        if (existingCanvas != null)
        {
            Debug.Log($"既存のキャンバスを使用します: {existingCanvas.name}");
            return existingCanvas;
        }

        // 新しいキャンバスを作成
        GameObject canvasObj = new GameObject("SkillUICanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();

        Debug.Log("新しいキャンバスを作成しました: SkillUICanvas");
        return canvas;
    }

    private GameObject CreateSkillUIPanel(Transform canvasTransform)
    {
        GameObject panel = new GameObject("SkillUIPanel");
        panel.transform.SetParent(canvasTransform, false);

        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        
        // 画面右下に配置 (ユーザー指定: Posx90 PosY5)
        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 0);
        rectTransform.pivot = new Vector2(1, 0);
        rectTransform.anchoredPosition = new Vector2(90, 5);
        rectTransform.sizeDelta = new Vector2(500, 300); // サイズ2倍 (250x150 -> 500x300)

        // 背景画像は削除（透明なコンテナとして機能させる）
        // Image bgImage = panel.AddComponent<Image>();
        // bgImage.color = new Color(0, 0, 0, 0.3f);
        // bgImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        // bgImage.type = Image.Type.Sliced;

        Debug.Log("SkillUIPanelを作成しました");
        return panel;
    }

    private void CreateUIElements(Transform panelTransform)
    {
        // スキルアイコン
        CreateSkillIcon(panelTransform);

        // クールタイムゲージ (色は黒半透明)
        Color gaugeColor = new Color(0f, 0f, 0f, 0.7f);
        // 全て中央に配置して重ねる (位置: 0, 0)
        CreateCooldownGauge(panelTransform, "UmedaCooldownGauge", Vector2.zero, gaugeColor);
        CreateCooldownGauge(panelTransform, "KitanoCooldownGauge", Vector2.zero, gaugeColor);
        CreateCooldownGauge(panelTransform, "NisiyamaCooldownGauge", Vector2.zero, gaugeColor);

        Debug.Log("UI要素を作成しました");
    }

    private void CreateSkillIcon(Transform parent)
    {
        GameObject icon = new GameObject("SkillIconImage");
        icon.transform.SetParent(parent, false);

        RectTransform rectTransform = icon.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero; // 中央
        rectTransform.sizeDelta = new Vector2(200, 200); // サイズ統一

        Image image = icon.AddComponent<Image>();
        image.color = Color.white;
        // アイコン用スプライト初期値
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        Debug.Log("SkillIconImageを作成しました");
    }

    private void CreateCooldownGauge(Transform parent, string name, Vector2 position, Color color)
    {
        GameObject gauge = new GameObject(name);
        gauge.transform.SetParent(parent, false);

        RectTransform rectTransform = gauge.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(200, 200); // サイズ統一

        Image image = gauge.AddComponent<Image>();
        // 重要: Filledタイプを機能させるためにスプライトを設定
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.color = color;
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Vertical;
        image.fillOrigin = (int)Image.OriginVertical.Bottom;
        image.fillAmount = 0f; // 初期状態は0
        image.raycastTarget = false; // 操作を阻害しないように

        Debug.Log($"{name}を作成しました");
    }
}
