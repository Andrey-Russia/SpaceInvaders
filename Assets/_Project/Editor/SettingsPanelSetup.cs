using System;
using SpaceInvaders.UI;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpaceInvaders.Editor
{
    [InitializeOnLoad]
    public static class SettingsPanelSetup
    {
        private const string SettingsPath =
            "Assets/_Project/Prefabs/UI/SettingsPanel.prefab";

        private const string MainMenuPath =
            "Assets/_Project/Prefabs/UI/MainMenuUI.prefab";

        private const string ButtonPath =
            "Assets/_Project/Prefabs/UI/Button.prefab";

        private static string CompletionKey =>
            "SpaceInvaders.SettingsPanelSetup.20260922." + Application.dataPath;

        static SettingsPanelSetup()
        {
            EditorApplication.delayCall += ApplyOnce;
        }

        [MenuItem("Tools/Space Invaders/Configure settings panel")]
        public static void Apply()
        {
            GameObject buttonPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(ButtonPath);

            if (buttonPrefab == null)
            {
                throw new InvalidOperationException(
                    "Button prefab is missing: " + ButtonPath);
            }

            ConfigureSettingsPrefab(buttonPrefab);
            FixMainMenuOverride();

            AssetDatabase.SaveAssets();
            Debug.Log("Settings panel, Back button and VK link are configured.");
        }

        private static void ApplyOnce()
        {
            if (EditorPrefs.GetBool(CompletionKey, false))
            {
                return;
            }

            if (EditorApplication.isCompiling || EditorApplication.isUpdating ||
                EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.delayCall += ApplyOnce;
                return;
            }

            try
            {
                Apply();
                EditorPrefs.SetBool(CompletionKey, true);
            }
            catch (Exception exception)
            {
                Debug.LogError("Settings panel setup failed: " + exception);
            }
        }

        private static void ConfigureSettingsPrefab(GameObject buttonPrefab)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(SettingsPath);

            try
            {
                SettingsPanelController controller =
                    root.GetComponent<SettingsPanelController>();

                if (controller == null)
                {
                    throw new InvalidOperationException(
                        "SettingsPanelController is missing from SettingsPanel prefab.");
                }

                Image backdrop = root.GetComponent<Image>();
                backdrop.color = new Color(0f, 0f, 0f, 0.78f);
                backdrop.raycastTarget = true;

                GameObject window = GetOrCreateWindow(root.transform);
                window.transform.SetAsFirstSibling();
                Center(window.GetComponent<RectTransform>(),
                    new Vector2(900f, 820f), Vector2.zero);
                window.GetComponent<Image>().color =
                    new Color(0.067f, 0.114f, 0.184f, 0.98f);

                Transform titleTransform = root.transform.Find("Title");
                TMP_Text title = titleTransform != null
                    ? titleTransform.GetComponent<TMP_Text>()
                    : null;

                if (title == null)
                {
                    throw new InvalidOperationException(
                        "Settings title is missing.");
                }

                Center(title.rectTransform,
                    new Vector2(700f, 110f), new Vector2(0f, 290f));
                title.text = "НАСТРОЙКИ";
                title.alignment = TextAlignmentOptions.Center;
                title.fontSize = 72f;
                title.color = Color.white;

                ConfigureVolumeRow(root.transform, "MusicSettings",
                    "MusicLabel", "MusicSlider", 120f);
                ConfigureVolumeRow(root.transform, "SfxSettings",
                    "SfxLabel", "SfxSlider", -30f);

                Transform backTransform = root.transform.Find("BackButton") ??
                    root.transform.Find("Button");

                if (backTransform == null)
                {
                    throw new InvalidOperationException(
                        "Existing Back button is missing from SettingsPanel prefab.");
                }

                Button backButton = backTransform.GetComponent<Button>();
                backButton.name = "BackButton";
                Center(backButton.GetComponent<RectTransform>(),
                    new Vector2(440f, 100f), new Vector2(0f, -335f));
                SetLabel(backButton, "НАЗАД", title);
                SetAction(backButton, controller.Close);

                Button vkButton = GetOrCreateButton(
                    root.transform, buttonPrefab, "VkCommunityButton");
                Center(vkButton.GetComponent<RectTransform>(),
                    new Vector2(520f, 100f), new Vector2(0f, -205f));
                SetLabel(vkButton, "СООБЩЕСТВО VK", title);
                SetAction(vkButton, controller.OpenVkCommunity);

                PrefabUtility.SaveAsPrefabAsset(root, SettingsPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void FixMainMenuOverride()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(MainMenuPath);

            try
            {
                SettingsPanelController controller =
                    root.GetComponentInChildren<SettingsPanelController>(true);

                if (controller == null)
                {
                    throw new InvalidOperationException(
                        "SettingsPanel is missing from MainMenuUI prefab.");
                }

                RectTransform panelRect =
                    controller.GetComponent<RectTransform>();
                Stretch(panelRect);

                Transform backTransform =
                    controller.transform.Find("BackButton") ??
                    controller.transform.Find("Close") ??
                    controller.transform.Find("Button");

                if (backTransform == null)
                {
                    throw new InvalidOperationException(
                        "Back button is missing from nested SettingsPanel.");
                }

                Button backButton = backTransform.GetComponent<Button>();
                TMP_Text title = controller.transform.Find("Title")
                    .GetComponent<TMP_Text>();

                backButton.name = "BackButton";
                Center(backButton.GetComponent<RectTransform>(),
                    new Vector2(440f, 100f), new Vector2(0f, -335f));
                SetLabel(backButton, "НАЗАД", title);
                SetAction(backButton, controller.Close);

                PrefabUtility.SaveAsPrefabAsset(root, MainMenuPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void ConfigureVolumeRow(
            Transform root,
            string rowName,
            string labelName,
            string sliderName,
            float y)
        {
            Transform row = root.Find(rowName);
            Transform label = row?.Find(labelName);
            Transform sliderHolder = row?.Find(sliderName);

            if (row == null || label == null || sliderHolder == null)
            {
                throw new InvalidOperationException(
                    "Settings volume row is incomplete: " + rowName);
            }

            Center(row.GetComponent<RectTransform>(),
                new Vector2(740f, 110f), new Vector2(0f, y));
            Center(label.GetComponent<RectTransform>(),
                new Vector2(220f, 90f), new Vector2(-240f, 0f));
            Center(sliderHolder.GetComponent<RectTransform>(),
                new Vector2(410f, 90f), new Vector2(135f, 0f));

            Slider slider = sliderHolder.GetComponentInChildren<Slider>(true);

            if (slider == null)
            {
                throw new InvalidOperationException(
                    "Slider is missing from " + rowName);
            }

            Stretch(slider.GetComponent<RectTransform>());

            TMP_Text labelText = label.GetComponentInChildren<TMP_Text>(true);
            if (labelText != null)
            {
                labelText.alignment = TextAlignmentOptions.Center;
                labelText.fontSize = 44f;
                labelText.color = Color.white;
                Stretch(labelText.rectTransform);
            }
        }

        private static GameObject GetOrCreateWindow(Transform parent)
        {
            Transform existing = parent.Find("Window");
            if (existing != null)
            {
                return existing.gameObject;
            }

            GameObject window = new GameObject("Window",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            window.transform.SetParent(parent, false);
            return window;
        }

        private static Button GetOrCreateButton(
            Transform parent, GameObject prefab, string name)
        {
            Transform existing = parent.Find(name);
            GameObject buttonObject = existing != null
                ? existing.gameObject
                : (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);

            buttonObject.name = name;
            return buttonObject.GetComponent<Button>();
        }

        private static void SetLabel(Button button, string label, TMP_Text template)
        {
            TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);

            if (text == null)
            {
                GameObject labelObject = new GameObject("Label",
                    typeof(RectTransform), typeof(CanvasRenderer),
                    typeof(TextMeshProUGUI));
                labelObject.transform.SetParent(button.transform, false);
                text = labelObject.GetComponent<TMP_Text>();
            }

            text.font = template.font;
            text.text = label;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 46f;
            text.enableAutoSizing = true;
            text.fontSizeMin = 30f;
            text.fontSizeMax = 46f;
            text.color = Color.white;
            text.raycastTarget = false;
            Stretch(text.rectTransform);
        }

        private static void SetAction(Button button, UnityAction action)
        {
            while (button.onClick.GetPersistentEventCount() > 0)
            {
                UnityEventTools.RemovePersistentListener(button.onClick, 0);
            }

            UnityEventTools.AddPersistentListener(button.onClick, action);
        }

        private static void Center(
            RectTransform rect, Vector2 size, Vector2 position)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
