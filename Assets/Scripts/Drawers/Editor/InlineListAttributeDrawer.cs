using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[DrawerPriority(DrawerPriorityLevel.AttributePriority)]
public class InlineListAttributeDrawer<T> : OdinAttributeDrawer<InlineListAttribute, T> where T : IList
{
    private LocalPersistentContext<bool[]> _isExpandedCollection;
    private string[] _labels;

    protected override void Initialize()
    {
        int count = Property.Children.Count;

        _isExpandedCollection = this.GetPersistentValue($"{nameof(InlineListAttributeDrawer<T>)}.{nameof(_isExpandedCollection)}", new bool[count]);

        _labels = new string[count];
        for (int i = 0; i < count; i++)
            _labels[i] = ObjectNames.NicifyVariableName(Property.Children[i].ValueEntry.TypeOfValue.Name);
    }

    
    protected override void DrawPropertyLayout(GUIContent label)
    {
        int elementCount = Property.Children.Count;
        foreach (var elementProperty in Property.Children)
        {
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();

            int index = elementProperty.Index;
            bool isExpanded = _isExpandedCollection.Value[index] = SirenixEditorGUI.Foldout(_isExpandedCollection.Value[index], _labels[index]);
            SirenixEditorGUI.EndBoxHeader();

            if (SirenixEditorGUI.BeginFadeGroup(_labels[index], isExpanded)) // Use string label as key.
            {
                foreach (var valueProperty in elementProperty.Children)
                    valueProperty.Draw();
            }

            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();

            if (elementProperty.Index < elementCount - 1) // Ignore space on the last element. 
                EditorGUILayout.Space(Attribute.ChildSpace);
        }
    }
}
