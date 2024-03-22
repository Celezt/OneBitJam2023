using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DrawerPriority(DrawerPriorityLevel.AttributePriority)]
public class MaxFloatAttributeDrawer : OdinAttributeDrawer<MaxAttribute, float>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        ValueEntry.SmartValue = Mathf.Min(Attribute.MaxValue, ValueEntry.SmartValue);
        CallNextDrawer(label);
    }
}

[DrawerPriority(DrawerPriorityLevel.AttributePriority)]
public class MaxIntAttributeDrawer : OdinAttributeDrawer<MaxAttribute, int>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        ValueEntry.SmartValue = Mathf.Min((int)Attribute.MaxValue, ValueEntry.SmartValue);
        CallNextDrawer(label);
    }
}
