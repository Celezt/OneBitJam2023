using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

[DrawerPriority(DrawerPriorityLevel.AttributePriority)]
public class MaxInt2AttributeDrawer : OdinAttributeDrawer<MaxAttribute, int2>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        ValueEntry.SmartValue = math.min((int)Attribute.MaxValue, ValueEntry.SmartValue);
        CallNextDrawer(label);
    }
}

[DrawerPriority(DrawerPriorityLevel.AttributePriority)]
public class MaxInt3AttributeDrawer : OdinAttributeDrawer<MaxAttribute, int3>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        ValueEntry.SmartValue = math.min((int)Attribute.MaxValue, ValueEntry.SmartValue);
        CallNextDrawer(label);
    }
}

[DrawerPriority(DrawerPriorityLevel.AttributePriority)]
public class MaxInt4AttributeDrawer : OdinAttributeDrawer<MaxAttribute, int4>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        ValueEntry.SmartValue = math.min((int)Attribute.MaxValue, ValueEntry.SmartValue);
        CallNextDrawer(label);
    }
}
