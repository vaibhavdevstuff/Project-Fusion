
    using System;
    using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static partial class ToolbarUtility
{
    // PRIVATE MEMBERS

    private static ScriptableObject _mainToolbar;
    private static int _mainToolbarInstanceID;
    private static VisualElement _leftToolbar;
    private static VisualElement _rightToolbar;
    private static string _currentScene;
    private static string[] _scenePaths;
    private static string[] _sceneNames;

    // CONSTRUCTORS

    static ToolbarUtility()
    {
        EditorApplication.update -= Update;
        EditorApplication.update += Update;
    }

    // PUBLIC METHODS

    public static void InvalidateToolbar()
    {
        _mainToolbarInstanceID += 1;
    }

    // PARTIAL METHODS

    static partial void OnUpdate();
    static partial void OnLeftToolbarAttached(VisualElement toolbar);
    static partial void OnRightToolbarAttached(VisualElement toolbar);

    // PRIVATE METHODS

    private static void Update()
    {
        if (_scenePaths == null || _scenePaths.Length != EditorBuildSettings.scenes.Length)
        {
            List<string> scenePaths = new List<string>();
            List<string> sceneNames = new List<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.path == null || scene.path.StartsWith("Assets") == false)
                    continue;

                string scenePath = Application.dataPath + scene.path.Substring(6);

                scenePaths.Add(scenePath);
                sceneNames.Add(Path.GetFileNameWithoutExtension(scenePath));
            }

            _scenePaths = scenePaths.ToArray();
            _sceneNames = sceneNames.ToArray();

            InvalidateToolbar();
        }

        string currentScene = EditorSceneManager.GetActiveScene().name;
        if (_currentScene != currentScene)
        {
            _currentScene = currentScene;

            InvalidateToolbar();
        }

        if (_mainToolbar == null)
        {
            UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(typeof(Editor).Assembly.GetType("UnityEditor.Toolbar"));
            _mainToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
        }

        if (_mainToolbar != null)
        {
            int mainToolbarInstanceID = _mainToolbar.GetInstanceID();
            if (mainToolbarInstanceID != _mainToolbarInstanceID)
            {
                _mainToolbarInstanceID = mainToolbarInstanceID;

                RefreshToolbar(ref _leftToolbar, "ToolbarZoneLeftAlign", FlexDirection.Row, LeftToolbarAttached);
                RefreshToolbar(ref _rightToolbar, "ToolbarZoneRightAlign", FlexDirection.RowReverse, RightToolbarAttached);
            }
        }

        OnUpdate();
    }

    private static void RefreshToolbar(ref VisualElement toolbar, string toolbarID, FlexDirection direction, Action<VisualElement> onAttachToMainToolbar)
    {
        if (toolbar != null)
        {
            toolbar.RemoveFromHierarchy();
            toolbar = null;
        }

        FieldInfo root = _mainToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
        if (root != null)
        {
            object rawRoot = root.GetValue(_mainToolbar);
            if (rawRoot != null)
            {
                VisualElement toolbarRoot = rawRoot as VisualElement;
                VisualElement toolbarZone = toolbarRoot.Q(toolbarID);

                toolbar = new VisualElement()
                {
                    style =
                        {
                            flexGrow = 1,
                            flexDirection = direction,
                        }
                };

                toolbar.Add(new VisualElement()
                {
                    style =
                        {
                            flexGrow = 1,
                        }
                });

                toolbarZone.Add(toolbar);

                onAttachToMainToolbar(toolbar);
            }
        }
    }

    private static void LeftToolbarAttached(VisualElement toolbar)
    {
        OnLeftToolbarAttached(toolbar);
    }

    private static void RightToolbarAttached(VisualElement toolbar)
    {
        OnRightToolbarAttached(toolbar);

        // Replace the existing buttons with your custom "Player" button
        //toolbar.Add(CreateToolbarButton(OpenPlaygroundScene, "BuildSettings.Editor", "Playground"));
        //toolbar.Add(CreateToolbarButton(OpenMainScene, "BuildSettings.Editor", "Main"));
        toolbar.Add(CreateToolbarButton(OpenPlayerPrefab, "Prefab Icon", "Player"));
    }

    // Your custom method to open the player prefab
    private static void OpenPlayerPrefab()
    {
        string prefabPath = "Assets/Prefab/Player/Player.prefab";
        var prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
        if (prefab != null)
        {
            AssetDatabase.OpenAsset(prefab);
        }
        else
        {
            Debug.LogError("Player Prefab not found. Please provide the correct path.");
        }
    }

    private static void OpenPlaygroundScene()
    {
        string scenePath = "Assets/Scenes/Playground.unity";
        var Scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath); // Specify the type as SceneAsset

        if (Scene != null)
        {
            EditorSceneManager.OpenScene(scenePath); // Use EditorSceneManager to open the scene
        }
        else
        {
            Debug.LogError("Scene not found. Please provide the correct path.");
        }
    }

    private static void OpenMainScene()
    {
        string scenePath = "Assets/Scenes/MainMenu.unity";
        var Scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath); // Specify the type as SceneAsset

        if (Scene != null)
        {
            EditorSceneManager.OpenScene(scenePath); // Use EditorSceneManager to open the scene
        }
        else
        {
            Debug.LogError("Scene not found. Please provide the correct path.");
        }
    }

    // ... (Other methods remain unchanged)

    private static VisualElement CreateToolbarButton(Action onClick, string icon = null, string text = null, Color color = default)
    {
        Button buttonElement = new Button(onClick);
        buttonElement.AddToClassList("unity-toolbar-button");
        buttonElement.AddToClassList("unity-editor-toolbar-element");
        buttonElement.RemoveFromClassList("unity-button");
        buttonElement.style.marginRight = 2;
        buttonElement.style.marginLeft = 2;

        if (color != default)
        {
            buttonElement.style.color = color;
        }

        if (string.IsNullOrEmpty(icon) == false)
        {
            VisualElement iconElement = new VisualElement();
            iconElement.AddToClassList("unity-editor-toolbar-element__icon");
            // Use your own icon here, or customize as needed
            iconElement.style.backgroundImage = Background.FromTexture2D(EditorGUIUtility.IconContent(icon).image as Texture2D);
            buttonElement.Add(iconElement);
        }

        if (string.IsNullOrEmpty(text) == false)
        {
            TextElement textElement = new TextElement();
            textElement.text = text;
            textElement.style.marginLeft = 4;
            textElement.style.marginRight = 4;
            textElement.style.unityTextAlign = TextAnchor.MiddleCenter;
            buttonElement.Add(textElement);
        }

        return buttonElement;
    }






}

