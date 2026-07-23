#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Loading;
using System.Collections.Generic;

namespace EditorScript
{
    public class SceneLoaderSetup : EditorWindow
    {
        [MenuItem("Tools/Setup Scene Loader")]
        public static void SetupSceneLoader()
        {
            // 1. Check if SceneLoader already exists
            SceneLoader existingLoader = FindObjectOfType<SceneLoader>();
            if (existingLoader != null)
            {
                Debug.LogWarning("SceneLoader already exists in the scene.");
                Selection.activeGameObject = existingLoader.gameObject;
                return;
            }

            // 2. Create the main GameObject
            GameObject loaderGO = new GameObject("SceneLoader");
            SceneLoader sceneLoader = loaderGO.AddComponent<SceneLoader>();

            // 3. Create the UI Canvas
            GameObject canvasGO = new GameObject("SceneLoaderCanvas");
            canvasGO.transform.SetParent(loaderGO.transform);
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // Ensure it's on top
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // 4. Create the Background Image
            GameObject bgGO = new GameObject("Background");
            bgGO.transform.SetParent(canvasGO.transform);
            Image bgImage = bgGO.AddComponent<Image>();
            bgImage.color = Color.black;
            RectTransform bgRect = bgImage.rectTransform;
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // 5. Create the Slider
            GameObject sliderGO = new GameObject("ProgressSlider");
            sliderGO.transform.SetParent(canvasGO.transform);
            Slider slider = sliderGO.AddComponent<Slider>();
            RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.1f, 0.1f);
            sliderRect.anchorMax = new Vector2(0.9f, 0.15f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;

            // Slider Background
            GameObject sliderBgGO = new GameObject("Background");
            sliderBgGO.transform.SetParent(sliderGO.transform);
            Image sliderBgImage = sliderBgGO.AddComponent<Image>();
            sliderBgImage.color = new Color(0.2f, 0.2f, 0.2f);
            RectTransform sliderBgRect = sliderBgImage.rectTransform;
            sliderBgRect.anchorMin = Vector2.zero;
            sliderBgRect.anchorMax = Vector2.one;
            sliderBgRect.offsetMin = Vector2.zero;
            sliderBgRect.offsetMax = Vector2.zero;

            // Slider Fill Area
            GameObject fillAreaGO = new GameObject("Fill Area");
            fillAreaGO.transform.SetParent(sliderGO.transform);
            RectTransform fillAreaRect = fillAreaGO.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(5, 0);
            fillAreaRect.offsetMax = new Vector2(-5, 0);

            // Slider Fill
            GameObject fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(fillAreaGO.transform);
            Image fillImage = fillGO.AddComponent<Image>();
            fillImage.color = Color.green;
            RectTransform fillRect = fillImage.rectTransform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            slider.targetGraphic = sliderBgImage;
            slider.fillRect = fillRect;
            slider.direction = Slider.Direction.LeftToRight;

            // 6. Setup LoadingScreenUI component
            LoadingScreenUI uiScript = canvasGO.AddComponent<LoadingScreenUI>();
            
            // Use SerializedObject to assign private fields
            SerializedObject serializedUI = new SerializedObject(uiScript);
            serializedUI.FindProperty("backgroundImage").objectReferenceValue = bgImage;
            serializedUI.FindProperty("progressSlider").objectReferenceValue = slider;
            serializedUI.ApplyModifiedProperties();

            // 7. Link UI to SceneLoader
            SerializedObject serializedLoader = new SerializedObject(sceneLoader);
            serializedLoader.FindProperty("loadingScreenUI").objectReferenceValue = uiScript;
            serializedLoader.ApplyModifiedProperties();

            // 8. Hide the UI initially (optional, but good for editing)
            // uiScript.SetActive(true); // Keep it visible for editing
            
            Debug.Log("Scene Loader setup complete!");
            Selection.activeGameObject = loaderGO;
        }
    }
}
#endif
