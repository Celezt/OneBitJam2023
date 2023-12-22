using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class InlineListAttribute : Attribute
{
    public int ChildSpace { get; set; }
}
