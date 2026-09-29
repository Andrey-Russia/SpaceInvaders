using System;
using TMPro;
using SpaceInvaders.UI.Gameplay;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpaceInvaders.Editor
{
    public static class UiPrefabSetup
    {
        private const string GameHudPath =
            "Assets/_Project/Prefabs/UI/GameHUD.prefab";

        private const string ButtonPath =
            "Assets/_Project/Prefabs/UI/Button.prefab";

        [MenuItem("Tools/Space Invaders/Apply UI prefab setup")]
        public static void Apply()
        {
            GameObject buttonPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(ButtonPath);

            if (buttonPrefab == null)
            {
                Debug.LogError($"Button prefab was not found: {ButtonPath}");
                return;
            }

            GameObject root = PrefabUtility.LoadPrefabContents(GameHudPath);

            try
            {
                SetupGameHud(root, buttonPrefab);
                PrefabUtility.SaveAsPrefabAsset(root, GameHudPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Game HUD pause panel and 1980x1080 scaling are configured.");
        }

        private static void SetupGameHud(
            GameObject root,
            GameObject buttonPrefab)
        {
            GameHUDController controller =
                root.GetComponent<GameHUDController>();

            CanvasScaler canvasScaler = root.GetComponent<CanvasScaler>();

            if (controller == null || canvasScaler == null)
            {
                throw new InvalidOperationException(
                    "GameHUD prefab must contain GameHUDController and CanvasScaler.");
            }

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1980f, 1080f);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;

            Transform safeArea = FindDeepChild(root.transform, "SafeArea");

            if (safeArea == null)
            {
                throw new InvalidOperationException(
                    "SafeArea was not found inside GameHUD prefab.");
            }

            TMP_Text textTemplate = root.GetComponentInChildren<TMP_Text>(true);

            GameObject pausePanel = GetOrCreatePanel(safeArea, "PausePanel");
            RectTransform pauseRect = pausePanel.GetComponent<RectTransform>();
            Stretch(pauseRect);
            pausePanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.78f);
            pausePanel.transform.SetAsLastSibling();

            GameObject window = GetOrCreatePanel(pausePanel.transform, "Window");
            RectTransform windowRect = window.GetComponent<RectTransform>();
            Center(windowRect, new Vector2(760f, 760f), Vector2.zero);
            window.GetComponent<Image>().color = new Color(0.035f, 0.075f, 0.16f, 0.98f);

            TMP_Text title = GetOrCreateText(window.transform, "Title", textTemplate);
            title.text = "ПАУЗА";
            title.fontSize = 72f;
            title.alignment = TextAlignmentOptions.Center;
            Center(
                title.rectTransform,
                new Vector2(620f, 110f),
                new Vector2(0f, 270f));

            CreateButton(
                window.transform,
                buttonPrefab,
                "ResumeButton",
                "ПРОДОЛЖИТЬ",
                new Vector2(0f, 95f),
                controller.ResumeGame);

            CreateButton(
                window.transform,
                buttonPrefab,
                "RestartButton",
                "ЗАНОВО",
                new Vector2(0f, -85f),
                controller.RestartGame);

            CreateButton(
                window.transform,
                buttonPrefab,
                "MainMenuButton",
                "ГЛАВНОЕ МЕНЮ",
                new Vector2(0f, -265f),
                controller.ReturnToMainMenu);

            GameObject closeButton = CreateButton(
                window.transform,
                buttonPrefab,
                "CloseButton",
                "X",
                Vector2.zero,
                controller.ResumeGame);

            RectTransform closeRect =
                closeButton.GetComponent<RectTransform>();

            closeRect.anchorMin = Vector2.one;
            closeRect.anchorMax = Vector2.one;
            closeRect.pivot = Vector2.one;
            closeRect.sizeDelta = new Vector2(110f, 110f);
            closeRect.anchoredPosition = new Vector2(-20f, -20f);

            GameObject pauseButton = CreateButton(
                safeArea,
                buttonPrefab,
                "PauseButton",
                "Ⅱ",
                Vector2.zero,
                controller.PauseGame);

            RectTransform pauseButtonRect =
                pauseButton.GetComponent<RectTransform>();

            pauseButtonRect.anchorMin = Vector2.one;
            pauseButtonRect.anchorMax = Vector2.one;
            pauseButtonRect.pivot = Vector2.one;
            pauseButtonRect.sizeDelta = new Vector2(130f, 130f);
            pauseButtonRect.anchoredPosition = new Vector2(-40f, -40f);
            pauseButton.transform.SetSiblingIndex(
                Mathf.Max(0, pausePanel.transform.GetSiblingIndex() - 1));

            SerializedObject serializedController =
                new SerializedObject(controller);

            SerializedProperty pausePanelProperty =
                serializedController.FindProperty("_pausePanel");

            pausePanelProperty.objectReferenceValue = pausePanel;
            serializedController.ApplyModifiedPropertiesWithoutUndo();

            pausePanel.SetActive(false);

            EditorUtility.SetDirty(controller);
            EditorUtility.SetDirty(root);
        }

        private static GameObject GetOrCreatePanel(
            Transform parent,
            string objectName)
        {
            Transform existing = parent.Find(objectName);

            GameObject panel = existing != null
                ? existing.gameObject
                : new GameObject(
                    objectName,
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Image));

            panel.transform.SetParent(parent, false);

            if (panel.GetComponent<CanvasRenderer>() == null)
            {
                panel.AddComponent<CanvasRenderer>();
            }

            if (panel.GetComponent<Image>() == null)
            {
                panel.AddComponent<Image>();
            }

            return panel;
        }

        private static TMP_Text GetOrCreateText(
            Transform parent,
            string objectName,
            TMP_Text template)
        {
            Transform existing = parent.Find(objectName);
            TextMeshProUGUI text;

            if (existing == null)
            {
                GameObject textObject = new GameObject(
                    objectName,
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(TextMeshProUGUI));

                textObject.transform.SetParent(parent, false);
                text = textObject.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                text = existing.GetComponent<TextMeshProUGUI>();

                if (text == null)
                {
                    text = existing.gameObject.AddComponent<TextMeshProUGUI>();
                }
            }

            if (template != null && template.font != null)
            {
                text.font = template.font;
            }

            text.color = Color.white;
            text.raycastTarget = false;
            text.enableAutoSizing = true;
            text.fontSizeMin = 24f;
            text.fontSizeMax = 72f;

            return text;
        }

        private static GameObject CreateButton(
            Transform parent,
            GameObject buttonPrefab,
            string objectName,
            string label,
            Vector2 position,
            UnityAction action)
        {
            Transform existing = parent.Find(objectName);

            GameObject buttonObject = existing != null
                ? existing.gameObject
                : (GameObject)PrefabUtility.InstantiatePrefab(
                    buttonPrefab,
                    parent);

            buttonObject.name = objectName;

            RectTransform rectTransform =
                buttonObject.GetComponent<RectTransform>();

            Center(
                rectTransform,
                new Vector2(520f, 130f),
                position);

            TMP_Text labelText =
                buttonObject.GetComponentInChildren<TMP_Text>(true);

            if (labelText != null)
            {
                labelText.text = label;
                labelText.enableAutoSizing = true;
                labelText.fontSizeMin = 22f;
                labelText.fontSizeMax = 52f;
            }

            Button button = buttonObject.GetComponent<Button>();

            while (button.onClick.GetPersistentEventCount() > 0)
            {
                UnityEventTools.RemovePersistentListener(
                    button.onClick,
                    0);
            }

            UnityEventTools.AddPersistentListener(button.onClick, action);

            return buttonObject;
        }

        private static Transform FindDeepChild(
            Transform parent,
            string objectName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == objectName)
                {
                    return child;
                }

                Transform result = FindDeepChild(child, objectName);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private static void Center(
            RectTransform rectTransform,
            Vector2 size,
            Vector2 position)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = position;
        }
    }
}
