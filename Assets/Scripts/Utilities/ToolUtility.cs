using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ToolUtility
{
#if UNITY_EDITOR
    private const int SCENE_TOOLBAR_HEIGHT = 70;

    public static Camera ViewCamera
    {
        get
        {
            if (!_viewCamera)
            {
               SceneView sceneView = SceneView.lastActiveSceneView;

                if (sceneView)
                    _viewCamera = sceneView.camera;
            }

            return _viewCamera;
        }
    }
    public static Vector2 GUIMousePosition
    {
        get
        {
            if (!ViewCamera)
                return new Vector2();

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 guiPosition = GUIUtility.ScreenToGUIPoint(mousePosition);

            return guiPosition;
        }
    }

    public static Vector2 SceneMousePosition
    {
        get
        {
            if (!ViewCamera)
                return new Vector2();

            if (EditorWindow.focusedWindow is SceneView)
            {
                Vector2 guiPosition = GUIMousePosition;
                Vector2 screenPosition = new Vector2(guiPosition.x, ViewCamera.pixelHeight + SCENE_TOOLBAR_HEIGHT - guiPosition.y);

                return screenPosition;
            }

            return new Vector2();
        }
    }

    private static Camera _viewCamera;

    public static bool IsHoveringScene(RectTransform rectTransform)
        => rectTransform.GetScreenRect().Contains(SceneMousePosition);

    public static bool IsFocusHoveringScene(RectTransform rectTransform)
    {
        if (Selection.activeGameObject != rectTransform.gameObject)
            return false;

        return IsHoveringScene(rectTransform);
    }

    public static bool IsFocusDraggingScene(RectTransform rectTransform)
    {
        if (!IsFocusHoveringScene(rectTransform))
            return false;

        return Mouse.current.leftButton.isPressed;
    }
#endif
}
