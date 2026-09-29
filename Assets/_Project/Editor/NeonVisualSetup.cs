using System;
using System.Collections.Generic;
using System.Linq;
using SpaceInvaders.Gameplay.Effects;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceInvaders.Editor
{
    [InitializeOnLoad]
    public static class NeonVisualSetup
    {
        private const string CompletionKey =
            "SpaceInvaders.NeonVisualSetup.20260927.v1";

        private const string ButtonSpritePath =
            "Assets/_Project/Art/UI/NeonKit/UI_Button_9Slice.png";
        private const string PanelSpritePath =
            "Assets/_Project/Art/UI/NeonKit/UI_Panel_9Slice.png";
        private const string IconButtonSpritePath =
            "Assets/_Project/Art/UI/NeonKit/UI_IconButton_9Slice.png";
        private const string TitleSpritePath =
            "Assets/_Project/Art/UI/NeonKit/UI_TitleFrame.png";
        private const string HealthSpritePath =
            "Assets/_Project/Art/UI/NeonKit/UI_HealthIcon.png";
        private const string IconsAtlasPath =
            "Assets/_Project/Art/UI/NeonKit/UI_IconsAtlas.png";
        private const string ExplosionAtlasPath =
            "Assets/_Project/Art/Effects/Explosions_Neon_Atlas.png";
        private const string ThrusterAtlasPath =
            "Assets/_Project/Art/Effects/PlayerThruster_Neon_4Frame.png";

        private const string ButtonPrefabPath =
            "Assets/_Project/Prefabs/UI/Button.prefab";
        private const string SettingsPrefabPath =
            "Assets/_Project/Prefabs/UI/SettingsPanel.prefab";
        private const string ResultPrefabPath =
            "Assets/_Project/Prefabs/UI/ResultPanel.prefab";
        private const string GameHudPrefabPath =
            "Assets/_Project/Prefabs/UI/GameHUD.prefab";
        private const string HealthIconPrefabPath =
            "Assets/_Project/Prefabs/Gameplay/PlayerPrefabs/HealthIcon.prefab";
        private const string MobileControlsPrefabPath =
            "Assets/_Project/Prefabs/Gameplay/MobileControls.prefab";
        private const string PlayerPrefabPath =
            "Assets/_Project/Prefabs/Gameplay/PlayerPrefabs/Player.prefab";
        private const string EnemyExplosionPrefabPath =
            "Assets/_Project/Art/Effects/EnemyExplosion.prefab";

        private const string EffectsAnimationFolder =
            "Assets/_Project/Art/Animations/Effects";
        private const string EnemyExplosionClipPath =
            EffectsAnimationFolder + "/EnemyExplosion_Neon.anim";
        private const string EnemyExplosionControllerPath =
            EffectsAnimationFolder + "/EnemyExplosion_Neon.controller";
        private const string PlayerThrusterClipPath =
            EffectsAnimationFolder + "/PlayerThruster_Neon.anim";
        private const string PlayerThrusterControllerPath =
            EffectsAnimationFolder + "/PlayerThruster_Neon.controller";

        static NeonVisualSetup()
        {
            EditorApplication.delayCall += ApplyOnce;
        }

        [MenuItem("Tools/Space Invaders/Apply neon visual setup")]
        public static void Apply()
        {
            Sprite buttonSprite = LoadFirstSprite(ButtonSpritePath);
            Sprite panelSprite = LoadFirstSprite(PanelSpritePath);
            Sprite iconButtonSprite = LoadFirstSprite(IconButtonSpritePath);
            Sprite titleSprite = LoadFirstSprite(TitleSpritePath);
            Sprite healthSprite = LoadFirstSprite(HealthSpritePath);
            Sprite[] icons = LoadSpritesInReadingOrder(IconsAtlasPath);

            ConfigureButtonPrefab(buttonSprite);
            ConfigureSettingsPanel(panelSprite, titleSprite);
            ConfigureResultPanel(panelSprite);
            ConfigureGameHud(panelSprite);
            ConfigureHealthIcon(healthSprite);
            ConfigureMobileControls(iconButtonSprite, icons);
            ConfigureEnemyExplosion();
            ConfigurePlayerThruster();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Neon visual setup applied successfully.");
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
                Debug.LogError("Neon visual setup failed: " + exception);
            }
        }

        private static void ConfigureButtonPrefab(Sprite sprite)
        {
            EditPrefab(ButtonPrefabPath, root =>
            {
                Image image = root.GetComponent<Image>();
                AssignImage(image, sprite);
            });
        }

        private static void ConfigureSettingsPanel(
            Sprite panelSprite,
            Sprite titleSprite)
        {
            EditPrefab(SettingsPrefabPath, root =>
            {
                Transform window = FindDeepChild(root.transform, "Window");
                if (window != null)
                {
                    AssignImage(window.GetComponent<Image>(), panelSprite);
                }

                Transform titleFrame = FindDeepChild(
                    root.transform,
                    "UI_TitleFrame_0");

                if (titleFrame != null)
                {
                    AssignImage(titleFrame.GetComponent<Image>(), titleSprite);
                }
            });
        }

        private static void ConfigureResultPanel(Sprite panelSprite)
        {
            EditPrefab(ResultPrefabPath, root =>
            {
                Transform window = FindDeepChild(root.transform, "Window") ??
                    FindDeepChild(root.transform, "Panel");

                if (window != null)
                {
                    AssignImage(window.GetComponent<Image>(), panelSprite);
                }
            });
        }

        private static void ConfigureGameHud(Sprite panelSprite)
        {
            EditPrefab(GameHudPrefabPath, root =>
            {
                Transform pausePanel = FindDeepChild(root.transform, "PausePanel");
                Transform window = pausePanel == null
                    ? null
                    : FindDeepChild(pausePanel, "Window") ??
                      FindDeepChild(pausePanel, "Panel");

                if (window != null)
                {
                    AssignImage(window.GetComponent<Image>(), panelSprite);
                }
            });
        }

        private static void ConfigureHealthIcon(Sprite healthSprite)
        {
            EditPrefab(HealthIconPrefabPath, root =>
            {
                Image image = root.GetComponent<Image>();
                AssignImage(image, healthSprite, false);
                image.preserveAspect = true;
            });
        }

        private static void ConfigureMobileControls(
            Sprite backgroundSprite,
            IReadOnlyList<Sprite> icons)
        {
            if (icons.Count < 12)
            {
                throw new InvalidOperationException(
                    "UI icon atlas must contain at least 12 sliced sprites.");
            }

            EditPrefab(MobileControlsPrefabPath, root =>
            {
                ConfigureMobileButton(root.transform, "LeftControl",
                    backgroundSprite, icons[8]);
                ConfigureMobileButton(root.transform, "RightControl",
                    backgroundSprite, icons[9]);
                ConfigureMobileButton(root.transform, "FireButton",
                    backgroundSprite, icons[10]);
            });
        }

        private static void ConfigureMobileButton(
            Transform root,
            string buttonName,
            Sprite backgroundSprite,
            Sprite iconSprite)
        {
            Transform button = FindDeepChild(root, buttonName);
            if (button == null)
            {
                throw new InvalidOperationException(
                    "Mobile control is missing: " + buttonName);
            }

            AssignImage(button.GetComponent<Image>(), backgroundSprite);

            Transform icon = FindDeepChild(button, "Icon");
            if (icon == null)
            {
                throw new InvalidOperationException(
                    "Mobile control icon is missing: " + buttonName);
            }

            Image iconImage = icon.GetComponent<Image>();
            AssignImage(iconImage, iconSprite, false);
            iconImage.preserveAspect = true;
        }

        private static void ConfigureEnemyExplosion()
        {
            Sprite[] frames = LoadSpritesInReadingOrder(ExplosionAtlasPath)
                .Take(5)
                .ToArray();

            if (frames.Length < 5)
            {
                throw new InvalidOperationException(
                    "Explosion atlas must contain five enemy frames.");
            }

            AnimationClip clip = GetOrCreateClip(
                EnemyExplosionClipPath,
                frames,
                12f,
                false);

            AnimatorController controller = GetOrCreateController(
                EnemyExplosionControllerPath,
                clip);

            EditPrefab(EnemyExplosionPrefabPath, root =>
            {
                SpriteRenderer renderer = root.GetComponent<SpriteRenderer>() ??
                    root.AddComponent<SpriteRenderer>();
                renderer.sprite = frames[0];

                Animator animator = root.GetComponent<Animator>() ??
                    root.AddComponent<Animator>();
                animator.runtimeAnimatorController = controller;

                EnemyExplosion explosion = root.GetComponent<EnemyExplosion>();
                if (explosion != null)
                {
                    SerializedObject serializedExplosion =
                        new SerializedObject(explosion);
                    serializedExplosion.FindProperty("_duration").floatValue =
                        frames.Length / 12f;
                    serializedExplosion.ApplyModifiedPropertiesWithoutUndo();
                }
            });
        }

        private static void ConfigurePlayerThruster()
        {
            Sprite[] frames = LoadSpritesLeftToRight(ThrusterAtlasPath);
            if (frames.Length < 4)
            {
                throw new InvalidOperationException(
                    "Thruster atlas must contain four frames.");
            }

            AnimationClip clip = GetOrCreateClip(
                PlayerThrusterClipPath,
                frames.Take(4).ToArray(),
                12f,
                true);

            AnimatorController controller = GetOrCreateController(
                PlayerThrusterControllerPath,
                clip);

            EditPrefab(PlayerPrefabPath, root =>
            {
                SpriteRenderer shipRenderer =
                    root.GetComponentInChildren<SpriteRenderer>(true);
                if (shipRenderer == null || shipRenderer.sprite == null)
                {
                    throw new InvalidOperationException(
                        "Player SpriteRenderer is missing.");
                }

                Transform existing = root.transform.Find("Thruster");
                GameObject thruster = existing != null
                    ? existing.gameObject
                    : new GameObject("Thruster", typeof(SpriteRenderer),
                        typeof(Animator));

                thruster.transform.SetParent(root.transform, false);

                SpriteRenderer renderer =
                    thruster.GetComponent<SpriteRenderer>() ??
                    thruster.AddComponent<SpriteRenderer>();
                renderer.sprite = frames[0];
                renderer.sortingLayerID = shipRenderer.sortingLayerID;
                renderer.sortingOrder = shipRenderer.sortingOrder - 1;

                Animator animator = thruster.GetComponent<Animator>() ??
                    thruster.AddComponent<Animator>();
                animator.runtimeAnimatorController = controller;

                float attachmentY = shipRenderer.sprite.bounds.min.y -
                    frames[0].bounds.max.y + 0.04f;
                thruster.transform.localPosition =
                    new Vector3(0f, attachmentY, 0f);
                thruster.transform.localRotation = Quaternion.identity;
                thruster.transform.localScale = Vector3.one * 0.55f;
            });
        }

        private static AnimationClip GetOrCreateClip(
            string path,
            IReadOnlyList<Sprite> frames,
            float frameRate,
            bool loop)
        {
            EnsureFolder(EffectsAnimationFolder);

            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
            {
                clip = new AnimationClip();
                AssetDatabase.CreateAsset(clip, path);
            }

            clip.frameRate = frameRate;
            clip.name = System.IO.Path.GetFileNameWithoutExtension(path);

            EditorCurveBinding binding = EditorCurveBinding.PPtrCurve(
                string.Empty,
                typeof(SpriteRenderer),
                "m_Sprite");

            ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[frames.Count];
            for (int i = 0; i < frames.Count; i++)
            {
                keys[i] = new ObjectReferenceKeyframe
                {
                    time = i / frameRate,
                    value = frames[i]
                };
            }

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

            AnimationClipSettings settings =
                AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            EditorUtility.SetDirty(clip);
            return clip;
        }

        private static AnimatorController GetOrCreateController(
            string path,
            AnimationClip clip)
        {
            AnimatorController controller =
                AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

            if (controller == null)
            {
                controller =
                    AnimatorController.CreateAnimatorControllerAtPath(path);
            }

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            AnimatorState state = stateMachine.states
                .Select(item => item.state)
                .FirstOrDefault(item => item.name == clip.name);

            if (state == null)
            {
                state = stateMachine.AddState(clip.name);
            }

            state.motion = clip;
            stateMachine.defaultState = state;
            EditorUtility.SetDirty(controller);
            return controller;
        }

        private static void AssignImage(
            Image image,
            Sprite sprite,
            bool allowSlicing = true)
        {
            if (image == null)
            {
                return;
            }

            image.sprite = sprite;
            image.type = allowSlicing && sprite.border.sqrMagnitude > 0f
                ? Image.Type.Sliced
                : Image.Type.Simple;
            image.color = Color.white;
        }

        private static Sprite LoadFirstSprite(string path)
        {
            Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<Sprite>()
                .FirstOrDefault();

            if (sprite == null)
            {
                throw new InvalidOperationException(
                    "Sprite was not found: " + path);
            }

            return sprite;
        }

        private static Sprite[] LoadSpritesLeftToRight(string path)
        {
            return LoadSprites(path)
                .OrderBy(sprite => sprite.rect.center.x)
                .ToArray();
        }

        private static Sprite[] LoadSpritesInReadingOrder(string path)
        {
            return LoadSprites(path)
                .OrderByDescending(sprite => sprite.rect.center.y)
                .ThenBy(sprite => sprite.rect.center.x)
                .ToArray();
        }

        private static IEnumerable<Sprite> LoadSprites(string path)
        {
            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<Sprite>()
                .ToArray();

            if (sprites.Length == 0)
            {
                throw new InvalidOperationException(
                    "No sliced sprites were found: " + path);
            }

            return sprites;
        }

        private static void EditPrefab(
            string path,
            Action<GameObject> edit)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            if (root == null)
            {
                throw new InvalidOperationException(
                    "Prefab was not found: " + path);
            }

            try
            {
                edit(root);
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
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

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }
    }
}
