using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlaylistDrawer : OdinValueDrawer<Playlist>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (label != null)
            EditorGUILayout.LabelField(label);

        foreach (var elementProperty in Property.Children)
        {
            elementProperty.Draw();
        }
    }
}
