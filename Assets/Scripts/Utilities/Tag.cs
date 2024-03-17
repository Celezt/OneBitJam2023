using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, InlineProperty]
public struct Tag
{
    [SerializeField, HorizontalGroup("Tag"), HideLabel]
    public string TagName;
    [SerializeField, HorizontalGroup("Tag", Width = 80, DisableAutomaticLabelWidth = true), HideLabel]
    public FilterType TagFilter;

    public enum FilterType
    {
        WhiteList,
        BlackList,
    }

    public bool Filter(Component component)
        => Filter(component.tag);
    public bool Filter(string tag)
    {
        if (string.IsNullOrWhiteSpace(TagName)) 
            return true;

        return TagFilter switch
        {
            FilterType.WhiteList => tag == TagName,
            FilterType.BlackList => tag != TagName,
            _ => throw new NotImplementedException(),
        };
    }
}
