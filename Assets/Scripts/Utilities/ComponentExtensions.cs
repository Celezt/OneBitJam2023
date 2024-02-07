using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component)
    {
        component = gameObject.GetComponentInChildren<T>();

        return component != null;
    }
    public static bool TryGetComponentInChildren<T>(this Component behaviour, out T component)
    {
        component = behaviour.GetComponentInChildren<T>();

        return component != null;
    }

    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component)
    {
        component = gameObject.GetComponentInParent<T>();

        return component != null;
    }
    public static bool TryGetComponentInParent<T>(this Component behaviour, out T component)
    {
        component = behaviour.GetComponentInParent<T>();

        return component != null;
    }
}
