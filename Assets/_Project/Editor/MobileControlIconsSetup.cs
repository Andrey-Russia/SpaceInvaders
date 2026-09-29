using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceInvaders.Editor
{
    [InitializeOnLoad]
    public static class MobileControlIconsSetup
    {
        private const string ArtPath = "Assets/_Project/Art/UI";
        private const string PrefabPath =
            "Assets/_Project/Prefabs/Gameplay/MobileControls.prefab";

        private static string CompletionKey =>
            "SpaceInvaders.MobileControlIconsSetup.20260922." +
            Application.dataPath;

        private static readonly string[] LeftPixels =
        {
            "................",
            "................",
            "......##........",
            ".....###........",
            "....####........",
            "...###########..",
            "..############..",
            ".#############..",
            ".#############..",
            "..############..",
            "...###########..",
            "....####........",
            ".....###........",
            "......##........",
            "................",
            "................"
        };

        private static readonly string[] FirePixels =
        {
            "................",
            ".......##.......",
            ".......##.......",
            ".......##.......",
            ".......##.......",
            ".......##.......",
            ".......##.......",
            "................",
            "................",
            ".......##.......",
            ".......##.......",
            ".....######.....",
            "...##########...",
            "...##########...",
            "................",
            "................"
        };

        static MobileControlIconsSetup()
        {
            EditorApplication.delayCall += ApplyOnce;
        }

        [MenuItem("Tools/Space Invaders/Generate mobile control icons")]
        public static void Apply()
        {
            Sprite left = CreateSprite("MobileControlLeft", LeftPixels);
            Sprite right = CreateSprite("MobileControlRight",
                Mirror(LeftPixels));
            Sprite fire = CreateSprite("MobileControlFire", FirePixels);

            GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
            try
            {
                ConfigureButton(root.transform, "LeftControl", left);
                ConfigureButton(root.transform, "RightControl", right);
                ConfigureButton(root.transform, "FireButton", fire);
                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Mobile control icons created and assigned.");
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
                Debug.LogError("Mobile control icon setup failed: " + exception);
            }
        }

        private static Sprite CreateSprite(string name, string[] pixels)
        {
            string path = ArtPath + "/" + name + ".png";
            if (!File.Exists(path))
            {
                Texture2D texture = new Texture2D(16, 16,
                    TextureFormat.RGBA32, false);
                texture.filterMode = FilterMode.Point;

                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        texture.SetPixel(x, 15 - y,
                            pixels[y][x] == '#' ? Color.white : Color.clear);
                    }
                }

                texture.Apply();
                File.WriteAllBytes(path, texture.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(texture);
            }

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException("Cannot import " + path);
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.spritePixelsPerUnit = 16f;
            importer.SaveAndReimport();

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                throw new InvalidOperationException("Sprite is missing: " + path);
            }

            return sprite;
        }

        private static void ConfigureButton(
            Transform root, string buttonName, Sprite sprite)
        {
            Transform button = root.Find(buttonName);
            if (button == null)
            {
                throw new InvalidOperationException(
                    "Mobile control button is missing: " + buttonName);
            }

            Image background = button.GetComponent<Image>();
            background.color = new Color(0.06f, 0.1f, 0.18f, 0.8f);
            background.raycastTarget = true;

            Transform iconTransform = button.Find("Icon");
            if (iconTransform == null)
            {
                GameObject iconObject = new GameObject("Icon",
                    typeof(RectTransform), typeof(CanvasRenderer),
                    typeof(Image));
                iconObject.transform.SetParent(button, false);
                iconTransform = iconObject.transform;
            }

            RectTransform rect = iconTransform.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(108f, 108f);

            Image image = iconTransform.GetComponent<Image>();
            image.sprite = sprite;
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
        }

        private static string[] Mirror(string[] pixels)
        {
            string[] result = new string[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                char[] characters = pixels[i].ToCharArray();
                Array.Reverse(characters);
                result[i] = new string(characters);
            }

            return result;
        }
    }
}
