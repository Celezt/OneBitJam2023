using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DrawerPriority(DrawerPriorityLevel.AttributePriority)]
public class MinFloatAttributeDrawer : OdinAttributeDrawer<MinAttribute, float>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        ValueEntry.SmartValue = Mathf.Max(Attribute.MinValue, ValueEntry.SmartValue);
        CallNextDrawer(label);
    }
}

[DrawerPriority(DrawerPriorityLevel.AttributePriority)]
public class MinIntAttributeDrawer : OdinAttributeDrawer<MinAttribute, int>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        ValueEntry.SmartValue = Mathf.Max((int)Attribute.MinValue, ValueEntry.SmartValue);
        CallNextDrawer(label);
    }
}
